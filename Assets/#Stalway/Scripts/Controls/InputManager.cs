using UnityEngine;

namespace Breaddog.Input 
{
    public static class InputManager
    {
        public static readonly Controls Controls;
        public static Controls.GameActions ControlsGame => Controls.Game;
        public static Controls.UIActions ControlsUI => Controls.UI;

        static InputManager()
        {
            Controls = new();
            Controls.Enable();

            // First always be UI
            // SwitchUI();

            // While debug
            SwitchGame();
        }

        public static void SwitchGame()
        {
            ControlsGame.Enable();
            ControlsUI.Disable();
        }

        public static void SwitchUI()
        {
            ControlsGame.Disable();
            ControlsUI.Enable();
        }
    }
}