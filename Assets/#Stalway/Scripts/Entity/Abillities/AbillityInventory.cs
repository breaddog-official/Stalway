using Breaddog.Gameplay.StorageManagement;
using Breaddog.Network;
using Mirror;
using NaughtyAttributes;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class AbillityInventory : Abillity
    {
        [Header("Backpack")]
        public Vector2Int BackpackSize;

        public readonly SyncStorage Backpack = new();


        public override void OnStartServer()
        {
            Backpack.Resize(BackpackSize.x, BackpackSize.y);
        }

        [Command] // Its for test!!!
        public void AddItem(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                return;

            var item = Item.Create(itemName);
            Backpack.TryPlaceItem(item);
        }
    }
}