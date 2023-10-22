﻿using System.IO;
using Data;
using Entities;
using Explosions;
using Infrastructure;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Infrastructure.Services.StaticData;
using MapLogic;
using Mirror;
using Networking.MessageHandlers.RequestHandlers;
using Networking.Messages.Responses;
using Networking.ServerServices;
using PlayerLogic.States;
using Steamworks;
using UnityEngine;
using ChangeSlotHandler = Networking.MessageHandlers.RequestHandlers.ChangeSlotHandler;
using ShootHandler = Networking.MessageHandlers.RequestHandlers.ShootHandler;

namespace Networking
{
    public class Server : IServer
    {
        private const string SpawnPointContainerName = "SpawnPointContainer";
        public MapProvider MapProvider { get; }
        public ServerData ServerData { get; }
        public MapUpdater MapUpdater { get; }
        private readonly IGameFactory _gameFactory;
        private readonly ServerSettings _serverSettings;
        private readonly EntityPositionValidator _entityPositionValidator;
        private readonly IEntityFactory _entityFactory;
        private readonly IPlayerFactory _playerFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly AddBlocksHandler _addBlocksHandler;
        private readonly ChangeClassHandler _changeClassHandler;
        private readonly GrenadeSpawnHandler _grenadeSpawnHandler;
        private readonly RocketSpawnHandler _rocketSpawnHandler;
        private readonly TntSpawnHandler _tntSpawnHandler;
        private readonly ChangeSlotHandler _changeSlotHandler;
        private readonly ShootHandler _shootHandler;
        private readonly ReloadHandler _reloadHandler;
        private readonly HitHandler _hitHandler;

        public Server(ICoroutineRunner coroutineRunner, IStaticDataService staticData,
            ServerSettings serverSettings, IAssetProvider assets, IGameFactory gameFactory,
            IParticleFactory particleFactory, IEntityFactory entityFactory)
        {
            _coroutineRunner = coroutineRunner;
            _serverSettings = serverSettings;
            _gameFactory = gameFactory;
            _entityFactory = entityFactory;
            MapProvider = MapReader.ReadFromFile(_serverSettings.MapName, staticData);
            MapUpdater = new MapUpdater(_coroutineRunner, MapProvider);
            ServerData = new ServerData(staticData);
            _entityPositionValidator = new EntityPositionValidator(MapUpdater, MapProvider);
            _playerFactory = new PlayerFactory(this, assets);
            var sphereExplosionArea = new SphereExplosionArea(MapProvider);
            var singleExplosionBehaviour = new SingleExplosionBehaviour(this, particleFactory,
                sphereExplosionArea);
            var chainExplosionBehaviour = new ChainExplosionBehaviour(this, particleFactory,
                sphereExplosionArea);
            _addBlocksHandler = new AddBlocksHandler(this);
            _changeClassHandler = new ChangeClassHandler(this);
            _changeSlotHandler = new ChangeSlotHandler(this);
            _grenadeSpawnHandler = new GrenadeSpawnHandler(this, coroutineRunner, entityFactory, staticData,
                singleExplosionBehaviour);
            _rocketSpawnHandler = new RocketSpawnHandler(this, staticData, entityFactory, particleFactory);
            _tntSpawnHandler =
                new TntSpawnHandler(this, coroutineRunner, entityFactory, staticData, chainExplosionBehaviour);
            var rangeWeaponValidator = new RangeWeaponValidator(this, coroutineRunner, particleFactory);
            var meleeWeaponValidator = new MeleeWeaponValidator(this, coroutineRunner, particleFactory);
            _shootHandler = new ShootHandler(this, rangeWeaponValidator);
            _reloadHandler = new ReloadHandler(this, rangeWeaponValidator);
            _hitHandler = new HitHandler(this, meleeWeaponValidator);
        }

        public void Start()
        {
            RegisterHandlers();
            CreateSpawnPoints();
        }

        public void AddPlayer(NetworkConnectionToClient connection, GameClass chosenClass, CSteamID steamID,
            string nickname)
        {
            ServerData.AddPlayer(connection, chosenClass, steamID, nickname);
            _playerFactory.CreatePlayer(connection);
            NetworkServer.SendToAll(new ScoreboardResponse(ServerData.GetScoreData()));
        }

