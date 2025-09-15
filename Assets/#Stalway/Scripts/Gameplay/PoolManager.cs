using Unity.Burst;
using UnityEngine;

namespace Breaddog.Gameplay
{
    /// <summary>
    /// ����� ������� �������� �������, ����� ������ ��� �� �� ���������������
    /// </summary>
    [BurstCompile]
    public class PoolManager
    {
        // ��������
        public T Spawn<T>(T obj, Vector3 position, Quaternion rotation) where T : Object
        {
            return Object.Instantiate(obj, position, rotation);
        }
    }
}
