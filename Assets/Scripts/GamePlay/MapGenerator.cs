﻿using Core;
using UnityEngine;

namespace GamePlay
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private ChunkRenderer chunkPrefab;
        private Map Map { get; set; }
        public ChunkRenderer[] Chunks { get; private set; }

        public void Initialize(Map map)
        {
            Map = map;
            GlobalEvents.OnSaveMapEvent.AddListener(mapName => MapWriter.SaveMap(mapName, Map));
            Chunks = new ChunkRenderer[Map.Width / ChunkData.ChunkSize * Map.Height / ChunkData.ChunkSize *
                                       Map.Depth /
                                       ChunkData.ChunkSize];
            for (var x = 0; x < Map.Width / ChunkData.ChunkSize; x++)
            {
                for (var y = 0; y < Map.Height / ChunkData.ChunkSize; y++)
                {
                    for (var z = 0; z < Map.Depth / ChunkData.ChunkSize; z++)
                    {
                        var index = z + y * Map.Depth / ChunkData.ChunkSize +
                                    x * Map.Height / ChunkData.ChunkSize * Map.Depth / ChunkData.ChunkSize;
                        var chunkRenderer = Instantiate(
                            chunkPrefab, new Vector3Int(x * ChunkData.ChunkSize, y * ChunkData.ChunkSize,
                                z * ChunkData.ChunkSize), Quaternion.identity, transform);
                        Chunks[index] = chunkRenderer;
                        Chunks[index].ChunkData = Map.Chunks[index];
                    }
                }
            }

            for (var i = 0; i < Chunks.Length; i++)
            {
                if (i + 1 < Chunks.Length &&
                    i / (map.Depth / ChunkData.ChunkSize) == (i + 1) / (map.Depth / ChunkData.ChunkSize))
                    Chunks[i].FrontNeighbour = Chunks[i + 1];
                if (i - 1 >= 0 && i / (map.Depth / ChunkData.ChunkSize) == (i - 1) / (map.Depth / ChunkData.ChunkSize))
                    Chunks[i].BackNeighbour = Chunks[i - 1];
                if (i + Map.Depth / ChunkData.ChunkSize < Chunks.Length &&
                    i / (map.Depth / ChunkData.ChunkSize * map.Height / ChunkData.ChunkSize) ==
                    (i + Map.Depth / ChunkData.ChunkSize) /
                    (map.Depth / ChunkData.ChunkSize * map.Height / ChunkData.ChunkSize))
                    Chunks[i].UpperNeighbour = Chunks[i + Map.Depth / ChunkData.ChunkSize];
                if (i - Map.Depth / ChunkData.ChunkSize >= 0 &&
                    i / (map.Depth / ChunkData.ChunkSize * map.Height / ChunkData.ChunkSize) ==
                    (i - Map.Depth / ChunkData.ChunkSize) /
                    (map.Depth / ChunkData.ChunkSize * map.Height / ChunkData.ChunkSize))
                    Chunks[i].LowerNeighbour = Chunks[i - Map.Depth / ChunkData.ChunkSize];
                if (i + Map.Height / ChunkData.ChunkSize * Map.Depth / ChunkData.ChunkSize < Chunks.Length)
                    Chunks[i].LeftNeighbour =
                        Chunks[i + Map.Height / ChunkData.ChunkSize * Map.Depth / ChunkData.ChunkSize];
                if (i - Map.Height / ChunkData.ChunkSize * Map.Depth / ChunkData.ChunkSize >= 0)
                    Chunks[i].RightNeighbour =
                        Chunks[i - Map.Height / ChunkData.ChunkSize * Map.Depth / ChunkData.ChunkSize];
            }
        }
    }
}