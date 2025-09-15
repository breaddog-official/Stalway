using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    [BurstCompile]
    public static class MathE
    {

        #region Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(params float[] values)
        {
            return Enumerable.Max(values);
        }

        #endregion

        #region ToInteger

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AsNumber(this in bool value, float falsePresent = 0, float truePresent = 1)
        {
            return value ? truePresent : falsePresent;
        }

        #endregion

        #region Double InverseLerp & Clamp

        public static double InverseLerp(double a, double b, double value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }

            return 0d;
        }

        public static double Clamp01(double value)
        {
            if (value < 0d)
            {
                return 0d;
            }

            if (value > 1d)
            {
                return 1d;
            }

            return value;
        }

        #endregion


        #region Vector Max

        /// <summary>
        /// Returns max axis in vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this in Vector2 value)
        {
            Vector2 absValue = value.Abs();
            return MathE.Max(absValue.x, absValue.y);
        }

        /// <summary>
        /// Returns max axis in vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this in Vector3 value)
        {
            Vector3 absValue = value.Abs();
            return MathE.Max(absValue.x, absValue.y, absValue.z);
        }

        /// <summary>
        /// Returns max axis in vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this in Vector4 value)
        {
            Vector4 absValue = value.Abs();
            return MathE.Max(absValue.x, absValue.y, absValue.z, absValue.w);
        }

        #endregion

        #region Vector Abs

        // Getted from Cinemachine extensions

        /// <summary>
        /// Component-wise absolute value
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>Component-wise absolute value of the input vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this in Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        /// <summary>
        /// Component-wise absolute value
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>Component-wise absolute value of the input vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Abs(this in Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        /// <summary>
        /// Component-wise absolute value
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>Component-wise absolute value of the input vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Abs(this in Vector4 v)
        {
            return new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));
        }

        #endregion

        #region Vector Flat

        /// <summary>
        /// Vector3(vector.x, 0, vector.y)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Flatten(this in Vector2 v)
        {
            return new Vector3(v.x, 0f, v.y);
        }

        /// <summary>
        /// Vector3(vector.x, 0, vector.z)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Flatten(this in Vector3 v)
        {
            return new Vector3(v.x, 0f, v.z);
        }

        #endregion

        #region Vector ClampMagnitude

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to 1f.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ClampMagnitude(this Vector2 vector)
        {
            float num = vector.sqrMagnitude;
            if (num > 1f)
            {
                float num2 = (float)Math.Sqrt(num);
                return new Vector2(vector.x / num2, vector.y / num2);
            }

            return vector;
        }

        /// <summary>
        /// Returns a copy of vector with its magnitude clamped to 1f.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampMagnitude(this Vector3 vector)
        {
            float num = vector.sqrMagnitude;
            if (num > 1f)
            {
                float num2 = (float)Math.Sqrt(num);
                return new Vector3(vector.x / num2, vector.y / num2, vector.z / num2);
            }

            return vector;
        }

        #endregion

        #region Vector Approximately

        /// <summary>
        /// Compares two vectors and returns true if they are similar.
        /// </summary>
        public static bool Approximately(this in Vector2 first, in Vector2 second)
        {
            return Mathf.Approximately(first.x, second.x) &&
                   Mathf.Approximately(first.y, second.y);
        }

        /// <summary>
        /// Compares two vectors and returns true if they are similar.
        /// </summary>
        public static bool Approximately(this in Vector3 first, in Vector3 second)
        {
            return Mathf.Approximately(first.x, second.x) &&
                   Mathf.Approximately(first.y, second.y) &&
                   Mathf.Approximately(first.z, second.z);
        }

        /// <summary>
        /// Compares two vectors and returns true if they are similar.
        /// </summary>
        public static bool Approximately(this in Vector4 first, in Vector4 second)
        {
            return Mathf.Approximately(first.x, second.x) &&
                   Mathf.Approximately(first.y, second.y) &&
                   Mathf.Approximately(first.z, second.z) &&
                   Mathf.Approximately(first.w, second.w);
        }

        #endregion

        #region Vector InverseLerp

        public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
        {
            Vector2 ab = b - a;
            Vector2 av = value - a;
            float magnitude = ab.sqrMagnitude;
            if (magnitude == 0f) return 0f; // �������������� ������� �� 0
            return Vector2.Dot(av, ab) / magnitude;
        }

        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 ab = b - a;
            Vector3 av = value - a;
            float magnitude = ab.sqrMagnitude;
            if (magnitude == 0f) return 0f;
            return Vector3.Dot(av, ab) / magnitude;
        }

        public static float InverseLerp(Vector4 a, Vector4 b, Vector4 value)
        {
            Vector4 ab = b - a;
            Vector4 av = value - a;
            float magnitude = Vector4.Dot(ab, ab);
            if (magnitude == 0f) return 0f;
            return Vector4.Dot(av, ab) / magnitude;
        }


        #endregion

        #region Vector Greater and Lower

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThen(this Vector2 vector, Vector2 then)
        {
            return Vector2.Min(vector, then).Approximately(then);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LowerThen(this Vector2 vector, Vector2 then)
        {
            return Vector2.Max(vector, then).Approximately(then);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterThen(this Vector3 vector, Vector3 then)
        {
            return Vector3.Min(vector, then).Approximately(then);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LowerThen(this Vector3 vector, Vector3 then)
        {
            return Vector3.Max(vector, then).Approximately(then);
        }

        #endregion


        #region IncreaseInBounds
        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref int index, IList array) => index.IncreaseInBounds(array.Count);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, IList array) => index.IncreaseInBounds((uint)array.Count);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref int index, int bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;
        }

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, uint bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;
        }


        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static int IncreaseInBoundsReturn(this int index, IList array) => index.IncreaseInBoundsReturn(array.Count);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static uint IncreaseInBoundsReturn(this uint index, IList array) => index.IncreaseInBoundsReturn((uint)array.Count);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static int IncreaseInBoundsReturn(this int index, int bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;

            return index;
        }

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static uint IncreaseInBoundsReturn(this uint index, uint bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;

            return index;
        }
        #endregion

        #region DecreaseInBounds

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref int index, IList array) => index.DecreaseInBounds(array.Count);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref uint index, IList array) => index.DecreaseInBounds((uint)array.Count);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref int index, int bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                index--;
            else
                index = bounds - (dontCollideBounds ? 1 : 0);
        }

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref uint index, uint bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                index--;
            else
                index = bounds - (dontCollideBounds ? 1u : 0u);

        }


        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static int DecreaseInBoundsReturn(this int index, IList array) => index.DecreaseInBoundsReturn(array.Count);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static uint DecreaseInBoundsReturn(this uint index, IList array) => index.DecreaseInBoundsReturn((uint)array.Count);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static int DecreaseInBoundsReturn(this int index, int bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                return index - 1;
            else
                return bounds - (dontCollideBounds ? 1 : 0);
        }

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static uint DecreaseInBoundsReturn(this uint index, uint bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                return index - 1;
            else
                return bounds - (dontCollideBounds ? 1u : 0u);

        }

        #endregion


        #region Percents

        /// <summary>
        /// Leaves only a certain percentage of value
        /// </summary>
        public static float KeepPercents(this float value, float percents)
        {
            return value / 100f * percents;
        }

        /// <summary>
        /// Subtracts a certain percentage from value
        /// </summary>
        public static float LeavePercents(this float value, float percents)
        {
            return value / 100f * (100f - percents);
        }

        #endregion
    }
}
