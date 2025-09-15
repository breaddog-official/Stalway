using System;
using Breaddog.Extensions;
using Breaddog.Gameplay;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class VisualAnimator : SerializedMonoBehaviour
{
    [Header("Links")]
    [OdinSerialize] public Animator Animator { get; protected set; }
    [OdinSerialize] public AbillityMovement AbillityMovement { get; protected set; }
    [OdinSerialize] public AbillityCollisioner AbillityCollisioner { get; protected set; }
    [OdinSerialize] public Transform RootTransform { get; protected set; }
    //[OdinSerialize] public AbillityWeapon AbillityWeapon { get; protected set; }

    [Header("Parameters")]
    [OdinSerialize] public string JumpName { get; protected set; } = "Jump";
    [OdinSerialize] public string BodyPositionName { get; protected set; } = "BodyPosition";
    [OdinSerialize] public string ShootName { get; protected set; } = "Shoot";
    [OdinSerialize] public string VelocityXName { get; protected set; }
    [OdinSerialize] public string VelocityZName { get; protected set; }
    [OdinSerialize] public string TurnName { get; protected set; }

    [Header("Global")]
    [OdinSerialize] public bool SimpleJump { get; protected set; } = true;
    [PropertySpace]
    [MinValue(0.001f)] // Avoiding division by zero
    [OdinSerialize] public float TurnSmooth { get; protected set; } = 0.5f;
    [OdinSerialize] public float TurnAngle { get; protected set; } = 20f;
    [MinValue(0.001f)] // Avoiding division by zero
    [OdinSerialize] public float SpeedSmooth { get; protected set; } = 0.5f;
    [OdinSerialize] public float SpeedMultiplier { get; protected set; } = 1f;

    private Vector3 curVelocity;
    private float lastTurn;


    protected virtual void Awake()
    {
        lastTurn = RootTransform.eulerAngles.y;
    }

    protected virtual void LateUpdate()
    {
        var jump = AbillityCollisioner.IsAir();
        var rawVelocity = RootTransform.InverseTransformVector(AbillityMovement.GetVelocity() * SpeedMultiplier);
        rawVelocity *= jump && SimpleJump ? 0f : 1f;
        curVelocity = Vector3.Lerp(curVelocity, rawVelocity, Time.deltaTime / SpeedSmooth);

        float turnDelta = RootTransform.eulerAngles.y - lastTurn; // Delta in degrees
        float turn = Mathf.InverseLerp(0f, TurnAngle, Mathf.Abs(turnDelta));
        turn = turnDelta > 0f ? turn : -turn;

        lastTurn = Mathf.Lerp(lastTurn, RootTransform.eulerAngles.y, Time.deltaTime / TurnSmooth);


        var requestedShoot = false;
        var bodyPos = (int)AbillityCollisioner.BodyPosition;

        Animator.SetFloat(VelocityXName, curVelocity.x);
        Animator.SetFloat(VelocityZName, curVelocity.z);
        Animator.SetFloat(TurnName, turn);
        Animator.SetInteger(BodyPositionName, bodyPos);
        Animator.SetBool(JumpName, jump && !SimpleJump);

        if (requestedShoot)
        {
            Animator.SetTrigger(ShootName);
        }
    }
}
