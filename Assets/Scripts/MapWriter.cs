﻿using System;
using System.IO;

public static class MapWriter
{
    public static void SaveMap(string fileName, Map map)
    {
        if (Path.GetExtension(fileName) != ".rch")
        {
            throw new ArgumentException();
        }

        var strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var strWorkPath = Path.GetDirectoryName(strExeFilePath) + @"\..\..\Assets\Maps\";
        using var binaryWriter = new BinaryWriter(File.OpenWrite(strWorkPath + fileName));
        binaryWriter.Write(map.Width);
        binaryWriter.Write(map.Height);
        binaryWriter.Write(map.Depth);
        for (var x = 0; x < map.Width / ChunkData.ChunkSize; x++)
        {
            for (var y = 0; y < map.Height / ChunkData.ChunkSize; y++)
            {
                for (var z = 0; z < map.Depth / ChunkData.ChunkSize; z++)
                {
                    WriteChunk(z + y * (map.Depth / ChunkData.ChunkSize) +
                               x * (map.Height / ChunkData.ChunkSize * map.Depth / ChunkData.ChunkSize));
                }
            }
        }

        void WriteChunk(int chunkNumber)
        {
            for (var x = 0; x < ChunkData.ChunkSize; x++)
            {
                for (var y = 0; y < ChunkData.ChunkSize; y++)
                {
                    for (var z = 0; z < ChunkData.ChunkSize; z++)
                    {
                        binaryWriter.Write((byte) map.Chunks[chunkNumber].Blocks[x, y, z].Kind);
                    }
                }
            }
        }
    }
}