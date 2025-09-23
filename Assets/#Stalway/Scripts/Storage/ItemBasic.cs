using System;
using Breaddog.Extensions;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [Serializable]
    public class ItemBasic : Item
    {
        public override string Name => name;
        public override string NameTranslateKey => nameTranslateKey;
        public override string DescriptionTranslateKey => descriptionTranslateKey;
        public override string InventorySprite => inventorySprite;
        public override string Model => model;
        public override float Weight => weight;
        public override int MaxStack => maxStack;
        public override Array2D<bool> Shape => shape;

        [Header("Basic Properties")]
        public string name;
        public string nameTranslateKey;
        public string descriptionTranslateKey;
        [FromAddressables(typeof(Sprite))]
        public string inventorySprite;
        [FromAddressables(typeof(GameObject))]
        public string model;
        public float weight;
        public int maxStack;
        [DrawShape]
        public Array2D<bool> shape;
    }
}