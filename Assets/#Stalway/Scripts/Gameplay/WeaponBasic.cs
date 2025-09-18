using System.Linq;
using Breaddog.Extensions;
using Breaddog.Gameplay.StorageManagement;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class WeaponBasic : Weapon
    {
        public Transform ShootPoint;

        protected Collider[] colliders;
        protected RaycastHit[] hits;

        protected int ammo = 30;
        protected bool isShooting;

        protected float delay;
        protected int bulletsInBurst;


        protected virtual void Awake()
        {
            colliders = new Collider[16];
            hits = new RaycastHit[16];
        }

        protected virtual void Update()
        {
            UpdateAutomaticsSystem();
        }

        protected virtual void UpdateAutomaticsSystem()
        {
            if (Item.AutomaticType == AutomaticTypes.Automatic || Item.AutomaticType == AutomaticTypes.Burst)
            {
                delay += Time.deltaTime;

                if (isShooting && delay >= Item.Firerate)
                {
                    delay = 0f;
                    Fire();

                    if (Item.AutomaticType == AutomaticTypes.SemiAuto)
                    {
                        isShooting = false;
                    }

                    if (Item.AutomaticType == AutomaticTypes.Burst)
                    {
                        bulletsInBurst++;
                        if (bulletsInBurst > Item.ShotsPerBurst)
                        {
                            isShooting = false;
                        }
                    }
                }
            }
        }



        /// <summary> Pull the trigger </summary>
        public override void StartFire()
        {
            isShooting = true;
        }

        /// <summary> Release the trigger </summary>
        public override void StopFire()
        {
            // Burst and SemiAuto modes manage isShooting independently
            if (Item.AutomaticType == AutomaticTypes.Automatic)
                isShooting = false;
        }

        /// <summary> Same as StopFire, but also cancels Burst and SemiAuto modes </summary>
        public override void CancelFire()
        {
            base.CancelFire();

            isShooting = false;
            bulletsInBurst = 0;
            delay = 0f;
        }




        /// <summary> Begins the shooting process </summary>
        protected virtual void Fire()
        {
            if (CanShoot())
                Shoot();
        }

        /// <summary> Checks stats to see if you can shoot </summary>
        protected virtual bool CanShoot()
        {
            if (Item.ReloadType != ReloadTypes.None && ammo == 0)
                return false;

            return true;
        }

        #region Shoot Process

        /// <summary> Finds a collider to damage or spawns a bullet that will find someone to damage </summary>
        protected virtual void Shoot()
        {
            if (Item.ShootType == ShootTypes.Overlap)
            {
                var count = Physics.OverlapSphereNonAlloc(ShootPoint.position, Item.Distance, colliders, Item.AttackableLayers);

                for (int i = 0; i < count; i++)
                {
                    HandleHit(colliders[i]);
                }
            }

            else if (Item.ShootType == ShootTypes.Raycast)
            {
                if (Physics.Raycast(ShootPoint.position, ShootPoint.forward, out var hit, Item.Distance, Item.AttackableLayers))
                {
                    HandleHit(hit.collider);
                }
            }

            else if (Item.ShootType == ShootTypes.Projectile)
            {
                var bullet = Instantiate(Item.Projectile, ShootPoint.position, ShootPoint.rotation);
            }
        }

        /// <summary> Handles finded in Shoot() collider </summary>
        protected virtual void HandleHit(Collider collider)
        {
            if (Penentration(collider, out var penentrations))
                Damage(collider, penentrations);
        }

        /// <summary> Checks if a bullet can penentrate from ShootPoint to the target collider </summary>
        protected virtual bool Penentration(Collider collider, out int penentrations)
        {
            penentrations = 0;

            if (Item.PenentrationType == PenentrationTypes.Infinite)
                return true;

            hits.Fill(default);
            var objects = PhysicsE.LinecastNonAlloc(ShootPoint.position, collider.transform.position, hits, Item.SolidLayers | Item.PenentrationLayers);
            penentrations = hits.Where(hit => Item.PenentrationLayers.HasLayer(hit.collider.gameObject.layer)).Count();

            if (penentrations > 0)
            {
                if (objects - penentrations > 0)
                    return false;

                if (Item.PenentrationType == PenentrationTypes.Nothing)
                    return false;

                if (Item.PenentrationType == PenentrationTypes.Specify)
                    return penentrations <= Item.PenentrationCount;
            }

            return true;
        }

        /// <summary> Takes damage and processes the resulting collider </summary>
        protected virtual void Damage(Collider collider, int penentrations)
        {
            // Hit Force
            if (collider.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(ShootPoint.forward * Item.HitForce, ForceMode.Impulse);
            }

            // Damage
            if (collider.TryGetComponent<IDamageReciever>(out var reciever))
            {
                var damage = CalculateDamage(reciever, Item.Damage, penentrations);
                var armorDamage = CalculateDamage(reciever, Item.ArmorDamage, penentrations, ignoreArmor: true);

                reciever.TakeDamage(damage, armorDamage);
            }
        }

        /// <summary> Calculates damage based on bullet and target parameters </summary>
        protected virtual float CalculateDamage(IDamageReciever reciever, float damage, int penentrations, bool ignoreArmor = false)
        {
            if (reciever.HasArmor() && !ignoreArmor)
                damage = damage.KeepPercents(Item.ArmorPenentration);

            if (penentrations > 0)
                damage = damage.LeavePercents(Item.PenentrationDamageReduction * penentrations);

            return damage;
        }

        #endregion
    }
}