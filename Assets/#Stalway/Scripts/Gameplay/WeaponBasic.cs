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
            if (Item.WeaponProperties.AutomaticType == AutomaticTypes.Automatic || Item.WeaponProperties.AutomaticType == AutomaticTypes.Burst)
            {
                delay += Time.deltaTime;

                if (isShooting && delay >= Item.WeaponProperties.Firerate)
                {
                    delay = 0f;
                    Fire();

                    if (Item.WeaponProperties.AutomaticType == AutomaticTypes.SemiAuto)
                    {
                        isShooting = false;
                    }

                    if (Item.WeaponProperties.AutomaticType == AutomaticTypes.Burst)
                    {
                        bulletsInBurst++;
                        if (bulletsInBurst > Item.WeaponProperties.ShotsPerBurst)
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
            if (Item.WeaponProperties.AutomaticType == AutomaticTypes.Automatic)
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
            if (Item.WeaponProperties.ReloadType != ReloadTypes.None && ammo == 0)
                return false;

            return true;
        }

        #region Shoot Process

        /// <summary> Finds a collider to damage or spawns a bullet that will find someone to damage </summary>
        protected virtual void Shoot()
        {
            if (Item.WeaponProperties.ShootType == ShootTypes.Overlap)
            {
                var count = Physics.OverlapSphereNonAlloc(ShootPoint.position, Item.WeaponProperties.Distance, colliders, Item.WeaponProperties.AttackableLayers);

                for (int i = 0; i < count; i++)
                {
                    Damage(colliders[i]);
                }
            }

            else if (Item.WeaponProperties.ShootType == ShootTypes.Projectile)
            {
                var bullet = Instantiate(Item.WeaponProperties.Projectile, ShootPoint.position, ShootPoint.rotation);
            }
        }

        /// <summary> Takes damage and processes the resulting collider </summary>
        protected virtual void Damage(Collider collider)
        {
            // Hit Force
            if (collider.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(ShootPoint.forward * Item.WeaponProperties.HitForce, ForceMode.Impulse);
            }

            // Damage
            if (collider.TryGetComponent<IDamageReciever>(out var reciever))
            {
                var damage = CalculateDamage(reciever, Item.WeaponProperties.Damage);
                var armorDamage = CalculateDamage(reciever, Item.WeaponProperties.ArmorDamage, ignoreArmor: true);

                reciever.TakeDamage(damage, armorDamage);
            }
        }

        /// <summary> Calculates damage based on bullet and target parameters </summary>
        protected virtual float CalculateDamage(IDamageReciever reciever, float damage, bool ignoreArmor = false)
        {
            if (reciever.HasArmor() && !ignoreArmor)
                damage = damage.KeepPercents(Item.WeaponProperties.ArmorPenentration);

            return damage;
        }

        #endregion
    }
}