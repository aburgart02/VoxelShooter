﻿using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Grenade", menuName = "Inventory System/Inventory Items/Grenade")]

    public class GrenadeItem : InventoryItem
    {
        public Sprite grenadeCountIcon;
        public int count;
        public float delayInSeconds;
        public int radius;

        public void Awake()
        {
            itemType = ItemType.Grenade;
        }

    }
}