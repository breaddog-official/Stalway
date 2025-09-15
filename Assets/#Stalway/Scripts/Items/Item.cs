using Breaddog.Extensions;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [Serializable]
    public abstract class Item : SerializedScriptableObject
    {
        public abstract string Name { get; }
        public abstract string NameTranslateKey { get; }
        public abstract string DescriptionTranslateKey { get; }
        public abstract Sprite InventorySprite { get; }
        public abstract ItemUser Model { get; }
    }
}