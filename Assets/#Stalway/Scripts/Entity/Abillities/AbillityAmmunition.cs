using Breaddog.Gameplay.StorageManagement;
using Mirror;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class AbillityAmmunition : Abillity
    {
        [Header("Links")]
        public Transform ItemParent;

        [field: SyncVar] public ItemWeapon WeaponFirst { get; protected set; }
        [field: SyncVar] public ItemWeapon WeaponSecond { get; protected set; }
        [field: SyncVar] public ItemWeapon WeaponHolster { get; protected set; }
        [field: SyncVar] public ItemWeapon WeaponKnife { get; protected set; }
    }
}