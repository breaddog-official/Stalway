using UnityEngine;
using UnityEngine.Rendering;

public abstract class ModelBasic : Model
{
    public Renderer Renderer;

    public override void SetShadows(ShadowCastingMode mode)
    {
        Renderer.shadowCastingMode = mode;
    }

    public override void SetActive(bool enabled)
    {
        Renderer.enabled = enabled;
    }
}
