﻿using UnityEngine;

namespace Core
{
    public class Map
    {
        public readonly int Width;
        public readonly int Depth;
        public readonly int Height;
        public readonly ChunkData[] Chunks;

        public Map(ChunkData[] chunks, int width, int height, int depth)
        {
            Chunks = chunks;
            Width = width;
            Height = height;
            Depth = depth;
        }


        public static Map CreateNewMap(int width = 512, int height = 64, int depth = 512)
        {
            var chunks = new ChunkData[width / ChunkData.ChunkSize * height / ChunkData.ChunkSize * depth /
                                       ChunkData.ChunkSize];
            for (var i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new ChunkData();
            }

            var map = new Map(chunks, width, height, depth);
            map.AddWater();
            return map;
        }

        public void AddWater()
        {
            var waterColor = new Color32();
            for (var x = 0; x < Width; x++)
            {
                for (var z = 0; z < Depth; z++)
                {
                    waterColor = Chunks[FindChunkNumberByPosition(new Vector3Int(x, 0, z))]
                        .Blocks[
                            (x & ChunkData.ChunkSize - 1) * ChunkData.ChunkSizeSquared +
                            (z & (ChunkData.ChunkSize - 1))].Color;
                }
            }

            if (waterColor.Equals(BlockColor.Empty))
                waterColor = new Color32(9, 20, 60, 255);

            for (var x = 0; x < Width; x++)
            {
                for (var z = 0; z < Depth; z++)
                {
                    var blocks = Chunks[FindChunkNumberByPosition(new Vector3Int(x, 0, z))].Blocks;
                    var block = blocks[
                        (x & (ChunkData.ChunkSize - 1)) * ChunkData.ChunkSizeSquared +
                        (z & (ChunkData.ChunkSize - 1))];
                    if (!block.Color
                            .Equals(BlockColor.Empty)) continue;
                    block.Color = waterColor;
                    blocks[(x & (ChunkData.ChunkSize - 1)) * ChunkData.ChunkSizeSquared +
                           (z & (ChunkData.ChunkSize - 1))] = block;
                }
            }
        }

        public int FindChunkNumberByPosition(Vector3Int position)
        {
            return position.z / ChunkData.ChunkSize +
                   position.y / ChunkData.ChunkSize * (Depth / ChunkData.ChunkSize) +
                   position.x / ChunkData.ChunkSize * (Height / ChunkData.ChunkSize * Depth / ChunkData.ChunkSize);
        }

        public bool IsValidPosition(Vector3Int globalPosition)
        {
            return !(globalPosition.x < 0 || globalPosition.x >= Width || globalPosition.y <= 0 ||
                   globalPosition.y >= Height ||
                   globalPosition.z < 0 || globalPosition.z >= Depth);
        }
    }
}