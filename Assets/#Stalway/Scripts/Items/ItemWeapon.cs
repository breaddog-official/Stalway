using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    public enum DamageTypes
    {
        None,       // Without damage (chicken)
        Instant,    // Damage when shoot (knife)
    }

    public enum ShootTypes
    {
        None,       // Without shoot (chicken)
        Overlap,    // Overlaps a sphere (knife)
        Projectile, // Spawning projectile (laser gun)
        Raycast,    // Throw raycast (pistol)
    }

    public enum AutomaticTypes
    {
        Automatic,  // Spray fire (knife or rifle)
        Burst,      // Burst fire (famas or glock)
        SemiAuto,   // Tap fire (svd)
    }

    public enum ReloadTypes
    {
        None,      // Without reload (knife)
        Cooling,   // With reload, but infinite ammo (laser gun)
        Magazine,  // With reload, but limited ammo (pistol)
        Bolt,      // With reload, but insert by bullet (sniper)
    }

    public enum PenentrationTypes
    {
        Nothing,   // Dont penentrate (knife)
        Specify,   // Specify count of penentrations (awp)
        Infinite,  // Penentrate all (serious bomb from Serious Sam)
    }

    [CreateAssetMenu(fileName = "ItemWeapon", menuName = "Items/Weapon")]
    public class ItemWeapon : ItemBasic
    {
        [TabGroup("props", "Stats", SdfIconType.InfoCircle, TextColor = "white")]
        [Header("Shop")]
        [TabGroup("props", "Stats"), MinValue(0f), Unit("Dollars")]
        [OdinSerialize] public float Cost { get; protected set; } = 2900f;
        [TabGroup("props", "Stats"), MinValue(1)]
        [OdinSerialize] public int MaxBuys { get; protected set; } = 3;


        [TabGroup("props", "Shoot", SdfIconType.Magic, TextColor = "orange")]
        [OdinSerialize] public ShootTypes ShootType { get; protected set; } = ShootTypes.Raycast;
        [TabGroup("props", "Shoot"), ShowIf("@ShootType == ShootTypes.Projectile"), PreviewField(100)]
        [OdinSerialize] public GameObject Projectile { get; protected set; }


        [TabGroup("props", "Shoot"), Header("Penentration"), ShowIf("@RaycastOrOverlap")]
        [OdinSerialize] public PenentrationTypes PenentrationType { get; protected set; } = PenentrationTypes.Infinite;
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrOverlap && PenentrationType == PenentrationTypes.Specify"), MinValue(1)]
        [OdinSerialize] public int PenentrationCount { get; protected set; } = 1;
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrOverlap && Penentrate && PenentrationType == PenentrationTypes.Specify")]
        [OdinSerialize] public LayerMask PenentrationLayers { get; protected set; }
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrOverlap && Penentrate")]
        [OdinSerialize] public LayerMask SolidLayers { get; protected set; }


        [TabGroup("props", "Shoot"), Header("Stats"), ShowIf("@RaycastOrOverlap")]
        [OdinSerialize] public LayerMask AttackableLayers { get; protected set; }
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrOverlap"), MinValue(0), Unit(Units.Meter)]
        [OdinSerialize] public float Distance { get; protected set; } = 1f;
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrProjectile"), MinValue(1)]
        [OdinSerialize] public int BulletsPerShot { get; protected set; } = 1;
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrOverlap"), MinValue(0)]
        [OdinSerialize] public float HitForce { get; protected set; } = 400f;
        [TabGroup("props", "Shoot"), ShowIf("@RaycastOrProjectile"), MinValue(0)]
        [OdinSerialize] public Vector2 Spread { get; protected set; } = new(0.25f, 0.25f);


        [TabGroup("props", "Shoot"), Header("Damage"), ShowIf("@RaycastOrOverlap")]
        [OdinSerialize] public DamageTypes DamageType { get; protected set; } = DamageTypes.Instant;
        [TabGroup("props", "Shoot"), ShowIf("@InstantDamage && RaycastOrOverlap"), MinValue(0f)]
        [OdinSerialize] public float Damage { get; protected set; } = 25f;
        [TabGroup("props", "Shoot"), ShowIf("@InstantDamage && RaycastOrOverlap"), MinValue(0f)]
        [OdinSerialize] public float ArmorDamage { get; protected set; } = 2f;
        [TabGroup("props", "Shoot"), ShowIf("@InstantDamage && RaycastOrOverlap"), MinValue(0f), MaxValue(100f), Unit(Units.Percent)]
        [OdinSerialize] public float ArmorPenentration { get; protected set; } = 50f;
        [TabGroup("props", "Shoot"), ShowIf("@InstantDamage && RaycastOrOverlap && PenentrationType == PenentrationTypes.Specify"), MinValue(0f), MaxValue(100f), Unit(Units.Percent)]
        [OdinSerialize] public float PenentrationDamageReduction { get; protected set; } = 20f;


        [TabGroup("props", "Automatics", SdfIconType.Check2All, TextColor = "cyan")]
        [OdinSerialize] public AutomaticTypes AutomaticType { get; protected set; } = AutomaticTypes.Automatic;
        [TabGroup("props", "Automatics"), ShowIf("@AutomaticType == AutomaticTypes.Burst")]
        [OdinSerialize] public int ShotsPerBurst { get; protected set; } = 3;
        [TabGroup("props", "Automatics"), Unit(Units.Second, "RoundsPerMinute")]
        [OdinSerialize] public float Firerate { get; protected set; } = 0.1f;


        [TabGroup("props", "Reload", SdfIconType.ArrowCounterclockwise, TextColor = "yellow")]
        [OdinSerialize] public ReloadTypes ReloadType { get; protected set; } = ReloadTypes.Magazine;
        [TabGroup("props", "Reload"), ShowIf("@ReloadType != ReloadTypes.None")]
        [OdinSerialize] public int Ammo { get; protected set; } = 30;
        [TabGroup("props", "Reload"), ShowIf("@ReloadType == ReloadTypes.Magazine")]
        [OdinSerialize] public int AmmoInMagazine { get; protected set; } = 30;


        // Helper properties
        protected bool RaycastOrOverlap => ShootType == ShootTypes.Raycast || ShootType == ShootTypes.Overlap;
        protected bool RaycastOrProjectile => ShootType == ShootTypes.Raycast || ShootType == ShootTypes.Projectile;
        protected bool Penentrate => PenentrationType == PenentrationTypes.Specify || PenentrationType == PenentrationTypes.Nothing;
        protected bool InstantDamage => DamageType == DamageTypes.Instant;
    }
}