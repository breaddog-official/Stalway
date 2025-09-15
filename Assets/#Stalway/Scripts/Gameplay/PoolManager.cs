using Unity.Burst;
using UnityEngine;

namespace Breaddog.Gameplay
{
    /// <summary>
    /// Класс который кеширует объекты, чтобы лишний раз их не инстанциировать
    /// </summary>
    [BurstCompile]
    public class PoolManager
    {
        // Заглушка
        public T Spawn<T>(T obj, Vector3 position, Quaternion rotation) where T : Object
        {
            return Object.Instantiate(obj, position, rotation);
        }
    }
}
