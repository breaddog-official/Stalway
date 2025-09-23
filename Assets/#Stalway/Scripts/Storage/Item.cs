using Breaddog.AssetsManagement;
using Breaddog.Extensions;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [Serializable, JsonObject]
    public abstract class Item
    {
        [JsonIgnore] public abstract string Name { get; }
        [JsonIgnore] public abstract string NameTranslateKey { get; }
        [JsonIgnore] public abstract string DescriptionTranslateKey { get; }
        [JsonIgnore] public abstract string InventorySprite { get; }
        [JsonIgnore] public abstract string Model { get; }
        [JsonIgnore] public abstract float Weight { get; }
        [JsonIgnore] public abstract int MaxStack { get; }
        [JsonIgnore] public abstract Array2D<bool> Shape { get; }

        public virtual Item CreateInstance(string path) => this.DeepCopy();
        public static Item Create(string path)
        {
            using var prototype = AssetsManager.GetAsset<Item>(path);

            return prototype.asset.CreateInstance(path);
        }
    }
}