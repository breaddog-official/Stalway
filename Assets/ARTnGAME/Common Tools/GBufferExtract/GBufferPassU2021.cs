using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GBufferPassU2021 : ScriptableRenderPass
{

    //[System.Serializable]
    //public class Settings
    //{
    //    [Range(0.1f, 1f)] public float ResolutionMultiplier = 0.5f;

    //    public bool DeferredPassOn = true;
    //    [Tooltip("Multiple Render Targets - More optimized GBuffer pass but support for fewer platforms:\nDX11+, OpenGL 3.2+, OpenGL ES 3+, Metal, Vulkan, PS4/XB1")]
    //    public bool UseMRT = true;

    //   // [Header("Debug")]
    //   // public DebugMode DebugMode = DebugMode.None;
    //    public bool DebugModeInSceneView;
    //    public bool ShowInSceneView = true;
    //}


    const string ALBEDO_HANDLE_ID = "_AlbedoTexture";
    const string SPECULAR_HANDLE_ID = "_SpecularTexture";
    const string WORLD_POSITION_HANDLE_ID = "_WorldPositionsTexture";
    const string DEPTH_NORMAL_HANDLE_ID = "_DepthNormalsTexture";

    const string ALBEDO_ID = "_AlbedoTexture";
    const string SPECULAR_ID = "_SpecularTexture";
    const string WORLD_POSITIONS_ID = "_WorldPositionsTexture";
    const string DEPTH_NORMAL_ID = "_DepthNormalsTexture";

    static readonly ShaderTagId gBufferShaderTag = new ShaderTagId("UniversalGBuffer");// ("R_GBuffer");//"UniversalGBuffer");

    RTHandle albedoHandle;
    RTHandle specularHandle;
    RTHandle worldPosHandle;
    RTHandle depthNormalHandle;

    FilteringSettings filteringSettings;
    RenderStateBlock renderStateBlock;

    RenderTargetIdentifier colorTarget;

    GBufferPassU2021Feature.Settings _settings;

    public ref RTHandle AlbedoHandle => ref albedoHandle;
    public ref RTHandle SpecularHandle => ref specularHandle;
    public ref RTHandle WorldPosHandle => ref worldPosHandle;
    public ref RTHandle DepthNormalHandle => ref depthNormalHandle;
    //public ref RenderTargetIdentifier AlbedoHandle => ref albedoHandle;
    //public ref RenderTargetIdentifier SpecularHandle => ref specularHandle;
    //public ref RenderTargetIdentifier WorldPosHandle => ref worldPosHandle;
    //public ref RenderTargetIdentifier DepthNormalHandle => ref depthNormalHandle;

    public GBufferPassU2021(GBufferPassU2021Feature.Settings settings)
    {
        _settings = settings;

        renderPassEvent = _settings.eventA;// RenderPassEvent.AfterRenderingPrePasses;

        filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

        //albedoHandle.Init(ALBEDO_HANDLE_ID);
        //specularHandle.Init(SPECULAR_HANDLE_ID);
        //worldPosHandle.Init(WORLD_POSITION_HANDLE_ID);
        // depthNormalHandle.Init(DEPTH_NORMAL_HANDLE_ID);

        //RTHandles.Alloc(albedoHandle, albedoHandle.name);
        //RTHandles.Alloc(albedoHandle, albedoHandle.name);
        //RTHandles.Alloc(albedoHandle, albedoHandle.name);
        //RTHandles.Alloc(albedoHandle, albedoHandle.name);

        //albedoHandle = new RenderTargetIdentifier("ALBEDO_HANDLE_ID");
        //specularHandle = new RenderTargetIdentifier("SPECULAR_HANDLE_ID");
        //worldPosHandle = new RenderTargetIdentifier("WORLD_POSITION_HANDLE_ID");
        //depthNormalHandle = new RenderTargetIdentifier("DEPTH_NORMAL_HANDLE_ID");

        //https://forum.unity.com/threads/rendertargethandle-is-obsolete-deprecated-in-favor-of-rthandle.1211052/
        //albedoHandle = RTHandles.Alloc("ALBEDO_HANDLE_ID", name: "ALBEDO_HANDLE_ID");
        //specularHandle = RTHandles.Alloc("SPECULAR_HANDLE_ID", name: "SPECULAR_HANDLE_ID");
        //worldPosHandle = RTHandles.Alloc("WORLD_POSITION_HANDLE_ID", name: "WORLD_POSITION_HANDLE_ID");
        //depthNormalHandle = RTHandles.Alloc("DEPTH_NORMAL_HANDLE_ID", name: "DEPTH_NORMAL_HANDLE_ID");
    }

    public void Setup(RenderTargetIdentifier colorTarget)
    {
        this.colorTarget = colorTarget;
    }
    public float ResolutionMultiplier = 1;
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        int width = (int)((float)cameraTextureDescriptor.width);
        int height = (int)((float)cameraTextureDescriptor.height);

        // ALBEDO
        {
            var rtd = cameraTextureDescriptor;
            rtd.graphicsFormat = QualitySettings.activeColorSpace == ColorSpace.Linear ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm;
            rtd.width = width;
            rtd.height = height;
            rtd.depthBufferBits = 0;
            rtd.msaaSamples = 1;
            //cmd.GetTemporaryRT(Shader.PropertyToID(albedoHandle.name), rtd, FilterMode.Bilinear);

            //RenderingUtils.ReAllocateIfNeeded(ref albedoHandle, rtd, FilterMode.Bilinear, TextureWrapMode.Clamp, name: ALBEDO_HANDLE_ID);
            if (albedoHandle == null)
            {
                albedoHandle = RTHandles.Alloc(width, height, colorFormat: GraphicsFormat.R32G32B32A32_SFloat,
                    filterMode: FilterMode.Bilinear, wrapMode: TextureWrapMode.Clamp, name: "ALBEDO_HANDLE_ID");
            }

            //cmd.SetComputeTextureParam(ComputeShaderUtils.LightsCompute, ComputeShaderUtils.LightsComputeKernels.ComputeLightsKernelID, ALBEDO_ID, albedoHandle.rt);
        }

        // SPECULAR
        {
            var rtd = cameraTextureDescriptor;
            rtd.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
            rtd.width = width;
            rtd.height = height;
            rtd.depthBufferBits = 0;
            rtd.msaaSamples = 1;
            // cmd.GetTemporaryRT(Shader.PropertyToID(specularHandle.name), rtd, FilterMode.Bilinear);

            //RenderingUtils.ReAllocateIfNeeded(ref specularHandle, rtd, FilterMode.Bilinear, TextureWrapMode.Clamp, name: SPECULAR_HANDLE_ID);
            if (specularHandle == null)
            {
                specularHandle = RTHandles.Alloc(width, height, colorFormat: GraphicsFormat.R32G32B32A32_SFloat,
                    filterMode: FilterMode.Bilinear, wrapMode: TextureWrapMode.Clamp, name: "SPECULAR_HANDLE_ID");
            }
            ///cmd.SetComputeTextureParam(ComputeShaderUtils.LightsCompute, ComputeShaderUtils.LightsComputeKernels.ComputeLightsKernelID, SPECULAR_ID, specularHandle.rt);
        }

        // WORLD_POS
        {
            var rtd = cameraTextureDescriptor;
            rtd.graphicsFormat = GraphicsFormat.R32G32B32A32_SFloat;
            rtd.depthBufferBits = 0;
            rtd.msaaSamples = 1;
            rtd.width = width;
            rtd.height = height;
            // cmd.GetTemporaryRT(Shader.PropertyToID(worldPosHandle.name), rtd, FilterMode.Bilinear);
            //RenderingUtils.ReAllocateIfNeeded(ref worldPosHandle, rtd, FilterMode.Bilinear, TextureWrapMode.Clamp, name: WORLD_POSITION_HANDLE_ID);
            if (worldPosHandle == null)
            {
                worldPosHandle = RTHandles.Alloc(width, height, colorFormat: GraphicsFormat.R32G32B32A32_SFloat,
                    filterMode: FilterMode.Bilinear, wrapMode: TextureWrapMode.Clamp, name: "WORLD_POSITION_HANDLE_ID");
            }
            // cmd.SetComputeTextureParam(ComputeShaderUtils.TilesCompute, ComputeShaderUtils.TilesComputeKernels.ComputeLightTilesKernelID, WORLD_POSITIONS_ID, worldPosHandle.rt);
            // cmd.SetComputeTextureParam(ComputeShaderUtils.LightsCompute, ComputeShaderUtils.LightsComputeKernels.ComputeLightsKernelID, WORLD_POSITIONS_ID, worldPosHandle.rt);
        }

        // DEPTH_NORMAL
        {
            var rtd = cameraTextureDescriptor;
            rtd.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
            rtd.depthBufferBits = 0;
            rtd.msaaSamples = 1;
            rtd.width = width;
            rtd.height = height;
            //cmd.GetTemporaryRT(Shader.PropertyToID(depthNormalHandle.name), rtd, FilterMode.Bilinear);
            if (depthNormalHandle == null)
            {
                depthNormalHandle = RTHandles.Alloc(width, height, colorFormat: GraphicsFormat.R32G32B32A32_SFloat,
                    filterMode: FilterMode.Bilinear, wrapMode: TextureWrapMode.Clamp, name: "DEPTH_NORMAL_HANDLE_ID");
            }
            //RenderingUtils.ReAllocateIfNeeded(ref depthNormalHandle, rtd, FilterMode.Bilinear, TextureWrapMode.Clamp, name: DEPTH_NORMAL_HANDLE_ID);
            // cmd.SetComputeTextureParam(ComputeShaderUtils.TilesCompute, ComputeShaderUtils.TilesComputeKernels.ComputeLightTilesKernelID, DEPTH_NORMAL_ID, depthNormalHandle.rt);
            // cmd.SetComputeTextureParam(ComputeShaderUtils.LightsCompute, ComputeShaderUtils.LightsComputeKernels.ComputeLightsKernelID, DEPTH_NORMAL_ID, depthNormalHandle.rt);
        }

        //ConfigureTarget((new RenderTargetIdentifier[] {
        //        albedoHandle.rt,
        //        specularHandle.rt,
        //        worldPosHandle.rt,
        //        depthNormalHandle.rt,
        //    });

        /*ConfigureTarget((new RenderTargetIdentifier[] {
                albedoHandle.rt,
                specularHandle.rt,
                worldPosHandle.rt,
                depthNormalHandle.rt,
            }));*/

        ConfigureTarget((new RTHandle[] {
                albedoHandle,
                specularHandle,
                worldPosHandle,
                depthNormalHandle,
            }));

        ConfigureClear(ClearFlag.All, Color.black);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get();
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        using (new ProfilingScope(cmd, new ProfilingSampler("DeferredLights::GBufferPass")))
        {
            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawSettings = CreateDrawingSettings(gBufferShaderTag, ref renderingData, sortFlags);
            drawSettings.enableDynamicBatching = true;
            drawSettings.enableInstancing = true;

            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings, ref renderStateBlock);

            cmd.SetGlobalTexture("_DeferredPass_Albedo_Texture", albedoHandle.rt);
            cmd.SetGlobalTexture("_DeferredPass_Specular_Texture", specularHandle.rt);
            cmd.SetGlobalTexture("_DeferredPass_WorldPosition_Texture", worldPosHandle.rt);
            cmd.SetGlobalTexture("_DeferredPass_DepthNormals_Texture", depthNormalHandle.rt);

            cmd.SetGlobalTexture("_GBuffer0", albedoHandle.rt);
            cmd.SetGlobalTexture("_GBuffer1", specularHandle.rt);
            cmd.SetGlobalTexture("_GBuffer2", worldPosHandle.rt);
            cmd.SetGlobalTexture("_CameraGBufferTexture2", worldPosHandle.rt);

            //Debug.Log("E2");
        }

        CoreUtils.SetRenderTarget(cmd, colorTarget, ClearFlag.All, Color.black);

        // Debug.Log("E3");

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("ALBEDO_HANDLE_ID"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("SPECULAR_HANDLE_ID"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("WORLD_POSITION_HANDLE_ID"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("DEPTH_NORMAL_HANDLE_ID"));
    }
}
