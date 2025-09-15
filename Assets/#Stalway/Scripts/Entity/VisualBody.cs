using System;
using Breaddog.Gameplay;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class VisualBody : SerializedMonoBehaviour
{
    [Header("Links")]
    [OdinSerialize] public VisualCamera VisualCamera { get; protected set; }
    [OdinSerialize] public GameObject[] DefaultModelParts { get; protected set; }
    [OdinSerialize] public GameObject[] ArmsModelParts { get; protected set; }
    [PropertySpace]
    [OdinSerialize] public Transform DefaultModel { get; protected set; }
    [OdinSerialize] public Vector3 FirstPersonOffset { get; protected set; }

    private Vector3 cachedModelPos;


    protected virtual void Awake()
    {
        cachedModelPos = DefaultModel.localPosition;
    }

    protected virtual void LateUpdate()
    {
        // Model for all players enables only in ThirdPerson
        foreach (var model in DefaultModelParts)
        {
            model.SetActive(VisualCamera.CameraMode == CharacterCameraMode.Third);
        }

        // Arms model enables only in FirstPerson
        foreach (var model in ArmsModelParts)
        {
            model.SetActive(VisualCamera.CameraMode == CharacterCameraMode.First);
        }

        DefaultModel.localPosition = VisualCamera.CameraMode == CharacterCameraMode.First ? cachedModelPos + FirstPersonOffset : cachedModelPos;
    }
}
