﻿using System.IO;
using UnityEngine;

namespace GamePlay
{
    public static class MapReader
    {
        public static Map CreateNewMap(int width = 512, int height = 64, int depth = 512)
        {
            var chunks = new ChunkData[width / ChunkData.ChunkSize * height / ChunkData.ChunkSize * depth /
                                       ChunkData.ChunkSize];
            for (var i = 0; i < chunks.Length; i++)
            {
                chunks[i] = new ChunkData();
            }

            return new Map(chunks, width, height, depth);
        }

        public static Map ReadFromFile(string fileName)
        {
            var filePath = Application.dataPath + $"/Maps/{fileName}";
            if (!File.Exists(filePath))
            {
                return CreateNewMap();
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
            var map = new Map(chunks, width, height, depth);
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
                            var blockColor = binaryReader.ReadByte();
                            var block = new Block()
                            {
                                ColorID = blockColor
                            };
                            chunk.Blocks[x,y,z] = block;
                        }
                    }
                }

                return chunk;
            }
        }
    }
}