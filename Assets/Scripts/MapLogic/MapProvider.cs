﻿using System;
using Data;
using UnityEngine;

namespace MapLogic
{
    [Serializable]
    public class MapProvider
    {
        public MapData MapData { get; }

        public MapProvider(MapData mapData)
        {
            MapData = mapData;
        }

        public BlockData GetBlockByGlobalPosition(Vector3Int position) =>
            GetBlockByGlobalPosition(position.x, position.y, position.z);

        public BlockData GetBlockByGlobalPosition(int x, int y, int z)
        {
            return MapData.Chunks[GetChunkNumberByGlobalPosition(x, y, z)]
                .Blocks[
                    x % ChunkData.ChunkSize * ChunkData.ChunkSizeSquared +
                    y % ChunkData.ChunkSize * ChunkData.ChunkSize + z % ChunkData.ChunkSize];
        }


        public int GetChunkNumberByGlobalPosition(int x, int y, int z)
        {
            return z / ChunkData.ChunkSize +
                   y / ChunkData.ChunkSize * (MapData.Depth / ChunkData.ChunkSize) +
                   x / ChunkData.ChunkSize *
                   (MapData.Height / ChunkData.ChunkSize * MapData.Depth / ChunkData.ChunkSize);
        }

        public int GetChunkNumberByGlobalPosition(Vector3Int position)
        {
            return position.z / ChunkData.ChunkSize +
                   position.y / ChunkData.ChunkSize * (MapData.Depth / ChunkData.ChunkSize) +
                   position.x / ChunkData.ChunkSize *
                   (MapData.Height / ChunkData.ChunkSize * MapData.Depth / ChunkData.ChunkSize);
        }

        public Vector3Int GetLocalPositionByGlobal(Vector3Int globalPosition)
        {
            return new Vector3Int(globalPosition.x % ChunkData.ChunkSize, globalPosition.y % ChunkData.ChunkSize,
                globalPosition.z % ChunkData.ChunkSize);
        }

        public bool IsValidPosition(Vector3Int globalPosition)
        {
            return !(globalPosition.x < 0 || globalPosition.x >= MapData.Width || globalPosition.y <= 0 ||
                     globalPosition.y >= MapData.Height ||
                     globalPosition.z < 0 || globalPosition.z >= MapData.Depth);
        }

        public int GetChunkCount()
        {
            return MapData.Width / ChunkData.ChunkSize * MapData.Height / ChunkData.ChunkSize * MapData.Depth /
                   ChunkData.ChunkSize;
        }
    }
}