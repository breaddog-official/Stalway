using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[ExecuteAlways]
public class CopyTransform : SerializedMonoBehaviour
{
    [OdinSerialize] public Transform From { get; protected set; }
    [PropertySpace]
    [OdinSerialize] public bool Position { get; protected set; } = true;
    [OdinSerialize] public bool Rotation { get; protected set; } = true;
    [OdinSerialize] public bool Scale { get; protected set; }

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
