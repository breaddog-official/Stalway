using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public enum RigidbodyQuality
{
    Default,
    Low,
    Medium,
    High
}

public class RigidbodyAdvanced : SerializedMonoBehaviour
{
    [OdinSerialize] public RigidbodyQuality RigidbodyQuality { get; protected set; }

    private const int lowIterations = 4;
    private const int mediumIterations = 8;
    private const int highIterations = 16;

    private const double velocityDivider = 4;

    public void Start()
    {
        if (RigidbodyQuality != RigidbodyQuality.Default && TryGetComponent(out Rigidbody rb))
        {
            int iterations = RigidbodyQuality switch
            {
                RigidbodyQuality.Low => lowIterations,
                RigidbodyQuality.Medium => mediumIterations,
                RigidbodyQuality.High => highIterations,
                _ => throw new NotImplementedException()
            };
            int velocityIterations = (int)(iterations / velocityDivider);

            rb.solverIterations = iterations;
            rb.solverVelocityIterations = velocityIterations;
        }
    }
}
