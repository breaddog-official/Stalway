using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.Serialization;
using Mirror;
using Breaddog.Extensions;
using Breaddog.Network;

namespace Breaddog.Gameplay
{
    public enum UpMode
    {
        WorldUp,
        LocalUp
    }

    public enum BodyPosition : byte // For SyncVar
    {
        Stand,
        Crouch,
        Lay
    }

    public enum IgnoreMode
    {
        None, // Like in CS2 competitive mode
        OnlyStand, // Like in CS2 casual mode
        Full
    }

    public class AbillityCollisioner : Abillity
    {
        [Header("Global")]
        [OdinSerialize] public UpMode UpMode { get; private set; }

        [Header("Ground Detection")]
        [OdinSerialize] public float MaxGroundAngle { get; private set; }
        [OdinSerialize] public float MaxSlopeAngle { get; private set; }

        [Header("Body Position")]
        [OdinSerialize] public float CheckTolerance { get; private set; }
        [OdinSerialize] public LayerMask SolidMask { get; private set; }
        [OdinSerialize] public LagCompensator LagCompensator { get; private set; }
        [OdinSerialize] public Collider[] StandColliders { get; private set; }
        [OdinSerialize] public Collider[] CrouchColliders { get; private set; }
        [OdinSerialize] public Collider[] LayColliders { get; private set; }
        [Header("Ignore Collision")]
        [OdinSerialize] public IgnoreMode IgnoreMode { get; private set; }


        [field: SyncVar]
        public BodyPosition BodyPosition { get; private set; }
        public float TimeInAir { get; protected set; }
        public float HeightInAir { get; protected set; }

        protected readonly HashSet<Collider> grounds = new(4);
        protected readonly HashSet<Collider> slopes = new(4);

        protected bool onGround;
        protected float heightOnStart;

        public event Action OnGround;
        public event Action OnAir;
        public event Action OnBodyPositionChanged;

        public static readonly HashSet<Collider> ignoreColliders = new();



        public override void Init()
        {
            OnAir += WipeAir;
            OnAir += StartAirHeight;
            OnGround += StopAirHeight;

            ModifyIgnoreColliders(true);
        }

        protected virtual void OnDestroy()
        {
            ModifyIgnoreColliders(false);
        }

        #region BodyPosition

        protected void FixedUpdate()
        {
            if (!IsInit)
                return;

            UpdateColliders();
        }

        protected void UpdateColliders()
        {
            foreach (var col in StandColliders)
                col.enabled = BodyPosition == BodyPosition.Stand;

            foreach (var col in CrouchColliders)
                col.enabled = BodyPosition == BodyPosition.Crouch;

            foreach (var col in LayColliders)
                col.enabled = BodyPosition == BodyPosition.Lay;

            if (LagCompensator != null)
                LagCompensator.trackedCollider = GetColliders()[0];
        }


        [Command]
        public void TrySetBodyPosition(BodyPosition position)
        {
            if (BodyPosition == position)
                return;

            if (CanSetBodyPosition(position))
            {
                BodyPosition = position;
                OnBodyPositionChanged?.Invoke();
            }
        }

        public virtual bool CanSetBodyPosition(BodyPosition position)
        {
            if (!IsInit)
                return false;

            foreach (var col in GetColliders(position))
            {
                if (col.CheckCollider(SolidMask, QueryTriggerInteraction.Ignore, tolerance: CheckTolerance))
                    return false;
            }

            return true;
        }

        private Collider[] GetColliders(BodyPosition? position = null)
        {
            position ??= BodyPosition;
            return position switch
            {
                BodyPosition.Stand => StandColliders,
                BodyPosition.Crouch => CrouchColliders,
                BodyPosition.Lay => LayColliders,
                _ => null
            };
        }

        #endregion

        #region IsGround & IsSlope & IsAir

