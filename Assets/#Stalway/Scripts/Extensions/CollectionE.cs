using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    /// <summary>
    /// ������� � �������� �������������
    /// </summary>
    public enum Rotation4 : byte
    {
        Up,
        Right,
        Down,
        Left
    }

    [BurstCompile]
    public static class CollectionE
    {

        #region RotateShape
        public static Array2D<bool> RotateShape(this Array2D<bool> shape, Rotation4 rotation)
        {
            int w = shape.Width;
            int h = shape.Height;
            Array2D<bool> result = null;

            switch (rotation)
            {
                case Rotation4.Up:
                    return new Array2D<bool>(shape); // �����
                case Rotation4.Right:
                    result = new Array2D<bool>(h, w);
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            result[h - 1 - y, x] = shape[x, y];
                    break;
                case Rotation4.Down:
                    result = new Array2D<bool>(w, h);
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            result[w - 1 - x, h - 1 - y] = shape[x, y];
                    break;
                case Rotation4.Left:
                    result = new Array2D<bool>(h, w);
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            result[y, w - 1 - x] = shape[x, y];
                    break;
            }

            return result;
        }

        public static Vector2 RotatedSize(this Array2D<bool> shape, Rotation4 rotation)
        {
            if (rotation == Rotation4.Up || rotation == Rotation4.Down)
                return new(shape.Width, shape.Height);
            else
                return new(shape.Height, shape.Width);
        }

        #endregion

        #region FindByType

        /// <summary>
        /// Looking for the first <see href="T"/>
        /// </summary>
        public static T FindByType<T>(this IEnumerable enumerable)
        {
            foreach (var t in enumerable)
            {
                if (t is T result)
                {
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// Looking for the first <see href="type"/>
        /// </summary>
        public static object FindByType(this IEnumerable enumerable, Type type)
        {
            foreach (var t in enumerable)
            {
                if (t.GetType() == type)
                {
                    return t;
                }
            }
            return null;
        }

        #endregion

        #region Fill

        /// <summary>
        /// Sets value to all elements in dictionary
        /// </summary>
        public static void Fill<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, params TKey[] except)
        {
            foreach (var pair in dictionary.ToArray())
            {
                if (except.Contains(pair.Key))
                    continue;

                dictionary[pair.Key] = value;
            }
        }

        /// <summary>
        /// Sets value to all elements in list
        /// </summary>
        public static void Fill<TValue>(this IList<TValue> list, TValue value, params int[] except)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (except.Contains(i))
                    continue;

                list[i] = value;
            }
        }

        #endregion

        #region GetAs

        /// <summary>
        /// Finds and returns
        /// </summary>
        public static IEnumerable<T> GetAs<T>(this IEnumerable enumerable)
        {
            return from object obj in enumerable
                   where obj is T
                   select (T)obj;
        }

        #endregion

        #region ForEach

        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        private class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion


        #region GetIfNull
        /// <summary>
        /// If null, causes <see href="GetComponent"/>, otherwise does nothing
        /// </summary>
        public static bool GetIfNull<T>(this T value, GameObject gameObject) where T : Component
        {
            ref T valueReference = ref value;
            return value == null && gameObject.TryGetComponent(out valueReference);
        }

        /// <summary>
        /// If null, causes <see href="GetComponent"/>, otherwise does nothing
        /// </summary>
        public static bool GetIfNull<T>(this GameObject gameObject, T value) where T : Component
        {
            ref T valueReference = ref value;
            return value == null && gameObject.TryGetComponent(out valueReference);
        }

        #endregion

        #region AddIfNotNull
        /// <summary>
        /// Add value if is not null
        /// </summary>
        public static bool AddIfNotNull<T>(this ICollection<T> collection, T value, bool checkContains = false)
        {
            if (collection != null && value != null)
            {
                if (checkContains && collection.Contains(value))
                    return false;

                collection.Add(value);
            }

            return value != null;
        }

        #endregion

        #region GetInRange

        public static T GetInRange<T>(this IReadOnlyList<T> list, int index)
        {
            return list[Math.Clamp(0, list.Count - 1, index)];
        }

        #endregion
    }
}
