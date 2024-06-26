﻿using Infrastructure.AssetManagement;
using Networking;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IAssetProvider _assets;
        private readonly SpawnPointService _spawnPointService;

        public PlayerFactory(IAssetProvider assets, SpawnPointService spawnPointService)
        {
            _spawnPointService = spawnPointService;
            _assets = assets;
        }

        public GameObject CreatePlayer()
        {
            var player = _assets.Instantiate(PlayerPath.MainPlayerPath, _spawnPointService.GetSpawnPosition(), Quaternion.identity);
            return player;
        }

        public GameObject CreateSpectatorPlayer(Vector3 deathPosition)
        {
            var spectator = _assets.Instantiate(PlayerPath.SpectatorPlayerPath, deathPosition, Quaternion.identity);
            return spectator;
        }
    }
}