        public void ChangeClass(NetworkConnectionToClient connection, GameClass chosenClass)
        {
            var playerData = ServerData.GetPlayerData(connection);
            if (playerData.GameClass == chosenClass) return;
            playerData.GameClass = chosenClass;
            if (playerData.IsAlive)
            {
                playerData.PlayerStateMachine.Enter<DeathState>();
                _playerFactory.CreateSpectatorPlayer(connection);
                var respawnTimer = new RespawnTimer(_coroutineRunner, connection, _serverSettings.SpawnTime,
                    () => _playerFactory.RespawnPlayer(connection));
                respawnTimer.Start();
            }

            NetworkServer.SendToAll(new ScoreboardResponse(ServerData.GetScoreData()));
        }

        public void DeletePlayer(NetworkConnectionToClient connection)
        {
            ServerData.DeletePlayer(connection);
            NetworkServer.SendToAll(new ScoreboardResponse(ServerData.GetScoreData()));
            NetworkServer.DestroyPlayerForConnection(connection);
        }

        public void AddKill(NetworkConnectionToClient killer, NetworkConnectionToClient victim)
        {
            var tombstonePosition = Vector3Int.FloorToInt(victim.identity.transform.position) +
                                    Constants.WorldOffset;
            var tombstone = _entityFactory.CreateTombstone(tombstonePosition);
            _entityPositionValidator.AddEntity(tombstone.GetComponent<PushableObject>());
            ServerData.AddKill(killer, victim);
            var playerData = ServerData.GetPlayerData(victim);
            playerData.PlayerStateMachine.Enter<DeathState>();
            _playerFactory.CreateSpectatorPlayer(victim);
            var respawnTimer = new RespawnTimer(_coroutineRunner, victim, _serverSettings.SpawnTime,
                () => _playerFactory.RespawnPlayer(victim));
            respawnTimer.Start();
            NetworkServer.SendToAll(new ScoreboardResponse(ServerData.GetScoreData()));
        }

        public void Damage(NetworkConnectionToClient source, NetworkConnectionToClient receiver, int totalDamage)
        {
            var result = ServerData.TryGetPlayerData(receiver, out var playerData);
            if (!result || !playerData.IsAlive) return;
            playerData.Health -= totalDamage;
            if (playerData.Health <= 0)
            {
                playerData.Health = 0;
                AddKill(source, receiver);
            }
            else
            {
                receiver.Send(new HealthResponse(playerData.Health));
            }
        }

        public void SendMap(NetworkConnectionToClient connection)
        {
            connection.Send(new MapNameResponse(MapProvider.MapName));
            using var memoryStream = new MemoryStream();
            MapWriter.WriteMap(MapProvider, memoryStream);
            var bytes = memoryStream.ToArray();
            var mapSplitter = new MapSplitter();
            var mapMessages = mapSplitter.SplitBytesIntoMessages(bytes, Constants.MessageSize);
            _coroutineRunner.StartCoroutine(mapSplitter.SendMessages(mapMessages, connection,
                Constants.MessageDelay));
        }

        public void Stop()
        {
            UnregisterHandlers();
        }

        private void RegisterHandlers()
        {
            _addBlocksHandler.Register();
            _changeClassHandler.Register();
            _changeSlotHandler.Register();
            _grenadeSpawnHandler.Register();
            _rocketSpawnHandler.Register();
            _tntSpawnHandler.Register();
            _shootHandler.Register();
            _reloadHandler.Register();
            _hitHandler.Register();
        }

        private void UnregisterHandlers()
        {
            _addBlocksHandler.Unregister();
            _changeClassHandler.Unregister();
            _changeSlotHandler.Unregister();
            _grenadeSpawnHandler.Unregister();
            _rocketSpawnHandler.Unregister();
            _tntSpawnHandler.Unregister();
            _shootHandler.Unregister();
            _reloadHandler.Unregister();
            _hitHandler.Unregister();
        }

        private void CreateSpawnPoints()
        {
            var parent = _gameFactory.CreateGameObjectContainer(SpawnPointContainerName).transform;
            foreach (var spawnPointData in MapProvider.SceneData.SpawnPoints)
            {
                var spawnPointScript = _entityFactory.CreateSpawnPoint(spawnPointData.ToVectorWithOffset(), parent)
                    .GetComponent<SpawnPoint>();
                spawnPointScript.Construct(spawnPointData);
                _entityPositionValidator.AddEntity(spawnPointScript);
                spawnPointScript.PositionUpdated += MapUpdater.UpdateSpawnPoint; // TODO : Need to unsubscribe
            }
        }
    }
}