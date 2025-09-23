using Breaddog.Input;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breaddog.UI
{
    public class GameUI : MonoBehaviour
    {
        public GameObject inventory;

        private void OnEnable()
        {
            InputManager.ControlsGame.Inventory.performed += ToggleInventory;
            InputManager.ControlsUI.Debug.performed += ToggleDebug;
            InputManager.ControlsUI.Quit.performed += ToggleInventory;
        }

        private void OnDisable()
        {
            InputManager.ControlsGame.Inventory.performed -= ToggleInventory;
            InputManager.ControlsUI.Debug.performed -= ToggleDebug;
            InputManager.ControlsUI.Quit.performed -= ToggleInventory;
        }

        private void ToggleInventory(InputAction.CallbackContext ctx = default)
        {
            if (inventory.activeSelf)
            {
                inventory.SetActive(false);
                InputManager.SwitchGame();
            }

            else
            {
                inventory.SetActive(true);
                InputManager.SwitchUI();
            }
        }

        private void ToggleDebug(InputAction.CallbackContext ctx = default)
        {
            if (inventory.TryGetComponent(out StorageDrawer drawer))
                drawer.AddTestItem();
        }
    }
}