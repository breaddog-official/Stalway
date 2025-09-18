using Breaddog.Extensions;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    public class ItemBasic : Item
    {
        public override string Name => name;
        public override string NameTranslateKey => nameTranslateKey;
        public override string DescriptionTranslateKey => descriptionTranslateKey;
        public override string InventorySprite => sprite;
        public override float Weight => weight;
        public override int MaxStack => maxStack;
        public override ItemUser Model => model;
        public override Array2D<bool> Shape => shape;

        [Header("Basic Properties")]
        [SerializeField] private string name;
        [SerializeField] private string nameTranslateKey;
        [SerializeField] private string descriptionTranslateKey;
        [SerializeField] private string sprite;
        [SerializeField] public float weight;
        [SerializeField] public int maxStack;
        [SerializeField] private ItemUser model;
        [SerializeField] public Array2D<bool> shape;
    }
}