using UnityEngine;

[ExecuteAlways]
public class CopyTransform : MonoBehaviour
{
    public Transform From;
    [Space]
    public bool Position = true;
    public bool Rotation = true;
    public bool Scale;

    public Transform Target => transform;


    protected virtual void Update()
    {
        if (From != null)
        {
            if (Position) Target.position = From.position;
            if (Rotation) Target.rotation = From.rotation;
            if (Scale) Target.localScale = From.localScale;
        }
    }
}
