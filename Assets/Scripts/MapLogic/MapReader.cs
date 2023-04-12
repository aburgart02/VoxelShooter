﻿using System;
using System.Collections.Generic;
using System.IO;
using Data;
using UnityEngine;

namespace MapLogic
{
    public static class MapReader
    {
        public static Map ReadFromFile(string fileName)
        {
            if (Path.GetExtension(fileName) != ".rch")
            {
                if (Path.GetExtension(fileName) == ".vxl")
                {
                    return Vxl2RchConverter.LoadVxl(fileName);
                }

                throw new ArgumentException();
            }

            var filePath = Application.dataPath + $"/Maps/{fileName}";
            if (!File.Exists(filePath))
            {
                return Map.CreateNewMap();
            }

            using var file = File.OpenRead(filePath);
            return ReadFromStream(file);
        }

        public static Map ReadFromStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(stream);
            var width = binaryReader.ReadInt32();
            var height = binaryReader.ReadInt32();
            var depth = binaryReader.ReadInt32();
            var chunks = new ChunkData[width / ChunkData.ChunkSize * height / ChunkData.ChunkSize * depth /
                                       ChunkData.ChunkSize];
            var spawnPoints = new List<SpawnPoint>();
            var map = new Map(new MapData(chunks, width, height, depth, spawnPoints));
            for (var x = 0; x < width / ChunkData.ChunkSize; x++)
            {
                for (var y = 0; y < height / ChunkData.ChunkSize; y++)
                {
                    for (var z = 0; z < depth / ChunkData.ChunkSize; z++)
                    {
                        chunks[
                            z + y * (depth / ChunkData.ChunkSize) +
                            x * (height / ChunkData.ChunkSize * depth / ChunkData.ChunkSize)] = ReadChunk();
                    }
                }
            }

            var spawnPointCount = binaryReader.ReadInt32();
            for (var i = 0; i < spawnPointCount; i++)
            {
                var x = binaryReader.ReadInt32();
                var y = binaryReader.ReadInt32();
                var z = binaryReader.ReadInt32();
                spawnPoints.Add(new SpawnPoint() {X = x, Y = y, Z = z});
            }

            return map;

            ChunkData ReadChunk()
            {
                var chunk = new ChunkData();
                for (var x = 0; x < ChunkData.ChunkSize; x++)
                {
                    for (var y = 0; y < ChunkData.ChunkSize; y++)
                    {
                        for (var z = 0; z < ChunkData.ChunkSize; z++)
                        {
                            var blockColor = binaryReader.ReadUInt32();
                            var block = new BlockData
                            {
                                Color = BlockColor.UIntToColor(blockColor)
                            };
                            chunk.Blocks[x * ChunkData.ChunkSizeSquared + y * ChunkData.ChunkSize + z] = block;
                        }
                    }
                }

                return chunk;
            }
        }
    }
}