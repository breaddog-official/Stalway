using UnityEngine;
using UnityEngine.Rendering;

public abstract class Model : MonoBehaviour
{
    public abstract void SetShadows(ShadowCastingMode mode);
    public abstract void SetActive(bool enabled);
}
