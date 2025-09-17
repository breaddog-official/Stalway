using Breaddog.Extensions;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Runtime.CompilerServices;
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
        [OdinSerialize] public PhysicsMaterial StayMaterial { get; protected set; }
        [OdinSerialize] public PhysicsMaterial MoveMaterial { get; protected set; }
        [OdinSerialize] public Transform Head { get; protected set; }
        [Header("Network")]
        [OdinSerialize] public MoveAuthority Authority { get; protected set; }
        [OdinSerialize, ShowIf("@this.Authority == MoveAuthority.Prediction || this.Authority == MoveAuthority.Hybrid")] public PredictedRigidbody PredictedRb { get; protected set; }
        [OdinSerialize, HideIf("@this.Authority == MoveAuthority.Prediction || this.Authority == MoveAuthority.Hybrid")] public Rigidbody CommonRb { get; protected set; }
        [OdinSerialize, HideIf(nameof(Authority), MoveAuthority.Prediction)] public NetworkTransformHybrid TransformSync { get; protected set; }
        [OdinSerialize, ShowIf(nameof(Authority), MoveAuthority.Hybrid)] public bool ClientSyncPosition { get; protected set; }
        [OdinSerialize, ShowIf(nameof(Authority), MoveAuthority.Hybrid)] public bool ClientSyncRotation { get; protected set; }

        [Header("Network: Compression")]
        [OdinSerialize] public bool Compress { get; protected set; } = true;
        [OdinSerialize, ShowIf(nameof(Compress))] public Vector2 MovePrecision { get; protected set; } = new(0.01f, 0.01f);
        [OdinSerialize, ShowIf(nameof(Compress))] public Vector2 LookPrecision { get; protected set; } = new(0.01f, 0.01f);
        [OdinSerialize, ShowIf(nameof(Compress))] public Vector3 VelocityPrecision { get; protected set; } = new(0.01f, 0.01f, 0.01f);
        [OdinSerialize, ShowIf(nameof(Compress))] public Vector3 AngularVelocityPrecision { get; protected set; } = new(0.01f, 0.01f, 0.01f);

        [Header("Move")]
        [OdinSerialize] public Vector2 MinMoveInput { get; protected set; } = new(0.01f, 0.01f);
        [OdinSerialize] public Vector2 MinLookInput { get; protected set; } = new(0.01f, 0.01f);
        [PropertySpace]
        [OdinSerialize] public AirMoveMode AirMove { get; protected set; }
        [PropertyRange(0f, 1f), ShowIf(nameof(AirMove), AirMoveMode.Multiplied)]
        [OdinSerialize] public float AirSpeedMultiplier { get; protected set; } = 0.1f;
        [PropertyRange(0f, 1f), ShowIf(nameof(AirMove), AirMoveMode.Multiplied)]
        [OdinSerialize] public float SlopeSpeedMultiplier { get; protected set; } = 0.1f;


        [Header("Forces")]
        [OdinSerialize] public float AccelerationForce { get; protected set; } = 10f;
        [OdinSerialize] public float AccelerationSmooth { get; protected set; } = 10f;
        [OdinSerialize] public float GroundGravityForce { get; protected set; } = 1f;
        [OdinSerialize] public float AirGravityForce { get; protected set; } = 5f;
        [OdinSerialize] public float JumpForce { get; protected set; } = 5f;


        [Header("Speed Limits")]
        [OdinSerialize, Unit(Units.MetersPerSecond)] public float MaxGroundSpeed { get; protected set; } = 5f;
        [OdinSerialize, Unit(Units.MetersPerSecond)] public float MaxAirSpeed { get; protected set; } = 5f;

        [Sirenix.OdinInspector.ShowInInspector, Unit(Units.MetersPerSecond, "UnitsPerSecond", DisplayAsString = true, ForceDisplayUnit = true)]
        public float Speed => GetVelocity().magnitude; // Units per second like in CS2

        [Header("Speed Limits: Multipliers")]
        [OdinSerialize, PropertyRange(0f, 2f)] public float StandMultiplier { get; protected set; } = 1f;
        [OdinSerialize, PropertyRange(0f, 2f)] public float CrouchMultiplier { get; protected set; } = 0.75f;
        [OdinSerialize, PropertyRange(0f, 2f)] public float LayMultiplier { get; protected set; } = 0.5f;


        [Header("Look")]
        [OdinSerialize, PropertyRange(0f, 100f)] public float LookSensitivity { get; protected set; } = 10f;
        [OdinSerialize, PropertyRange(0f, 90f), Unit(Units.Degree)] public float MinLookAngle { get; protected set; } = 90f;
        [OdinSerialize, PropertyRange(0f, 90f), Unit(Units.Degree)] public float MaxLookAngle { get; protected set; } = 90f;

        public Vector2 MinMove => Vector2.Max(MinMoveInput, Compress ? MovePrecision : Vector2.zero);
        public Vector2 MinLook => Vector2.Max(MinLookInput, Compress ? LookPrecision : Vector2.zero);

        private Vector3 syncVelocity;
        private Vector3 syncAngularVelocity;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpInput;

        private double lastMoveTime = -1;
        private bool lastMoveMode;
        private Vector2 lastCalculatedVector;
        private float headRotation;
        private PhysicsMaterial physicsMaterial;

        private AbillityCollisioner collisioner;

        public Rigidbody Rigidbody => Authority == MoveAuthority.Prediction ? PredictedRb.predictedRigidbody : CommonRb;

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

                    LimitSpeed();
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

            if (moveInput.Abs().LowerThen(MinMove) && (lastMoveTime < 0 || lastMoveMode == true))
            {
                lastMoveTime = NetworkTime.time;
                lastMoveMode = false;
            }

            else if (moveInput.Abs().GreaterThen(MinMove) && (lastMoveTime < 0 || lastMoveMode == false))
            {
                lastMoveTime = NetworkTime.time;
                lastMoveMode = true;
            }


            var t = (float)MathE.InverseLerp(lastMoveTime, lastMoveTime + 1d, NetworkTime.time);

            // Calculate move vector
            Vector3 calculatedVector = moveInput.Flatten().ClampMagnitude();
            calculatedVector *= AccelerationForce;
            calculatedVector *= Time.fixedDeltaTime;

            // Apply multiply if needed
            if (AirMove == AirMoveMode.Multiplied)
                calculatedVector *= collisioner.IsSlope() ? SlopeSpeedMultiplier : AirSpeedMultiplier;

            //calculatedVector = Vector3.Lerp(lastMoveMode ? Vector3.zero : Vector3.one, calculatedVector, t);
            //lastCalculatedVector = calculatedVector;

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
            // Predict jump, because GroundGravity is usually much stronger than AirGravity, and this can affect the force of the jump
            var willJump = jumpInput && CanJump();

            var force = collisioner.IsAir() || willJump ? AirGravityForce : GroundGravityForce;
            var forceVector = -GetUp() * force * Rigidbody.mass * Time.fixedDeltaTime;

            Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
        }

        private void LimitSpeed()
        {
            Vector3 xzVelocity = Rigidbody.linearVelocity.Flatten();

            if (xzVelocity.magnitude > GetMaxSpeed())
            {
                xzVelocity = xzVelocity.normalized * GetMaxSpeed();
                xzVelocity.y = Rigidbody.linearVelocity.y;

                Rigidbody.linearVelocity = xzVelocity;
            }
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
        public float GetMaxSpeed() => GetMaxSpeedMultiplier() * (collisioner.IsGround() ? MaxGroundSpeed * moveInput.ClampMagnitude().magnitude : MaxAirSpeed);
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
