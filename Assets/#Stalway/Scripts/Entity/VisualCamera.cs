using System;
using Breaddog.Gameplay;
using Breaddog.Network;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Cinemachine;
using UnityEngine;

public enum CharacterCameraMode
{
    First,
    Third
}

public class VisualCamera : SerializedMonoBehaviour, IInterestOverrider
{
    [Header("Links")]
    [OdinSerialize] public NetworkIdentity Identity { get; protected set; }
    [OdinSerialize] public AbillityCollisioner AbillityCollisioner { get; protected set; }
    [PropertySpace]
    [OdinSerialize] public CinemachineCamera FirstCamera { get; protected set; }
    [OdinSerialize] public CinemachineCamera ThirdCamera { get; protected set; }
    [OdinSerialize] public CinemachineImpulseSource GroundImpulse { get; protected set; }
    [PropertySpace]
    [OdinSerialize] public Transform TrackingTarget { get; protected set; }
    [OdinSerialize] public Transform StandHead { get; protected set; }
    [OdinSerialize] public Transform CrouchHead { get; protected set; }
    [OdinSerialize] public Transform LayHead { get; protected set; }
    [OdinSerialize] public float SwapHeadSpeed { get; protected set; } = 1f;

    [Header("Global")]
    [OdinSerialize] public bool EnableOnlyOwned { get; protected set; }
    [OdinSerialize] public bool CameraModeIsOwned { get; protected set; }
    [OdinSerialize] public CharacterCameraMode CameraMode { get; protected set; } = CharacterCameraMode.First;
    [OdinSerialize, MinMaxSlider(0f, 10f)] public Vector2 GroundImpulseAirHeight { get; protected set; } = new(0.5f, 2.0f);
    [OdinSerialize, MinMaxSlider(0f, 2f)] public Vector2 GroundImpulseForce { get; protected set; } = new(0.2f, 2.0f);


    public CinemachineCamera Camera => CameraMode == CharacterCameraMode.First ? FirstCamera : ThirdCamera;
    public Transform InterestTransform => GetHead();

    private bool CamerasAuthority => EnableOnlyOwned ? Identity.isOwned : true;



    protected virtual void OnEnable()
    {
        if (GroundImpulse != null) AbillityCollisioner.OnGround += OnGround;
    }

    protected virtual void OnDisable()
    {
        if (GroundImpulse != null) AbillityCollisioner.OnGround -= OnGround;
    }


    protected virtual void LateUpdate()
    {
        if (CameraModeIsOwned)
            CameraMode = Identity.isOwned ? CharacterCameraMode.First : CharacterCameraMode.Third;

        FirstCamera.gameObject.SetActive(CameraMode == CharacterCameraMode.First && CamerasAuthority);
        ThirdCamera.gameObject.SetActive(CameraMode == CharacterCameraMode.Third && CamerasAuthority);

        //Camera.Target.TrackingTarget = TrackingTarget;
        //Camera.Target.LookAtTarget = TrackingTarget;

        TrackingTarget.position = Vector3.Lerp(TrackingTarget.position, GetHead().position, SwapHeadSpeed * Time.deltaTime);
    }

    private void OnGround()
    {
        var t = Mathf.InverseLerp(GroundImpulseAirHeight.x, GroundImpulseAirHeight.y, -AbillityCollisioner.HeightInAir);
        var force = Mathf.Lerp(GroundImpulseForce.x, GroundImpulseForce.y, t);

        GroundImpulse.GenerateImpulseWithForce(force);
    }

    private Transform GetHead()
    {
        return AbillityCollisioner.BodyPosition switch
        {
            BodyPosition.Stand => StandHead,
            BodyPosition.Crouch => CrouchHead,
            BodyPosition.Lay => LayHead,
            _ => null
        };
    }
}
