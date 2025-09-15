using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    [BurstCompile]
    public static class ExtensionsE
    {

        #region CheckInitialization
        /// <summary>
        /// If already initialized, returns true, however if not initialized, <br />
        /// returns false and makes the field <see href="isInitialized"/> true.
        /// </summary>
        public static bool CheckInitialization(this ref bool isInitialized)
        {
            if (isInitialized)
                return true;

            isInitialized = true;
            return false;
        }

        #endregion

        #region IfNotNull & IfNull
        /// <summary>
        /// Invokes an action if not null
        /// </summary>
        public static bool IfNotNull(this object value, Action action)
        {
            if (value != null)
                action?.Invoke();

            return value != null;
        }

        /// <summary>
        /// Invokes an action if not null
        /// </summary>
        public static bool IfNotNull<T>(this T value, Action<T> action)
        {
            if (value != null)
                action?.Invoke(value);

            return value != null;
        }




        /// <summary>
        /// Invokes an action if null
        /// </summary>
        public static bool IfNull(this object value, Action action)
        {
            if (value == null)
                action?.Invoke();

            return value == null;
        }

        #endregion

        #region HasFlag

        /// <summary>
        /// Checks, is 'Enum' has 'flag'
        /// </summary>
        public static bool HasFlag(this int Enum, int flag)
        {
            return (Enum & flag) == flag;
        }

        #endregion

        #region HasLayer

        /// <summary>
        /// Checks if layermask containts layer
        /// </summary>
        public static bool HasLayer(this LayerMask layerMask, int layer)
        {
            return layerMask.value.HasFlag(layer);
        }

        #endregion

        #region ConvertInput

        /// <summary>
        /// Converts Vector2 input to Vector3
        /// </summary>
        public static Vector3 ConvertInputToVector3(this Vector2 input)
        {
            return new Vector3(input.x, 0.0f, input.y);
        }

        /// <summary>
        /// Converts Vector3 input to Vector2
        /// </summary>
        public static Vector2 ConvertInputToVector2(this Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        #endregion

        #region ConvertSecondsToMiliseconds

        /// <summary>
        /// Converts float seconds to int miliseconds
        /// </summary>
        public static int ConvertSecondsToMiliseconds(this float seconds)
        {
            return (int)(seconds * 1000);
        }


        #endregion

        #region Reset Token

        /// <summary>
        /// Cancels and disposes a token
        /// </summary>
        public static void ResetToken(this CancellationTokenSource source)
        {
            if (source != null)
            {
                if (source.Token.CanBeCanceled)
                    source.Cancel();

                source.Dispose();
            }
        }


        #endregion

        #region Support paths

        /// <summary>
        /// Is platform supporting dataPath?
        /// </summary>
        public static bool SupportDataPath(this RuntimePlatform value)
        {
            return value is not RuntimePlatform.IPhonePlayer or RuntimePlatform.Android or RuntimePlatform.WebGLPlayer or RuntimePlatform.WSAPlayerARM or RuntimePlatform.WSAPlayerX64 or RuntimePlatform.WSAPlayerX86;
        }

        /// <summary>
        /// Is platform supporting persistentDataPath?
        /// </summary>
        public static bool SupportPersistentDataPath(this RuntimePlatform value)
        {
            return value is not RuntimePlatform.tvOS or RuntimePlatform.WebGLPlayer or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor;
        }

        #endregion

        #region FlagsToArray

        public static IEnumerable<T> FlagsToArray<T>(this T @enum) where T : struct
        {
            var array = @enum.ToString()
                .Split(new string[] { ", " }, StringSplitOptions.None)
                .Select(i => Enum.Parse<T>(i));

            var first = array.First().ToString();
            var names = Enum.GetNames(typeof(T));

            if (first == names[0])
                array = array.Skip(1);

            else if (first == "-1")
            {
                array = names.Select(i => Enum.Parse<T>(i));
                array = array.Skip(1);
            }

            return array;
        }

        #endregion

        #region Measure

        public static long Measure(this Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public static long Measure<T1>(this Action<T1> action, T1 t1)
        {
            var sw = Stopwatch.StartNew();
            action(t1);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public static long Measure<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
        {
            var sw = Stopwatch.StartNew();
            action(t1, t2);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public static long Measure<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            var sw = Stopwatch.StartNew();
            action(t1, t2, t3);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public static long Measure<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var sw = Stopwatch.StartNew();
            action(t1, t2, t3, t4);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        #endregion

        #region ProgressBarColor

        public static Color GetProgressBarColor(this float value, float minValue, float maxValue, Color minColor, Color maxColor)
        {
            return Color.Lerp(minColor, maxColor, Mathf.InverseLerp(minValue, maxValue, value));
        }

        #endregion
    }
}
