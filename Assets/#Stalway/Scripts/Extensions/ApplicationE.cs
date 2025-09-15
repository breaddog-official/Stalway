using System;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breaddog.Extensions
{
    [Flags]
    public enum PlatformSpecifies
    {
        None = 0,
        ComputeShaders = 1 << 0,
        ShaderLevel_3 = 1 << 1,
        ShaderLevel_4 = 1 << 2,
        ShaderLevel_4_5 = 1 << 3,
        ShaderLevel_5 = 1 << 4,
        Texutres2K = 1 << 5,
        Texutres4K = 1 << 6,
        Texutres8K = 1 << 7,
        Texutres16K = 1 << 8,
        Textures2DArray = 1 << 9,
        Textures3DVolume = 1 << 10,
        AnsotropicTextures = 1 << 11,
        Shadows = 1 << 12,
        MotionVectors = 1 << 13,
        ForwardRendering = 1 << 14,
        DefferedRendering = 1 << 15,
        x64 = 1 << 16,
        RawShadowDepthSampling = 1 << 17,
        Textures3DRender = 1 << 18
    }

    [BurstCompile]
    public static class ApplicationE
    {
        public static RenderingPath RenderPath { get; private set; } = RenderingPath.Forward;
        public static event Action OnRenderPathChanged;

        public static float FixedDeltaTime => Physics.simulationMode == SimulationMode.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;


        public static PlatformSpecifies GetSpecifies()
        {
            PlatformSpecifies specifies = 0;

            int shaderLevel = SystemInfo.graphicsShaderLevel;
            int textureSize = SystemInfo.maxTextureSize;

            if (shaderLevel >= 30) specifies |= PlatformSpecifies.ShaderLevel_3;
            if (shaderLevel >= 40) specifies |= PlatformSpecifies.ShaderLevel_4;
            if (shaderLevel >= 45) specifies |= PlatformSpecifies.ShaderLevel_4_5;
            if (shaderLevel >= 50) specifies |= PlatformSpecifies.ShaderLevel_5;

            if (textureSize >= 2048) specifies |= PlatformSpecifies.Texutres2K;
            if (textureSize >= 4096) specifies |= PlatformSpecifies.Texutres4K;
            if (textureSize >= 8192) specifies |= PlatformSpecifies.Texutres8K;
            if (textureSize >= 16384) specifies |= PlatformSpecifies.Texutres16K;

            if (SystemInfo.supports2DArrayTextures) specifies |= PlatformSpecifies.Textures2DArray;
            if (SystemInfo.supports3DTextures) specifies |= PlatformSpecifies.Textures3DVolume;
            if (SystemInfo.supports3DRenderTextures) specifies |= PlatformSpecifies.Textures3DRender;
            if (SystemInfo.supportsComputeShaders) specifies |= PlatformSpecifies.ComputeShaders;
            if (SystemInfo.supportsAnisotropicFilter) specifies |= PlatformSpecifies.AnsotropicTextures;
            if (SystemInfo.supportsShadows) specifies |= PlatformSpecifies.Shadows;
            if (SystemInfo.supportsMotionVectors) specifies |= PlatformSpecifies.MotionVectors;
            if (SystemInfo.supportsRawShadowDepthSampling) specifies |= PlatformSpecifies.RawShadowDepthSampling;

            if (RenderPath == RenderingPath.Forward) specifies |= PlatformSpecifies.ForwardRendering;
            if (RenderPath == RenderingPath.DeferredShading) specifies |= PlatformSpecifies.DefferedRendering;

            if (IntPtr.Size >= 8) specifies |= PlatformSpecifies.x64;


            return specifies;
        }

        public static bool IsSelectableInput()
        {
            return InputSystem.devices.Where(d => d is Pointer && d is not Touchscreen).Count() <= 0;
        }

        public static void SetRenderPath(RenderingPath path)
        {
            RenderPath = path;
            OnRenderPathChanged?.Invoke();
        }
    }
}
