using Breaddog.Gameplay.StorageManagement;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class AbillityInventory : Abillity
    {
        [Header("Links")]
        [OdinSerialize] public Transform ItemParent { get; private set; }


        [Header("Indexes")]
        [OdinSerialize] public int PrimaryWeaponIndex { get; private set; } = 0;
        [OdinSerialize] public int HolsterWeaponIndex { get; private set; } = 1;
        [OdinSerialize] public int KnifeIndex { get; private set; } = 2;
        [OdinSerialize] public int BombIndex { get; private set; } = 3;
        [OdinSerialize] public int GrenadesIndex { get; private set; } = 4;


        [Header("Items")]
        [OdinSerialize] public readonly SyncList<Item> Items = new();
        [field: SyncVar(hook = nameof(UpdateItem))]
        [OdinSerialize] public int CurrentItemIndex { get; private set; } = -1;

        public Item CurrentItem => Items[CurrentItemIndex];
        public ItemUser SpawnedItem { get; protected set; }


        public override void Init()
        {
            if (CurrentItemIndex > -1)
            {
                UpdateItem();
            }
        }

        public void UpdateItem(int oldItem, int newItem) => UpdateItem();
        public void UpdateItem()
        {
            if (SpawnedItem != null)
            {
                SpawnedItem.CancelUsing();
                // Dequip animation
                Destroy(SpawnedItem);
            }

            SpawnedItem = Instantiate(CurrentItem.Model, ItemParent);

        }
    }
}