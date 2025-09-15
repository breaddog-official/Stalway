using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [CreateAssetMenu(fileName = "ItemBasic", menuName = "Items/Basic")]
    public class ItemBasic : Item
    {
        [ShowInInspector, PropertyOrder(-1)]
        public override string Name => name;
        public override string NameTranslateKey => nameTranslateKey;
        public override string DescriptionTranslateKey => descriptionTranslateKey;
        public override Sprite InventorySprite => sprite;
        public override ItemUser Model => model;

        [Header("Basic Properties")]
        [OdinSerialize] private string nameTranslateKey;
        [OdinSerialize] private string descriptionTranslateKey;
        [OdinSerialize] private ItemUser model;
        [OdinSerialize, PreviewField(100)] private Sprite sprite;
    }
}