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
    }
}