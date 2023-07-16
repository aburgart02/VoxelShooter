﻿using System;
using Data;
using UnityEngine;

namespace MapLogic
{
    public class MapUpdater : IMapUpdater
    {
        public event Action<Vector3Int, BlockData> MapUpdated;
        private readonly IMapProvider _mapProvider;

        public MapUpdater(IMapProvider mapProvider)
        {
            _mapProvider = mapProvider;
        }

        public void UpdateSpawnPoint(Vector3 oldPosition, Vector3 position)
        {
            var oldSpawnPoint = new SpawnPoint(Vector3Int.FloorToInt(oldPosition));
            var index = _mapProvider.MapData.SpawnPoints.FindIndex(point => point.Equals(oldSpawnPoint));
            _mapProvider.MapData.SpawnPoints[index] = new SpawnPoint(Vector3Int.FloorToInt(position));
        }

        public void SetBlockByGlobalPosition(Vector3Int position, BlockData blockData) =>
            SetBlockByGlobalPosition(position.x, position.y, position.z, blockData);

        private void SetBlockByGlobalPosition(int x, int y, int z, BlockData blockData)
        {
            _mapProvider.MapData.Chunks[_mapProvider.FindChunkNumberByPosition(x, y, z)]
                .Blocks[
                    x % ChunkData.ChunkSize * ChunkData.ChunkSizeSquared +
                    y % ChunkData.ChunkSize * ChunkData.ChunkSize + z % ChunkData.ChunkSize] = blockData;
            MapUpdated?.Invoke(new Vector3Int(x, y, z), blockData);
        }
    }
}