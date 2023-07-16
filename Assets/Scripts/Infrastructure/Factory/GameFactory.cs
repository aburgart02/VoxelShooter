﻿using System.Collections.Generic;
using Data;
using Infrastructure.AssetManagement;
using Infrastructure.Services.PlayerDataLoader;
using Infrastructure.Services.StaticData;
using Infrastructure.States;
using MapLogic;
using Networking;
using Rendering;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assets;
        private MapRenderer _mapGenerator;
        private readonly IStaticDataService _staticData;
        private const string NetworkManagerPath = "Prefabs/Network/LocalNetworkManager";
        private const string SteamNetworkManagerPath = "Prefabs/Network/SteamManager";
        private const string MapRendererPath = "Prefabs/MapCreation/MapRenderer";
        private const string ChunkRendererPath = "Prefabs/MapCreation/Chunk";
        private const string Wall = "Prefabs/MapCreation/Wall";
        private GameObject _networkManager;
        private readonly IEntityFactory _entityFactory;
        private readonly IAvatarLoader _avatarLoader;
        private readonly IPlayerFactory _playerFactory;
        private readonly IParticleFactory _particleFactory;


        public GameFactory(IAssetProvider assets, IEntityFactory entityFactory,
            IStaticDataService staticData, IParticleFactory particleFactory)
        {
            _assets = assets;
            _entityFactory = entityFactory;
            _particleFactory = particleFactory;
            _staticData = staticData;
        }

        public GameObject CreateLocalNetworkManager(GameStateMachine stateMachine, ServerSettings serverSettings)
        {
            _networkManager = _assets.Instantiate(NetworkManagerPath);
            _networkManager.GetComponent<CustomNetworkManager>().Construct(stateMachine, _staticData, _entityFactory,
                _particleFactory, _assets, this, serverSettings);
            return _networkManager;
        }

        public GameObject CreateSteamNetworkManager(GameStateMachine stateMachine, ServerSettings serverSettings,
            bool isHost)
        {
            _networkManager = _assets.Instantiate(SteamNetworkManagerPath);
            _networkManager.GetComponent<CustomNetworkManager>().Construct(stateMachine, _staticData, _entityFactory,
                _particleFactory, _assets, this, serverSettings);
            _networkManager.GetComponent<SteamLobby>().Construct(isHost);
            return _networkManager;
        }

        public void CreateWalls(MapProvider mapProvider)
        {
            _assets.Instantiate(Wall).GetComponent<WallRenderer>().Construct(mapProvider, Faces.Top);
            _assets.Instantiate(Wall).GetComponent<WallRenderer>().Construct(mapProvider, Faces.Bottom);
            _assets.Instantiate(Wall).GetComponent<WallRenderer>().Construct(mapProvider, Faces.Right);
            _assets.Instantiate(Wall).GetComponent<WallRenderer>().Construct(mapProvider, Faces.Left);
            _assets.Instantiate(Wall).GetComponent<WallRenderer>().Construct(mapProvider, Faces.Front);
            _assets.Instantiate(Wall).GetComponent<WallRenderer>().Construct(mapProvider, Faces.Back);
        }

        public void CreateMapRenderer(MapProvider mapProvider, Dictionary<Vector3Int, BlockData> buffer)
        {
            var mapGenerator = _assets.Instantiate(MapRendererPath);
            _mapGenerator = mapGenerator.GetComponent<MapRenderer>();
            _mapGenerator.Construct(mapProvider, this, buffer);
        }

        public ChunkRenderer CreateChunkRenderer(Vector3Int position, Quaternion rotation, Transform transform)
        {
            var chunkRenderer = _assets.Instantiate(ChunkRendererPath, position, rotation, transform)
                .GetComponent<ChunkRenderer>();
            chunkRenderer.Construct();
            return chunkRenderer;
        }

        public void CreateCamera()
        {
            new GameObject().AddComponent<Camera>();
        }
    }
}