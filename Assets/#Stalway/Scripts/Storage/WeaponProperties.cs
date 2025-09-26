using System;
using NaughtyAttributes;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [Flags]
    public enum DamageTypes
    {
        None = 0,       // Without damage (chicken)
        Instant = 1,    // Damage when shoot (knife)
        Effects = 2,    // Adding effects, like poison
    }

    public enum ShootTypes
    {
        None,       // Without shoot (chicken)
        Overlap,    // Overlaps a sphere (knife)
        Projectile, // Spawning projectile (laser gun)
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
        Bullets,   // With reload, but insert by bullet (sniper)
        Magazine,  // With reload, but limited ammo (rifle)
    }

    public interface IItemWeapon
    {
        public WeaponProperties WeaponProperties { get; }
    }

    [Serializable]
    public class WeaponProperties
    {
        [Header("Shoot")] public ShootTypes ShootType;
        [ShowIf("ShootType", ShootTypes.Projectile)] public GameObject Projectile;


        [Header("Overlap")]
        [ShowIf("ShootType", ShootTypes.Overlap)] public LayerMask AttackableLayers;
        [ShowIf("ShootType", ShootTypes.Overlap), MinValue(0)] public float Distance = 1f;
        [ShowIf("ShootType", ShootTypes.Overlap), MinValue(0)] public float HitForce = 400f;


        [Header("Overlap Damage")]
        [ShowIf("ShootType", ShootTypes.Overlap)] public DamageTypes DamageType = DamageTypes.Instant;
        [ShowIf("InstantDamageOverlap"), MinValue(0f)] public float Damage = 25f;
        [ShowIf("InstantDamageOverlap"), MinValue(0f)] public float ArmorDamage = 2f;
        [ShowIf("InstantDamageOverlap"), MinValue(0f), MaxValue(100f)] public float ArmorPenentration = 50f;


        [Header("Projectile")]
        [ShowIf("ShootType", ShootTypes.Projectile), MinValue(1)] public int BulletsPerShot = 1;
        [ShowIf("ShootType", ShootTypes.Projectile), MinValue(0)] public Vector2 Spread = new(0.25f, 0.25f);


        [Header("Automatics")] public AutomaticTypes AutomaticType = AutomaticTypes.Automatic;
        [ShowIf("AutomaticType", AutomaticTypes.Burst)] public int ShotsPerBurst = 3;
        [ShowIf("AutomaticType", AutomaticTypes.Burst)] public float FirerateInBurst = 0.05f;
        [ShowIf("AutomaticType", AutomaticTypes.Burst)] public float DelayAfterBurst = 0.25f;
        [MinValue(0f)] public float Firerate = 0.1f;


        [Header("Reload")] public ReloadTypes ReloadType = ReloadTypes.Magazine;
        [HideIf("ReloadType", ReloadTypes.None)] public int Ammo = 30;
        [ShowIf("ReloadType", ReloadTypes.Magazine)] public int AmmoInMagazine = 30;


        // Helper properties
        protected bool InstantDamage => DamageType.HasFlag(DamageTypes.Instant);
        protected bool InstantDamageOverlap => InstantDamage && ShootType == ShootTypes.Overlap;
    }
}