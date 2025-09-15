using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    /// <summary>
    /// ����� ����������� ������������� ������� � ������������� �����-���� ������. <br />
    /// �� �����������, ��� ���� � ��� �� ID ������� �� ����� ��������������.
    /// </summary>
    [Serializable, BurstCompile]
    public class Database<T>
    {
        // ��������� ������ �� ����������� ID
        [SerializeField, HideInInspector] private Dictionary<ulong, T> data = new();

        // ��������� ��������� ID. �� �������� � 1 ����� ���������������� ������ 0 ��� �������� null � ����������
        [SerializeField, HideInInspector] private ulong nextIndex = 1;

        public T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[index];
        }

        /// <summary>
        /// ��������� ������� � ���� � ���������� ��� ���������� ID.
        /// </summary>
        public ulong Add(T item)
        {
            var id = nextIndex;

            // ������ ���������� ���� id ��� ���� � �������
            data.Add(id, item);

            // ���� ���������� ���, ����� ����� ��������� nextIndex
            nextIndex++;

            return id;
        }

        /// <summary>
        /// �������� ������� �� ID, ���� �� ����������.
        /// </summary>
        public bool TryGet(ulong id, out T item)
        {
            return data.TryGetValue(id, out item);
        }

        /// <summary>
        /// ������� ������� �� ID.
        /// </summary>
        public bool Remove(ulong id)
        {
            return data.Remove(id);
        }

        /// <summary>
        /// ��������� ������� ID.
        /// </summary>
        public bool Contains(ulong id)
        {
            return data.ContainsKey(id);
        }

        /// <summary>
        /// �������� ��� ��������.
        /// </summary>
        public IReadOnlyDictionary<ulong, T> GetData()
        {
            return data;
        }


        public override string ToString()
        {
            return data.ToString();
        }
    }
}