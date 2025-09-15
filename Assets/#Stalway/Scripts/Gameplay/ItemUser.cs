using Mirror;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public abstract class ItemUser : NetworkBehaviour
    {
        /// <summary> Starts using </summary>
        public abstract void StartUsing();

        /// <summary> Stops using </summary>
        public abstract void StopUsing();

        /// <summary> Same as StopUsing, but do it immedeatly </summary>
        public virtual void CancelUsing() => StopUsing();
    }
}