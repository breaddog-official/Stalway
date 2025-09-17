using Mirror;
using Breaddog.Input;
using UnityEngine;
using Breaddog.Extensions;
using UnityEngine.InputSystem;

namespace Breaddog.Gameplay
{

    public class ControllerPlayer : Controller
    {
        private AbillityMovement abillityMovement;
        private AbillityCollisioner abillityCollisioner;
        private AbillityInventory abillityInventory;
        private AbillityFlashlight abillityFlashlight;

        private SmartAction<Vector2> moveAction;
        private SmartAction<Vector2> lookAction;
        private SmartActionBool jumpAction;
        private SmartActionBool walkAction;
        private SmartActionBool crouchAction;
        private SmartActionBool layAction;
        private SmartActionBool shootAction;
        private SmartActionBool flashlightAction;

        private bool walk;


        public override void Init()
        {
            Cursor.lockState = CursorLockMode.Confined;

            abillityMovement = Entity.FindAbillity<AbillityMovement>();
            abillityCollisioner = Entity.FindAbillity<AbillityCollisioner>();
            abillityInventory = Entity.FindAbillity<AbillityInventory>();
            abillityFlashlight = Entity.FindAbillity<AbillityFlashlight>();

            moveAction = new SmartAction<Vector2>(InputManager.ControlsGame.Move, SetMove).AlwaysUpdate();
            lookAction = new SmartAction<Vector2>(InputManager.ControlsGame.Look, abillityMovement.SetLook).AlwaysUpdate();
            jumpAction = new SmartActionBool(InputManager.ControlsGame.Jump, abillityMovement.SetJump).AlwaysUpdate().Toggle(lockedValue: false, onlyNonLockedValue: true);
            walkAction = new SmartActionBool(InputManager.ControlsGame.Walk, SetWalk);
            crouchAction = new SmartActionBool(InputManager.ControlsGame.Crouch, SetCrouch).Toggle();
            layAction = new SmartActionBool(InputManager.ControlsGame.Lay, SetLay).Toggle();
            shootAction = new SmartActionBool(InputManager.ControlsGame.Fire, SetShoot);
            flashlightAction = new SmartActionBool(InputManager.ControlsGame.Flashlight, SetFlashlight).Toggle();
        }

        private void OnDestroy()
        {
            moveAction.Dispose();
            lookAction.Dispose();
            jumpAction.Dispose();
            walkAction.Dispose();
            crouchAction.Dispose();
            layAction.Dispose();
            shootAction.Dispose();
            flashlightAction.Dispose();
        }





        public void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            moveAction.Update();
            lookAction.Update();
            jumpAction.Update();
            walkAction.Update();
            crouchAction.Update();
            layAction.Update();
            shootAction.Update();
            flashlightAction.Update();
        }





        private void SetMove(Vector2 input)
        {
            Vector3 relativeVector = transform.TransformDirection(input.Flatten());
            var speed = walk ? 0.25f : 1f; // In future, get in audio manager
            var calculatedMoveInput = Vector2.ClampMagnitude(new Vector2(relativeVector.x, relativeVector.z), speed);
            abillityMovement.SetMove(calculatedMoveInput);
        }


        private void SetCrouch(bool crouch)
        {
            var state = crouch ? BodyPosition.Crouch : BodyPosition.Stand;
            if (abillityCollisioner.CanSetBodyPosition(state))
            {
                abillityCollisioner.TrySetBodyPosition(state);
                layAction.SilentUnlock();
            }
        }

        private void SetLay(bool lay)
        {
            var state = lay ? BodyPosition.Lay : BodyPosition.Stand;
            if (abillityCollisioner.CanSetBodyPosition(state))
            {
                abillityCollisioner.TrySetBodyPosition(state);
                crouchAction.SilentUnlock();
            }
        }

        private void SetShoot(bool shoot)
        {
            //if (abillityInventory.SpawnedItem != null)
            //{
            //    if (shoot)
            //        abillityInventory.SpawnedItem.StartUsing();
            //
            //    else
            //        abillityInventory.SpawnedItem.StopUsing();
            //}
        }

        private void SetWalk(bool walk)
        {
            this.walk = walk;
        }

        private void SetFlashlight(bool flashlight)
        {
            abillityFlashlight.Toggle();
        }
    }
}
