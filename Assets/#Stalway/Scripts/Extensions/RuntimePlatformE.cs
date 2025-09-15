using System;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    [Flags]
    public enum RuntimePlatformFlags
    {
        None = 0,
        OSXEditor = 1 << 0,
        OSXPlayer = 1 << 1,
        WindowsPlayer = 1 << 2,
        WindowsEditor = 1 << 3,
        IPhonePlayer = 1 << 4,
        Android = 1 << 5,
        LinuxPlayer = 1 << 6,
        LinuxEditor = 1 << 7,
        WebGLPlayer = 1 << 8,
        WSAPlayerX86 = 1 << 9,
        WSAPlayerX64 = 1 << 10,
        WSAPlayerARM = 1 << 11,
        PS4 = 1 << 12,
        XboxOne = 1 << 13,
        tvOS = 1 << 14,
        Switch = 1 << 15,
        PS5 = 1 << 16,
        LinuxServer = 1 << 17,
        WindowsServer = 1 << 18,
        OSXServer = 1 << 19,
        VisionOS = 1 << 20
    }

    [BurstCompile]
    public static class RuntimePlatformConverter
    {
        public static RuntimePlatformFlags ToFlags(this RuntimePlatform platform)
        {
            return platform switch
            {
                RuntimePlatform.OSXEditor => RuntimePlatformFlags.OSXEditor,
                RuntimePlatform.OSXPlayer => RuntimePlatformFlags.OSXPlayer,
                RuntimePlatform.WindowsPlayer => RuntimePlatformFlags.WindowsPlayer,
                RuntimePlatform.WindowsEditor => RuntimePlatformFlags.WindowsEditor,
                RuntimePlatform.IPhonePlayer => RuntimePlatformFlags.IPhonePlayer,
                RuntimePlatform.Android => RuntimePlatformFlags.Android,
                RuntimePlatform.LinuxPlayer => RuntimePlatformFlags.LinuxPlayer,
                RuntimePlatform.LinuxEditor => RuntimePlatformFlags.LinuxEditor,
                RuntimePlatform.WebGLPlayer => RuntimePlatformFlags.WebGLPlayer,
                RuntimePlatform.WSAPlayerX86 => RuntimePlatformFlags.WSAPlayerX86,
                RuntimePlatform.WSAPlayerX64 => RuntimePlatformFlags.WSAPlayerX64,
                RuntimePlatform.WSAPlayerARM => RuntimePlatformFlags.WSAPlayerARM,
                RuntimePlatform.PS4 => RuntimePlatformFlags.PS4,
                RuntimePlatform.XboxOne => RuntimePlatformFlags.XboxOne,
                RuntimePlatform.tvOS => RuntimePlatformFlags.tvOS,
                RuntimePlatform.Switch => RuntimePlatformFlags.Switch,
                RuntimePlatform.PS5 => RuntimePlatformFlags.PS5,
                RuntimePlatform.LinuxServer => RuntimePlatformFlags.LinuxServer,
                RuntimePlatform.WindowsServer => RuntimePlatformFlags.WindowsServer,
                RuntimePlatform.OSXServer => RuntimePlatformFlags.OSXServer,
                RuntimePlatform.VisionOS => RuntimePlatformFlags.VisionOS,
                _ => RuntimePlatformFlags.None
            };
        }

        public static RuntimePlatform ToRuntimePlatform(this RuntimePlatformFlags flags)
        {
            return flags switch
            {
                RuntimePlatformFlags.OSXEditor => RuntimePlatform.OSXEditor,
                RuntimePlatformFlags.OSXPlayer => RuntimePlatform.OSXPlayer,
                RuntimePlatformFlags.WindowsPlayer => RuntimePlatform.WindowsPlayer,
                RuntimePlatformFlags.WindowsEditor => RuntimePlatform.WindowsEditor,
                RuntimePlatformFlags.IPhonePlayer => RuntimePlatform.IPhonePlayer,
                RuntimePlatformFlags.Android => RuntimePlatform.Android,
                RuntimePlatformFlags.LinuxPlayer => RuntimePlatform.LinuxPlayer,
                RuntimePlatformFlags.LinuxEditor => RuntimePlatform.LinuxEditor,
                RuntimePlatformFlags.WebGLPlayer => RuntimePlatform.WebGLPlayer,
                RuntimePlatformFlags.WSAPlayerX86 => RuntimePlatform.WSAPlayerX86,
                RuntimePlatformFlags.WSAPlayerX64 => RuntimePlatform.WSAPlayerX64,
                RuntimePlatformFlags.WSAPlayerARM => RuntimePlatform.WSAPlayerARM,
                RuntimePlatformFlags.PS4 => RuntimePlatform.PS4,
                RuntimePlatformFlags.XboxOne => RuntimePlatform.XboxOne,
                RuntimePlatformFlags.tvOS => RuntimePlatform.tvOS,
                RuntimePlatformFlags.Switch => RuntimePlatform.Switch,
                RuntimePlatformFlags.PS5 => RuntimePlatform.PS5,
                RuntimePlatformFlags.LinuxServer => RuntimePlatform.LinuxServer,
                RuntimePlatformFlags.WindowsServer => RuntimePlatform.WindowsServer,
                RuntimePlatformFlags.OSXServer => RuntimePlatform.OSXServer,
                RuntimePlatformFlags.VisionOS => RuntimePlatform.VisionOS,
                _ => throw new ArgumentException("Invalid platform flag")
            };
        }
    }
}
