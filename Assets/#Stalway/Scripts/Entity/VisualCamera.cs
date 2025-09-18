using Breaddog.Gameplay;
using Breaddog.Network;
using Mirror;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

public enum CharacterCameraMode
{
    First,
    Third
}

public class VisualCamera : MonoBehaviour, IInterestOverrider
{
    [Header("Links")]
    public NetworkIdentity Identity;
    public AbillityCollisioner AbillityCollisioner;
    [Space]
    public CinemachineCamera FirstCamera;
    public CinemachineCamera ThirdCamera;
    public CinemachineImpulseSource GroundImpulse;
    [Space]
    public Transform TrackingTarget;
    public Transform StandHead;
    public Transform CrouchHead;
    public Transform LayHead;
    public float SwapHeadSpeed = 1f;

    [Header("Global")]
    public SteamAudio.SteamAudioSource[] Sources;
    public bool EnableOnlyOwned;
    public bool CameraModeIfLocalPlayer;
    public CharacterCameraMode CameraMode = CharacterCameraMode.First;
    [MinMaxSlider(0f, 10f)] public Vector2 GroundImpulseAirHeight = new(0.5f, 2.0f);
    [MinMaxSlider(0f, 2f)] public Vector2 GroundImpulseForce = new(0.2f, 2.0f);


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
        if (CameraModeIfLocalPlayer)
            CameraMode = Identity.isLocalPlayer ? CharacterCameraMode.First : CharacterCameraMode.Third;

        FirstCamera.gameObject.SetActive(CameraMode == CharacterCameraMode.First && CamerasAuthority);
        ThirdCamera.gameObject.SetActive(CameraMode == CharacterCameraMode.Third && CamerasAuthority);

        foreach (var source in Sources)
        {
            source.occlusion = Identity.isLocalPlayer;
        }

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
