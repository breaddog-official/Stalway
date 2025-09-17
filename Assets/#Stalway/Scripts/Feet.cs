using Mirror;
using UnityEngine;

public class Feet : MonoBehaviour
{
    public AudioSource StepSource;
    public Vector3 RayVector = new(0f, -0.05f, 0f);
    public LayerMask RayLayers;
    public float Delay = 0.05f;

    private bool stepped;
    private double lastStep;

    private void FixedUpdate()
    {
        var ground = Physics.Raycast(transform.position, RayVector, RayVector.magnitude, RayLayers);

        if (!stepped && ground && NetworkTime.time > lastStep + Delay)
        {
            StepSource.Play();
            lastStep = NetworkTime.time;
        }

        stepped = ground;
    }
}
