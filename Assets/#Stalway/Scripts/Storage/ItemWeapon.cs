using System;
using NaughtyAttributes;
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
        Magazine,  // With reload, but limited ammo (rifle)
        Bolt,      // With reload, but insert by bullet (sniper)
    }

    public enum PenentrationTypes
    {
        Nothing,   // Dont penentrate (knife or weak pistol)
        Specify,   // Specify count of penentrations (awp)
        Infinite,  // Penentrate all (serious bomb from Serious Sam)
    }

    public class ItemWeapon : ItemBasic
    {
        [Header("Shop")]
        [MinValue(0f)] public float Cost = 300f;
        [MinValue(1)] public int MaxBuys = 3;


        [Header("Shoot")] public ShootTypes ShootType = ShootTypes.Raycast;
        [ShowIf("ShootType", ShootTypes.Projectile)] public GameObject Projectile;


        [Header("Penentration")]
        [ShowIf("RaycastOrOverlap")] public PenentrationTypes PenentrationType = PenentrationTypes.Infinite;
        [ShowIf("RaycastOrOverlapPenSpecify"), MinValue(1)] public int PenentrationCount = 1;
        [ShowIf("RaycastOrOverlapPenSpecify")] public LayerMask PenentrationLayers;
        [ShowIf("RaycastOrOverlapPenentrate")] public LayerMask SolidLayers;


        [Header("Raycast and Overlap")]
        [ShowIf("RaycastOrOverlap")] public LayerMask AttackableLayers;
        [ShowIf("RaycastOrOverlap"), MinValue(0)] public float Distance = 1f;
        [ShowIf("RaycastOrProjectile"), MinValue(1)] public int BulletsPerShot = 1;
        [ShowIf("RaycastOrOverlap"), MinValue(0)] public float HitForce = 400f;
        [ShowIf("RaycastOrProjectile"), MinValue(0)] public Vector2 Spread = new(0.25f, 0.25f);


        [Header("Damage")]
        [ShowIf("RaycastOrOverlap")] public DamageTypes DamageType = DamageTypes.Instant;
        [ShowIf("RaycastOrOverlapInstantDamage"), MinValue(0f)] public float Damage = 25f;
        [ShowIf("RaycastOrOverlapInstantDamage"), MinValue(0f)] public float ArmorDamage = 2f;
        [ShowIf("RaycastOrOverlapInstantDamage"), MinValue(0f), MaxValue(100f)] public float ArmorPenentration = 50f;
        [ShowIf("RaycastOrOverlapInstantDamagePenSpecify"), MinValue(0f), MaxValue(100f)] public float PenentrationDamageReduction = 20f;


        [Header("Automatics")] public AutomaticTypes AutomaticType = AutomaticTypes.Automatic;
        [ShowIf("AutomaticType", AutomaticTypes.Burst)] public int ShotsPerBurst = 3;
        [MinValue(0f)] public float Firerate = 0.1f;


        [Header("Reload")] public ReloadTypes ReloadType = ReloadTypes.Magazine;
        [HideIf("ReloadType", ReloadTypes.None)] public int Ammo = 30;
        [ShowIf("ReloadType", ReloadTypes.Magazine)] public int AmmoInMagazine = 30;


        // Helper properties
        protected bool RaycastOrOverlap => ShootType == ShootTypes.Raycast || ShootType == ShootTypes.Overlap;
        protected bool RaycastOrProjectile => ShootType == ShootTypes.Raycast || ShootType == ShootTypes.Projectile;
        protected bool Penentrate => PenentrationType == PenentrationTypes.Specify || PenentrationType == PenentrationTypes.Nothing;
        protected bool InstantDamage => DamageType == DamageTypes.Instant;
        protected bool RaycastOrOverlapInstantDamage => InstantDamage && RaycastOrOverlap;
        protected bool RaycastOrOverlapInstantDamagePenSpecify => RaycastOrOverlapInstantDamage && PenentrationType == PenentrationTypes.Specify;
        protected bool RaycastOrOverlapPenSpecify => RaycastOrOverlap && PenentrationType == PenentrationTypes.Specify;
        protected bool RaycastOrOverlapPenentrate => RaycastOrOverlap && Penentrate;
    }
}