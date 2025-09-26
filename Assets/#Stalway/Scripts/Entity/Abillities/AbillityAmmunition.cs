using Breaddog.Gameplay.StorageManagement;
using Mirror;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class AbillityAmmunition : Abillity
    {
        [Header("Links")]
        public Transform ItemParent;

        [field: SyncVar] public IItemWeapon WeaponFirst { get; protected set; }
        [field: SyncVar] public IItemWeapon WeaponSecond { get; protected set; }
        [field: SyncVar] public IItemWeapon WeaponHolster { get; protected set; }
        [field: SyncVar] public IItemWeapon WeaponKnife { get; protected set; }
    }
}