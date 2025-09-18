using Breaddog.AssetsManagement;
using Breaddog.Extensions;
using System;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [Serializable]
    public abstract class Item
    {
        public abstract string Name { get; }
        public abstract string NameTranslateKey { get; }
        public abstract string DescriptionTranslateKey { get; }
        public abstract string InventorySprite { get; }
        public abstract float Weight { get; }
        public abstract int MaxStack { get; }
        public abstract ItemUser Model { get; }
        public abstract Array2D<bool> Shape { get; }

        public virtual Item CreateInstance(string path) => this.DeepCopy();
        public static Item Create(string path)
        {
            using var prototype = AssetsManager.GetAsset<Item>(path);

            return prototype.asset.CreateInstance(path);
        }
    }
}