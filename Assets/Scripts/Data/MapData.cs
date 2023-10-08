﻿using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class MapData
    {
        public readonly Color32 SolidColor;
        public readonly Color32 WaterColor;
        public readonly int Width;
        public readonly int Depth;
        public readonly int Height;
        public readonly ChunkData[] Chunks;
        public HashSet<int> _solidBlocks = new();
        public HashSet<int> _blocksPlacedByPlayer = new();
        public Dictionary<int, Color32> _blockColors = new();

        public MapData(ChunkData[] chunks, int width, int height, int depth, Color32 solidColor, Color32 waterColor)
        {
            Chunks = chunks;
            Width = width;
            Height = height;
            Depth = depth;
            SolidColor = solidColor;
            WaterColor = waterColor;
        }
    }
}