using Breaddog.Extensions;
using Mirror;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public enum AirMoveMode
    {
        Normal,
        Multiplied,
        None
    }

    public class AbillityMovement : Abillity
    {
        [Header("Links")]
        public PhysicsMaterial StayMaterial;
        public PhysicsMaterial MoveMaterial;
        public Transform Head;

        [Header("Network")]
        public MoveAuthority Authority;
        [ShowIf(nameof(PredictionOrHybryd))] public PredictedRigidbody PredictedRb;
        [HideIf(nameof(PredictionOrHybryd))] public Rigidbody CommonRb;
        [HideIf(nameof(Authority), MoveAuthority.Prediction)] public NetworkTransformHybrid TransformSync;
        [ShowIf(nameof(Authority), MoveAuthority.Hybrid)] public bool ClientSyncPosition;
        [ShowIf(nameof(Authority), MoveAuthority.Hybrid)] public bool ClientSyncRotation;

        [Header("Network: Compression")]
        public bool Compress = true;
        [ShowIf(nameof(Compress))] public Vector2 MovePrecision = new(0.01f, 0.01f);
        [ShowIf(nameof(Compress))] public Vector2 LookPrecision = new(0.01f, 0.01f);
        [ShowIf(nameof(Compress))] public Vector3 VelocityPrecision = new(0.01f, 0.01f, 0.01f);
        [ShowIf(nameof(Compress))] public Vector3 AngularVelocityPrecision = new(0.01f, 0.01f, 0.01f);

        [Header("Move")]
        public Vector2 MinMoveInput = new(0.01f, 0.01f);
        public Vector2 MinLookInput = new(0.01f, 0.01f);
        [Space]
        public AirMoveMode AirMove;
        [Range(0f, 1f), ShowIf(nameof(AirMove), AirMoveMode.Multiplied)] public float AirSpeedMultiplier = 0.1f;
        [Range(0f, 1f), ShowIf(nameof(AirMove), AirMoveMode.Multiplied)] public float SlopeSpeedMultiplier = 0.1f;


        [Header("Forces")]
        public float AccelerationSpeed = 1f;
        public float GroundGravityForce = 1f;
        public float AirGravityForce = 5f;
        public float JumpForce = 5f;


        [Header("Speed Limits")]
        public float MaxGroundSpeed = 5f;
        public float MaxAirSpeed = 5f;

        [ShowNativeProperty]
        public float Speed => Rigidbody != null && Rigidbody.linearVelocity.magnitude > 0.01f ? Rigidbody.linearVelocity.magnitude : 0f;


        [Header("Speed Limits: Multipliers")]
        [Range(0f, 2f)] public float StandMultiplier = 1f;
        [Range(0f, 2f)] public float CrouchMultiplier = 0.75f;
        [Range(0f, 2f)] public float LayMultiplier = 0.5f;


        [Header("Look")]
        [Range(0f, 100f)] public float LookSensitivity = 10f;
        [Range(0f, 90f)] public float MinLookAngle = 90f;
        [Range(0f, 90f)] public float MaxLookAngle = 90f;

        private bool PredictionOrHybryd => Authority == MoveAuthority.Prediction || Authority == MoveAuthority.Hybrid;

        public Vector2 MinMove => Vector2.Max(MinMoveInput, Compress ? MovePrecision : Vector2.zero);
        public Vector2 MinLook => Vector2.Max(MinLookInput, Compress ? LookPrecision : Vector2.zero);

        private Vector3 syncVelocity;
        private Vector3 syncAngularVelocity;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpInput;

        private double lastMoveTime = -1;
        private bool lastMoveMode;
        private Vector3 lastCalculatedVector;
        private float headRotation;
        private PhysicsMaterial physicsMaterial;

        private AbillityCollisioner collisioner;

        public Rigidbody Rigidbody => Authority == MoveAuthority.Prediction ? PredictedRb?.predictedRigidbody : CommonRb;

        public event Action OnMove;
        public event Action OnLook;
        public event Action OnJump;


        public override void Init()
        {
            headRotation = Head.localEulerAngles.x;
            collisioner = Entity.FindAbillity<AbillityCollisioner>();

            physicsMaterial = new($"{gameObject.name}_physicsMaterial")
            {
                staticFriction = StayMaterial.staticFriction,
                dynamicFriction = StayMaterial.dynamicFriction,
                frictionCombine = StayMaterial.frictionCombine,
                bounciness = StayMaterial.bounciness,
                bounceCombine = StayMaterial.bounceCombine
            };


            foreach (var col in collisioner.StandColliders)
                col.sharedMaterial = physicsMaterial;

            foreach (var col in collisioner.CrouchColliders)
                col.sharedMaterial = physicsMaterial;

            foreach (var col in collisioner.LayColliders)
                col.sharedMaterial = physicsMaterial;


            if (Authority == MoveAuthority.Hybrid)
            {
                PredictedRb.syncPosition = !ClientSyncPosition;
                PredictedRb.syncRotation = !ClientSyncRotation;

                TransformSync.syncPosition = ClientSyncPosition;
                TransformSync.syncRotation = ClientSyncRotation;
            }
        }


        private void OnDestroy()
        {
            Destroy(physicsMaterial);
        }


        private void FixedUpdate()
        {
            // For velocity sync
            SetDirty();

            if (AuthorityCorrect())
            {
                if (IsSyncRotation())
                {
                    ApplyLook();
                }

                if (IsSyncPosition())
                {
                    UpdateFriction();
                    Gravity();

                    ApplyMove();
                    ApplyJump();
                }
            }
        }



        #region Move

        public void SetMove(Vector2 input)
        {
            moveInput = input;

            if (Authority != MoveAuthority.ClientAuthority && moveInput.Abs().GreaterThen(MinMove))
                SetDirty();
        }

        private void ApplyMove()
        {
            // Skip move if input too small
            if (moveInput.Abs().LowerThen(MinMove))
                return;

            // Skip move if moving in air disabled
            if (AirMove == AirMoveMode.None && collisioner.IsAir())
                return;

            // Calculate move vector
            Vector3 calculatedVector = moveInput.Flatten().ClampMagnitude();
            Vector3 targetVector = calculatedVector * GetMaxSpeed();
            calculatedVector = (targetVector - Rigidbody.linearVelocity.Flatten()) * AccelerationSpeed;

            // Apply multiply if needed
            if (AirMove == AirMoveMode.Multiplied && collisioner.IsAir())
                calculatedVector *= collisioner.IsSlope() ? SlopeSpeedMultiplier : AirSpeedMultiplier;

            // Apply movement
            Rigidbody.AddForce(calculatedVector, ForceMode.VelocityChange);

            OnMove?.Invoke();
        }

        #endregion

        #region Look

        public void SetLook(Vector2 input)
        {
            lookInput = input;

            if (Authority != MoveAuthority.ClientAuthority && lookInput.Abs().GreaterThen(MinLook))
                SetDirty();
        }

        public void ApplyLook()
        {
            // Skip look if input too small
            if (lookInput.Abs().LowerThen(MinLook))
                return;

            var input = LookSensitivity * Time.fixedDeltaTime * lookInput;

            var bodyRotation = Quaternion.AngleAxis(Rigidbody.rotation.eulerAngles.y + input.x, GetUp());

            headRotation -= input.y;
            headRotation = Mathf.Clamp(headRotation, -MaxLookAngle, MinLookAngle);

            Head.localEulerAngles = new(headRotation, 0f, 0f);
            Rigidbody.MoveRotation(bodyRotation);

            OnLook?.Invoke();
        }

        #endregion

        #region Jump

        public void SetJump(bool input)
        {
            var changed = jumpInput != input;

            jumpInput = input;

            if (changed && Authority != MoveAuthority.ClientAuthority && IsSyncPosition())
                SyncJump(input);
        }

        [Command] // We use command instead serialization, because jumpInput can be true for only one frame, and must be sent in reliable ordered packet
        public void SyncJump(bool jump) => jumpInput = jump;
        public bool CanJump() => collisioner.IsGround();

        private void ApplyJump()
        {
            if (jumpInput == false)
                return;

            if (!CanJump())
                return;

            Vector3 velocity = GetUp() * JumpForce;
            Rigidbody.AddForce(velocity, ForceMode.VelocityChange);

            OnJump?.Invoke();
        }

        #endregion

        #region Additional Forces

        /// <summary>
        /// We add our gravity, because default gravity does not depend on mass
        /// </summary>
        private void Gravity()
        {
            // Predict jump, because GroundGravity can be much stronger than AirGravity, and this can affect the force of the jump
            var willJump = jumpInput && CanJump();

            var force = collisioner.IsAir() || willJump ? AirGravityForce : GroundGravityForce;
            var forceVector = -GetUp() * force * Rigidbody.mass * Time.fixedDeltaTime;

            Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
        }

        private void UpdateFriction()
        {
            var moveFactor = Mathf.InverseLerp(0f, MinMove.sqrMagnitude, moveInput.sqrMagnitude);

            physicsMaterial.dynamicFriction = Mathf.Lerp(StayMaterial.dynamicFriction, MoveMaterial.dynamicFriction, moveFactor);
            physicsMaterial.staticFriction = Mathf.Lerp(StayMaterial.staticFriction, MoveMaterial.staticFriction, moveFactor);
            physicsMaterial.bounciness = Mathf.Lerp(StayMaterial.bounciness, MoveMaterial.bounciness, moveFactor);
            physicsMaterial.frictionCombine = moveFactor < 0.5f ? StayMaterial.frictionCombine : MoveMaterial.frictionCombine;
            physicsMaterial.bounceCombine = moveFactor < 0.5f ? StayMaterial.bounceCombine : MoveMaterial.bounceCombine;
        }

        #endregion

        #region Serialization

        public override void OnSerialize(NetworkWriter writer, bool initialState)
        {
            base.OnSerialize(writer, initialState);

            if (initialState || !Compress)
            {
                if (Authority != MoveAuthority.ClientAuthority)
                {

                    if (IsSyncPosition())
                    {
                        writer.WriteVector2(moveInput);
                    }

                    if (IsSyncRotation())
                    {
                        writer.WriteVector2(lookInput);
                    }
                }

                writer.WriteVector3(Rigidbody.linearVelocity);
                writer.WriteVector3(Rigidbody.angularVelocity);
            }
            else
            {
                if (Authority != MoveAuthority.ClientAuthority)
                {
                    if (IsSyncPosition())
                    {
                        NetworkE.WriteAndCompressVector2(writer, moveInput, MovePrecision);
                    }

                    if (IsSyncRotation())
                    {
                        NetworkE.WriteAndCompressVector2(writer, lookInput, LookPrecision);
                    }
                }

                NetworkE.WriteAndCompressVector3(writer, Rigidbody.linearVelocity, VelocityPrecision);
                NetworkE.WriteAndCompressVector3(writer, Rigidbody.angularVelocity, AngularVelocityPrecision);
            }
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);

            if (initialState || !Compress)
            {
                if (Authority != MoveAuthority.ClientAuthority)
                {
                    if (IsSyncPosition())
                    {
                        moveInput = reader.ReadVector2();
                    }

                    if (IsSyncRotation())
                    {
                        lookInput = reader.ReadVector2();
                    }
                }

                syncVelocity = reader.ReadVector3();
                syncAngularVelocity = reader.ReadVector3();
            }
            else
            {
                if (Authority != MoveAuthority.ClientAuthority)
                {
                    if (IsSyncPosition())
                    {
                        moveInput = NetworkE.ReadCompressedVector2(reader, MovePrecision);
                    }

                    if (IsSyncRotation())
                    {
                        lookInput = NetworkE.ReadCompressedVector2(reader, LookPrecision);
                    }
                }

                syncVelocity = NetworkE.ReadCompressedVector3(reader, VelocityPrecision);
                syncAngularVelocity = NetworkE.ReadCompressedVector3(reader, AngularVelocityPrecision);
            }

        }

        #endregion



        #region Move Multipling

        public float GetBodyPositionSpeed(BodyPosition position)
        {
            return position switch
            {
                BodyPosition.Stand => StandMultiplier,
                BodyPosition.Crouch => CrouchMultiplier,
                BodyPosition.Lay => LayMultiplier,
                _ => 1f,
            };
        }

        #endregion

        public float GetMaxSpeedMultiplier() => GetBodyPositionSpeed(collisioner.BodyPosition);
        public float GetMaxSpeed() => GetMaxSpeedMultiplier() * (collisioner.IsGround() ? MaxGroundSpeed : MaxAirSpeed);
        protected Vector3 GetUp() => collisioner.GetUp();

        /// <summary> Returns the velocity depending on the current authority mode </summary>
        public Vector3 GetVelocity()
        {
            if (!Application.isPlaying)
                return Vector3.zero;

            if (!AuthorityCorrect())
                return syncVelocity;
            else
                return Rigidbody.linearVelocity;
        }

        /// <summary> Returns the angularVelocity depending on the current authority mode </summary>
        public Vector3 GetAngularVelocity()
        {
            if (!Application.isPlaying)
                return Vector3.zero;

            if (!AuthorityCorrect())
                return syncAngularVelocity;
            else
                return Rigidbody.angularVelocity;
        }



        public bool AuthorityCorrect()
        {
            return Authority switch
            {
                MoveAuthority.ClientAuthority => isClient && isOwned,
                MoveAuthority.ServerAuthority => isServer,
                _ => isServer || isOwned
            };
        }

        // Server authority already checked in AuthorityCorrect()
        public bool IsSyncPosition() => Authority != MoveAuthority.Hybrid || !ClientSyncPosition || isClient;
        public bool IsSyncRotation() => Authority != MoveAuthority.Hybrid || !ClientSyncRotation || isClient;
    }
}
