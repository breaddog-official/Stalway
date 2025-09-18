using UnityEngine;
using Mirror;

namespace Breaddog.Gameplay
{
    public class AbillityFlashlight : Abillity
    {
        [SyncVar(hook = nameof(UpdateFlashlight))]
        public bool Enabled;
        public GameObject Flashlight;

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
