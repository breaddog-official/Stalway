using UnityEngine;
using Sirenix.Serialization;
using Mirror;

namespace Breaddog.Gameplay
{
    public class AbillityFlashlight : Abillity
    {
        [field: SyncVar(hook = nameof(UpdateFlashlight))]
        [OdinSerialize] public bool Enabled { get; private set; }
        [OdinSerialize] public GameObject Flashlight { get; private set; }

        [Command]
        public void Toggle()
        {
            Enabled = !Enabled;
            UpdateFlashlight();
        }

        private void UpdateFlashlight(bool oldValue = false, bool newValue = true)
        {
            Flashlight.SetActive(Enabled);
        }
    }
}
