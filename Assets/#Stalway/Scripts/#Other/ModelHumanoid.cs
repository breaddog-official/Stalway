using UnityEngine;
using UnityEngine.Rendering;

public class ModelHumanoid : Model
{
    public Renderer[] Head;
    public Renderer[] Body;
    public Renderer[] Arms;
    public Renderer[] Legs;

    public override void SetShadows(ShadowCastingMode mode)
    {
        SetShadowsHead(mode);
        SetShadowsBody(mode);
        SetShadowsArms(mode);
        SetShadowsLegs(mode);
    }

    public virtual void SetShadowsHead(ShadowCastingMode mode)
    {
        foreach (var renderer in Head) renderer.shadowCastingMode = mode;
    }

    public virtual void SetShadowsBody(ShadowCastingMode mode)
    {
        foreach (var renderer in Body) renderer.shadowCastingMode = mode;
    }

    public virtual void SetShadowsArms(ShadowCastingMode mode)
    {
        foreach (var renderer in Arms) renderer.shadowCastingMode = mode;
    }

    public virtual void SetShadowsLegs(ShadowCastingMode mode)
    {
        foreach (var renderer in Legs) renderer.shadowCastingMode = mode;
    }





    public override void SetActive(bool enabled)
    {
        SetActiveHead(enabled);
        SetActiveBody(enabled);
        SetActiveArms(enabled);
        SetActiveLegs(enabled);
    }

    public virtual void SetActiveHead(bool enabled)
    {
        foreach (var renderer in Head) renderer.enabled = enabled;
    }

    public virtual void SetActiveBody(bool enabled)
    {
        foreach (var renderer in Body) renderer.enabled = enabled;
    }

    public virtual void SetActiveArms(bool enabled)
    {
        foreach (var renderer in Arms) renderer.enabled = enabled;
    }

    public virtual void SetActiveLegs(bool enabled)
    {
        foreach (var renderer in Legs) renderer.enabled = enabled;
    }
}