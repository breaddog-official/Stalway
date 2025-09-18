using UnityEngine;
using UnityEngine.Rendering;

public class VisualBody : MonoBehaviour
{
    [Header("Links")]
    public VisualCamera VisualCamera;
    [Space]
    public bool AlwaysShowLegs = true;
    public ModelHumanoid DefaultModel;
    public ModelHumanoid FirstPersonModel;
    public Vector3 FirstPersonOffset;

    private Vector3 cachedModelPos;


    protected virtual void Awake()
    {
        cachedModelPos = DefaultModel.transform.localPosition;
    }

    protected virtual void LateUpdate()
    {
        DefaultModel.transform.localPosition = VisualCamera.CameraMode == CharacterCameraMode.First ? cachedModelPos + FirstPersonOffset : cachedModelPos;

        DefaultModel.SetActive(true);
        DefaultModel.SetShadows(VisualCamera.CameraMode == CharacterCameraMode.Third ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly);
        if (AlwaysShowLegs) DefaultModel.SetShadowsLegs(ShadowCastingMode.On);

        FirstPersonModel.SetActive(false);
        FirstPersonModel.SetShadows(ShadowCastingMode.Off);
        FirstPersonModel.SetActiveArms(VisualCamera.CameraMode == CharacterCameraMode.First);
    }
}
