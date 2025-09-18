using Breaddog.Gameplay;
using UnityEngine;

public class VisualAnimator : MonoBehaviour
{
    [Header("Links")]
    public Animator Animator;
    public AbillityMovement AbillityMovement;
    public AbillityCollisioner AbillityCollisioner;
    public Transform RootTransform;

    [Header("Parameters")]
    public string JumpName = "Jump";
    public string BodyPositionName = "BodyPosition";
    public string ShootName = "Shoot";
    public string VelocityXName = "VelocityX";
    public string VelocityZName = "VelocityZ";
    public string TurnName = "Turn";

    [Header("Global")]
    public bool SimpleJump = true;
    [Space]
    [Min(0.001f)] // Avoiding division by zero
    public float TurnSmooth = 0.5f;
    public float TurnAngle = 20f;
    [Min(0.001f)] // Avoiding division by zero
    public float SpeedSmooth = 0.5f;
    public float SpeedMultiplier = 1f;

    [Header("Sounds")]
    public bool PlayStepSounds = true;
    public bool PlayStepExcludeLay = true;
    public float StepPivotRange = 0.1f;
    public AudioSource StepSource;


    private Vector3 curVelocity;
    private float lastTurn;
    private bool leftStep;
    private bool rightStep;





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

        CheckStep(false);
        CheckStep(true);

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

    // false - left leg
    // true - right leg
    protected virtual void CheckStep(bool leg)
    {
        var step = leg ? leftStep : rightStep;
        var target = leg ? 0f : 1f;
        var ground = Animator.pivotWeight > target - StepPivotRange && Animator.pivotWeight < target + StepPivotRange;

        if (!step && ground && (PlayStepExcludeLay ? AbillityCollisioner.BodyPosition != BodyPosition.Lay : true))
        {
            StepSource.Play();
        }

        if (leg)
            leftStep = ground;
        else
            rightStep = ground;
    }
}