        protected virtual void OnCollisionStay(Collision collision)
        {
            if (!IsInit)
                return;

            HandleCollision(collision);
            UpdateAirTime();
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (!IsInit)
                return;

            RemoveCollision(collision);
        }




        protected virtual void HandleCollision(Collision collision)
        {
            bool foundedGround = false;

            for (int i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);

                if (IsSlopeNormal(contact.normal))
                {
                    slopes.Add(contact.otherCollider);
                }
                else
                {
                    slopes.Remove(contact.otherCollider);
                }

                if (!foundedGround)
                {
                    if (IsGroundNormal(contact.normal))
                    {
                        // We don't call Contains because Add and Remove already checks for this
                        grounds.Add(contact.otherCollider);

                        SetOnGround(true);

                        foundedGround = true;
                    }
                    else
                    {
                        grounds.Remove(contact.otherCollider);

                        if (grounds.Count == 0)
                            SetOnGround(false);
                    }
                }
            }
        }

        protected virtual void RemoveCollision(Collision collision)
        {
            // We don't call Contains because Remove already checks for this
            grounds.Remove(collision.collider);
            slopes.Remove(collision.collider);

            if (grounds.Count == 0)
                SetOnGround(false);
        }



        private void SetOnGround(bool onGround)
        {
            if (this.onGround && !onGround)
                OnAir?.Invoke();

            if (!this.onGround && onGround)
                OnGround?.Invoke();

            this.onGround = onGround;
        }


        public bool IsGround() => onGround;
        public bool IsSlope() => !onGround && slopes.Count > 0;
        public bool IsAir() => !onGround;


        public bool IsGroundNormal(Vector3 normal)
        {
            var angle = Vector3.Angle(GetUp(), normal);
            return angle <= MaxGroundAngle;
        }

        public bool IsSlopeNormal(Vector3 normal)
        {
            var angle = Vector3.Angle(GetUp(), normal);
            return angle > MaxGroundAngle && angle <= MaxSlopeAngle;
        }

        #endregion

        #region AirTime & AirHeight

        private void UpdateAirTime()
        {
            if (IsAir())
                TimeInAir += Time.fixedDeltaTime;
        }

        private void StartAirHeight()
        {
            heightOnStart = transform.position.y;
        }

        private void StopAirHeight()
        {
            HeightInAir = transform.position.y - heightOnStart;
        }

        private void WipeAir()
        {
            TimeInAir = 0f;
            HeightInAir = 0f;
        }

        #endregion

        #region IgnoreCollision

        public void ModifyIgnoreColliders(bool add)
        {
            if (IgnoreMode == IgnoreMode.None)
                return;

            else if (IgnoreMode == IgnoreMode.OnlyStand)
            {
                ModifyColliders(StandColliders);
            }

            else if (IgnoreMode == IgnoreMode.Full)
            {

                ModifyColliders(StandColliders);
                ModifyColliders(CrouchColliders);
                ModifyColliders(LayColliders);
            }

            void ModifyColliders(ICollection<Collider> colliders)
            {
                foreach (var col1 in colliders)
                {
                    if (!add)
                        ignoreColliders.Remove(col1);

                    foreach (var col2 in ignoreColliders)
                    {
                        Physics.IgnoreCollision(col1, col2, add);
                    }

                    if (add)
                        ignoreColliders.Add(col1);
                }
            }
        }

        #endregion


        public Vector3 GetUp()
        {
            return UpMode switch
            {
                UpMode.WorldUp => Vector3.up,
                UpMode.LocalUp => transform.up,
                _ => throw new NotImplementedException()
            };
        }


        private void OnDrawGizmosSelected()
        {
            Vector3 rotatedDirection = Quaternion.AngleAxis(90f - MaxGroundAngle, Vector3.right) * -GetUp();

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, rotatedDirection);

            foreach (var col in GetColliders(BodyPosition))
            {
                col.GizmosCollider(tolerance: CheckTolerance);
            }
        }
    }
}
