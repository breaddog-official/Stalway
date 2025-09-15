using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    /// <summary>
    /// ������ ������� ������ ������� ������ ��� ��������� (��� ��������� � ������� ��� �� ��������� � T[,])
    /// </summary>
    [Serializable, BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
    public class Array2D<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, IReadOnlyArray2D<T>
    {
        [SerializeField, HideInInspector] private int width;
        [SerializeField, HideInInspector] private int height;
        [SerializeField, HideInInspector] private T[] data;

        public int Count => data.Length;
        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => width;
            set
            {
                if (value == width) return;
                Resize(value, height);
            }
        }


        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => height;
            set
            {
                if (value == height) return;
                Resize(width, value);
            }
        }

        public IReadOnlyList<T> RawData => data;
        public T[,] AsArray => ToArray();





        public Array2D(int width, int height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentOutOfRangeException("Width and Height must be positive");

            this.width = width;
            this.height = height;
            data = new T[width * height];
        }

        public Array2D(T[] data, int width, int height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentOutOfRangeException("Width and Height must be positive");

            this.width = width;
            this.height = height;
            this.data = data;
        }

        public Array2D(IReadOnlyArray2D<T> from)
        {
            width = from.Width;
            height = from.Height;

            int length = width * height;
            data = new T[length];

            for (int i = 0; i < length; i++)
                data[i] = from.RawData[i];
        }

        public Array2D(T[,] source)
        {
            height = source.GetLength(0);
            width = source.GetLength(1);
            data = new T[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    data[GetIndex(x, y)] = source[y, x];
                }
            }
        }





        public T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[GetIndex(x, y)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => data[GetIndex(x, y)] = value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(int x, int y)
            => (uint)x < (uint)Width && (uint)y < (uint)Height;

        public void Fill(Func<int, int, T> generator)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    this[x, y] = generator(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(int x, int y)
        {
            //if (!InBounds(x, y))
            //    throw new IndexOutOfRangeException($"Out of bounds: ({x}, {y})");

            return y * Width + x;
        }

        public void Resize(int newWidth, int newHeight)
        {
            var newData = new T[newWidth * newHeight];
            int copyWidth = Math.Min(width, newWidth);
            int copyHeight = Math.Min(height, newHeight);

            for (int y = 0; y < copyHeight; y++)
                Array.Copy(data, y * width, newData, y * newWidth, copyWidth);

            data = newData;
            width = newWidth;
            height = newHeight;
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result.Append(this[x, y]);
                    if (x < width - 1)
                        result.Append('\t');
                }
                result.AppendLine();
            }
            return result.ToString();
        }

        public T[,] ToArray()
        {
            var result = new T[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[y, x] = data[GetIndex(x, y)];
                }
            }
            return result;
        }

        #region Enumerator
        public Enumerator GetEnumerator() => new Enumerator(data);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _data;
            private int _index;

            public Enumerator(T[] data)
            {
                _data = data;
                _index = -1;
            }

            public bool MoveNext() => ++_index < _data.Length;
            public T Current => _data[_index];
            object IEnumerator.Current => Current;
            public void Reset() => throw new NotSupportedException();
            public void Dispose() { }
        }

        #endregion
    }

    public interface IReadOnlyArray2D<out T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
        int Width { get; }
        int Height { get; }
        IReadOnlyList<T> RawData { get; }
        T this[int x, int y] { get; }
    }

    /// <summary>
    /// ������ ������� ������ ������� ������ ��� ��������� (��� ��������� � ������� ��� �� ��������� � T[,,])
    /// </summary>
    [Serializable, BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
    public class Array3D<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, IReadOnlyArray3D<T>
    {
        [SerializeField, HideInInspector] private T[] data;
        [SerializeField, HideInInspector] private int width;
        [SerializeField, HideInInspector] private int height;
        [SerializeField, HideInInspector] private int depth;


        public int Count => data.Length;
        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => width;
            set => Resize(value, height, depth);
        }

        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => height;
            set => Resize(width, value, depth);
        }

        public int Depth
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => depth;
            set => Resize(width, height, value);
        }
        public IReadOnlyList<T> RawData => data;
        public T[,,] AsArray => ToArray();

        public Array3D(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            data = new T[width * height * depth];
        }

        public Array3D(in Array3D<T> from)
        {
            width = from.width;
            height = from.height;
            depth = from.depth;
            from.data.CopyTo(data, 0);
        }

        public Array3D(T[,,] source)
        {
            depth = source.GetLength(0);
            height = source.GetLength(1);
            width = source.GetLength(2);
            data = new T[width * height * depth];

            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        data[GetIndex(x, y, z)] = source[z, y, x];
                    }
                }
            }
        }

        public T this[int x, int y, int z]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[GetIndex(x, y, z)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => data[GetIndex(x, y, z)] = value;
        }

        public void Resize(int newWidth, int newHeight, int newDepth)
        {
            var newData = new T[newWidth * newHeight * newDepth];
            int copyWidth = Math.Min(width, newWidth);
            int copyHeight = Math.Min(height, newHeight);
            int copyDepth = Math.Min(depth, newDepth);

            for (int z = 0; z < copyDepth; z++)
            {
                for (int y = 0; y < copyHeight; y++)
                {
                    int oldIndex = z * width * height + y * width;
                    int newIndex = z * newWidth * newHeight + y * newWidth;
                    Array.Copy(data, oldIndex, newData, newIndex, copyWidth);
                }
            }

            width = newWidth;
            height = newHeight;
            depth = newDepth;
            data = newData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(int x, int y, int z) =>
            (uint)x < (uint)width && (uint)y < (uint)height && (uint)z < (uint)depth;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(int x, int y, int z)
        {
            //if (!InBounds(x, y))
            //    throw new IndexOutOfRangeException($"Out of bounds: ({x}, {y})");

            return z * width * height + y * width + x;
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();

            for (int z = 0; z < Depth; z++)
            {
                result.AppendLine($"Layer Z = {z}:");
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        result.Append(this[x, y, z]);
                        if (x < Width - 1)
                            result.Append('\t');
                    }
                    result.AppendLine();
                }
                result.AppendLine(); // ����������� ����� ������
            }

            return result.ToString();
        }

        public T[,,] ToArray()
        {
            var result = new T[depth, height, width];
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        result[z, y, x] = data[GetIndex(x, y, z)];
                    }
                }
            }
            return result;
        }

        #region Enumerator
        public Enumerator GetEnumerator() => new Enumerator(data);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _data;
            private int _index;

            public Enumerator(T[] data)
            {
                _data = data;
                _index = -1;
            }

            public bool MoveNext() => ++_index < _data.Length;
            public T Current => _data[_index];
            object IEnumerator.Current => Current;
            public void Reset() => throw new NotSupportedException();
            public readonly void Dispose() { }
        }

        #endregion
    }

    public interface IReadOnlyArray3D<out T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
        int Width { get; }
        int Height { get; }
        int Depth { get; }
        IReadOnlyList<T> RawData { get; }
        T this[int x, int y, int z] { get; }
    }
}
