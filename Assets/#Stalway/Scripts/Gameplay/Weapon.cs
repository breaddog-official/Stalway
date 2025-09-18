using Breaddog.Gameplay.StorageManagement;

namespace Breaddog.Gameplay
{
    public abstract class Weapon : ItemUser
    {
        public ItemWeapon Item;


        public override void StartUsing() => StartFire();
        public override void StopUsing() => StopFire();
        public override void CancelUsing() => CancelFire();


        /// <summary> Starts shooting or drawing the bowstring for example </summary>
        public abstract void StartFire();

        /// <summary> Stops shooting or releases an arrow for example </summary>
        public abstract void StopFire();

        /// <summary> Same as StopFire, but instead of releasing an arrow it can return the bowstring for example </summary>
        public virtual void CancelFire() => StopFire();
    }
}