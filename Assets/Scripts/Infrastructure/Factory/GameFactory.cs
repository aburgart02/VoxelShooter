﻿using System.Collections.Generic;
using Data;
using Infrastructure.AssetManagement;
using Infrastructure.Services;
using Infrastructure.Services.Input;
using Inventory;
using MapLogic;
using Mirror;
using Networking;
using Networking.Synchronization;
using Rendering;
using UI;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assets;
        private MapRenderer _mapGenerator;
        private readonly IInputService _inputService;
        private readonly IStaticDataService _staticData;
        private const string NetworkManagerPath = "Prefabs/Network/LocalNetworkManager";
        private const string SteamNetworkManagerPath = "Prefabs/Network/SteamManager";
        private const string MapSynchronization = "Prefabs/MapCreation/MapSyncronization";
        private const string MapRendererPath = "Prefabs/MapCreation/MapRenderer";
        private const string ChunkRendererPath = "Prefabs/MapCreation/Chunk";
        private const string HudPath = "Prefabs/UI/HUD";
        private const string ChangeClassMenu = "Prefabs/UI/Change Class Menu";
        private GameObject _networkManager;


        public GameFactory(IAssetProvider assets, IInputService inputService, IStaticDataService staticData)
        {
            _inputService = inputService;
            _assets = assets;
            _staticData = staticData;
        }

        public GameObject CreateLocalNetworkManager(MapMessageHandler mapSynchronization)
        {
            _networkManager = _assets.Instantiate(NetworkManagerPath);
            _networkManager.GetComponent<CustomNetworkManager>().Construct(_staticData, mapSynchronization);
            return _networkManager;
        }

        public GameObject CreateSteamNetworkManager(MapMessageHandler mapSynchronization)
        {
            _networkManager = _assets.Instantiate(SteamNetworkManagerPath);
            _networkManager.GetComponent<CustomNetworkManager>().Construct(_staticData, mapSynchronization);
            return _networkManager;
        }

        public GameObject CreateMapSynchronization()
        {
            var mapSynchronization = _assets.Instantiate(MapSynchronization);
            NetworkServer.Spawn(mapSynchronization);
            return mapSynchronization;
        }

        public GameObject CreateMapRenderer(Map map,Dictionary<Vector3Int, BlockData> buffer)
        {
            var mapGenerator = _assets.Instantiate(MapRendererPath);
            _mapGenerator = mapGenerator.GetComponent<MapRenderer>();
            _mapGenerator.Construct(map, this, buffer);
            return mapGenerator;
        }

        public GameObject CreateHud(GameObject player)
        {
            var hud = _assets.Instantiate(HudPath);
            var inventoryController = hud.GetComponent<Hud>().inventory.GetComponent<InventoryController>();
            inventoryController.Construct(_inputService, this, hud, player);
            hud.GetComponent<Hud>().healthCounter.Construct(player);
            return hud;
        }

        public GameObject CreateGameModel(GameObject model, Transform itemPosition) => Object.Instantiate(model, itemPosition);

        public GameObject CreateChangeClassMenu()
        {
            var menu = _assets.Instantiate(ChangeClassMenu);
            menu.GetComponent<ChangeClassMenu>().Construct(_networkManager.GetComponent<CustomNetworkManager>()); 
            return menu;
        }

        public ChunkRenderer CreateChunkRenderer(Vector3Int position, Quaternion rotation, Transform transform)
        {
            var chunkRenderer = _assets.Instantiate(ChunkRendererPath, position, rotation, transform)
                .GetComponent<ChunkRenderer>();
            chunkRenderer.Construct();
            return chunkRenderer;
        }
    }
}