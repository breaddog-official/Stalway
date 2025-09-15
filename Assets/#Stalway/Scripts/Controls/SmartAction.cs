using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breaddog.Input
{
    public class SmartAction<TValue> : IDisposable where TValue : struct
    {
        public InputAction InputAction;
        public Action<TValue> Action;

        protected bool onlyOnChange = true;

        protected bool toggle;
        protected bool locked;
        protected bool onlyNonLockedValue;

        protected TValue? onlyWhenValueIs;
        protected TValue? lockedValue;

        protected Func<bool, TValue> onlyWhenValue;
        protected Func<bool> onlyWhen;

        protected TValue lastValue;


        public SmartAction(InputAction inputAction, Action<TValue> action, bool setLastValue = true)
        {
            InputAction = inputAction;
            Action = action;

            if (setLastValue)
                lastValue = ReadValue();
        }


        public virtual void Update()
        {
            TValue currentValue = ReadValue();

            if (locked && lockedValue == null)
                return;

            if (onlyOnChange && currentValue.Equals(lastValue))
                return;

            if (onlyWhenValueIs.HasValue && !currentValue.Equals(onlyWhenValueIs.Value))
                return;

            if (onlyWhen != null && !onlyWhen.Invoke())
                return;

            lastValue = locked ? lockedValue.Value : currentValue;
            Action?.Invoke(lastValue);

            if (toggle && !locked && (onlyNonLockedValue ? !lastValue.Equals(lockedValue.Value) : true))
                Lock();
        }

        public virtual TValue ReadValue()
        {
            return InputAction.ReadValue<TValue>();
        }


        protected virtual void Lock(InputAction.CallbackContext ctx = default)
        {
            locked = true;
        }

        protected virtual void Unlock(InputAction.CallbackContext ctx = default)
        {
            locked = false;
        }

        public virtual void SilentUnlock()
        {
            locked = false;
            lastValue = ReadValue();
        }

        public virtual SmartAction<TValue> AlwaysUpdate()
        {
            onlyOnChange = false;
            return this;
        }

        public virtual SmartAction<TValue> OnlyWhenValueIs(TValue value)
        {
            onlyWhenValueIs = value;
            return this;
        }

        public virtual SmartAction<TValue> OnlyWhenValue(Func<bool, TValue> func)
        {
            onlyWhenValue = func;
            return this;
        }

        public virtual SmartAction<TValue> OnlyWhen(Func<bool> func)
        {
            onlyWhen = func;
            return this;
        }

        /// <summary>
        /// Если все условия были выполнены, то действие выполнится и встанет в блокировку до тех пор пока пользователь не нажмёт кнопку ещё раз <br />
        /// Если lockedValue не равно нулю, то во время блокировки будет выполняться действие с lockedValue
        /// </summary>
        public virtual SmartAction<TValue> Toggle(TValue? lockedValue = null, bool onlyNonLockedValue = false)
        {
            this.toggle = true;
            this.onlyNonLockedValue = onlyNonLockedValue;
            this.lockedValue = lockedValue;
            InputAction.started += Unlock;
            return this;
        }


        public void Dispose()
        {
            if (InputAction != null && toggle)
                InputAction.started -= Unlock;
        }
    }

    public class SmartActionBool : SmartAction<bool>
    {
        public SmartActionBool(InputAction inputAction, Action<bool> action, bool setLastValue = true) : base(inputAction, action, setLastValue)
        {

        }


        public override bool ReadValue()
        {
            return InputAction.IsPressed();
        }


        public new SmartActionBool AlwaysUpdate()
        {
            base.AlwaysUpdate();
            return this;
        }

        public new SmartActionBool OnlyWhenValueIs(bool value)
        {
            base.OnlyWhenValueIs(value);
            return this;
        }

        public new SmartActionBool OnlyWhenValue(Func<bool, bool> func)
        {
            base.OnlyWhenValue(func);
            return this;
        }

        public new SmartActionBool OnlyWhen(Func<bool> func)
        {
            base.OnlyWhen(func);
            return this;
        }

        /// <summary>
        /// Если все условия были выполнены, то действие выполнится и встанет в блокировку до тех пор пока пользователь не нажмёт кнопку ещё раз <br />
        /// Если lockedValue не равно нулю, то во время блокировки будет выполняться действие с lockedValue
        /// </summary>
        public new SmartActionBool Toggle(bool? lockedValue = null, bool onlyNonLockedValue = false)
        {
            base.Toggle(lockedValue, onlyNonLockedValue);
            return this;
        }
    }
}