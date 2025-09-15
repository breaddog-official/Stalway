using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace Breaddog.Extensions
{
    /// <summary>
    /// Обёртка которая хранит обычный массив как двумерный (даёт ускорение в десятки раз по сравнению с T[,])
    /// </summary>
    [BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
    public struct NativeArray2D<T> : IDisposable where T : struct
    {
        public NativeArray<T> Data;
        private int width;
        private int height;

        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => width;
            set => Resize(value, height);
        }

        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => height;
            set => Resize(width, value);
        }

        private readonly Allocator allocator;

        public NativeArray2D(int width, int height, Allocator allocator)
        {
            this.width = width;
            this.height = height;
            this.allocator = allocator;
            Data = new NativeArray<T>(width * height, allocator, NativeArrayOptions.ClearMemory);
        }

        public void Resize(int newWidth, int newHeight, bool copyOldData = true)
        {
            var newData = new NativeArray<T>(newWidth * newHeight, allocator, NativeArrayOptions.ClearMemory);

            if (copyOldData)
            {
                int copyWidth = Math.Min(width, newWidth);
                int copyHeight = Math.Min(height, newHeight);

                for (int y = 0; y < copyHeight; y++)
                {
                    int oldBase = y * width;
                    int newBase = y * newWidth;
                    NativeArray<T>.Copy(Data, oldBase, newData, newBase, copyWidth);
                }
            }

            if (Data.IsCreated)
                Data.Dispose();

            Data = newData;
            width = newWidth;
            height = newHeight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetIndex(int x, int y) => y * width + x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool InBounds(int x, int y) =>
            (uint)x < (uint)width && (uint)y < (uint)height;

        public T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => Data[GetIndex(x, y)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Data[GetIndex(x, y)] = value;
        }

        public void Dispose()
        {
            if (Data.IsCreated)
                Data.Dispose();
        }

        public override string ToString()
        {
            if (!Data.IsCreated)
                return "Array is not created.";

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
    }

    /// <summary>
    /// Обёртка которая хранит обычный массив как трёхмерный (даёт ускорение в десятки раз по сравнению с T[,,])
    /// </summary>
    [BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
    public struct NativeArray3D<T> : IDisposable where T : struct
    {
        public NativeArray<T> Data;
        private int width;
        private int height;
        private int depth;

        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => width;
            set => Resize(value, height, depth);
        }

        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => height;
            set => Resize(width, value, depth);
        }

        public int Depth
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => depth;
            set => Resize(width, height, value);
        }

        private readonly Allocator allocator;

        public NativeArray3D(int width, int height, int depth, Allocator allocator)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.allocator = allocator;
            Data = new NativeArray<T>(width * height * depth, allocator, NativeArrayOptions.ClearMemory);
        }

        public void Resize(int newWidth, int newHeight, int newDepth, bool copyOldData = true)
        {
            var newData = new NativeArray<T>(newWidth * newHeight * newDepth, allocator, NativeArrayOptions.ClearMemory);

            if (copyOldData)
            {
                int copyWidth = Math.Min(Width, newWidth);
                int copyHeight = Math.Min(Height, newHeight);
                int copyDepth = Math.Min(Depth, newDepth);

                for (int z = 0; z < copyDepth; z++)
                {
                    for (int y = 0; y < copyHeight; y++)
                    {
                        int oldBase = z * Width * Height + y * Width;
                        int newBase = z * newWidth * newHeight + y * newWidth;
                        NativeArray<T>.Copy(Data, oldBase, newData, newBase, copyWidth);
                    }
                }
            }

            if (Data.IsCreated)
                Data.Dispose();

            Data = newData;
            width = newWidth;
            height = newHeight;
            depth = newDepth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetIndex(int x, int y, int z) => z * Width * Height + y * Width + x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool InBounds(int x, int y, int z) =>
            (uint)x < (uint)Width && (uint)y < (uint)Height && (uint)z < (uint)Depth;

        public T this[int x, int y, int z]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => Data[GetIndex(x, y, z)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Data[GetIndex(x, y, z)] = value;
        }

        public void Dispose()
        {
            if (Data.IsCreated)
                Data.Dispose();
        }

        public override string ToString()
        {
            if (!Data.IsCreated)
                return "Array is not created.";

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
                result.AppendLine(); // Разделитель между слоями
            }

            return result.ToString();
        }

    }
}
