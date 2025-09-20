using Breaddog.Network;
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
    }
}