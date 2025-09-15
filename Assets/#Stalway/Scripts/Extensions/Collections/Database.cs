using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    /// <summary>
    /// Класс позволяющий автоматически хранить и индексировать какие-либо классы. <br />
    /// Он гарантирует, что один и тот же ID никогда не будет использоваться.
    /// </summary>
    [Serializable, BurstCompile]
    public class Database<T>
    {
        // Хранилище данных по уникальному ID
        [SerializeField, HideInInspector] private Dictionary<ulong, T> data = new();

        // Следующий доступный ID. Мы начинаем с 1 чтобы зарезезрвировать индекс 0 как значение null в переменных
        [SerializeField, HideInInspector] private ulong nextIndex = 1;

        public T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[index];
        }

        /// <summary>
        /// Добавляет элемент в базу и возвращает его уникальный ID.
        /// </summary>
        public ulong Add(T item)
        {
            var id = nextIndex;

            // Выдаст исключение если id уже есть в словаре
            data.Add(id, item);

            // Если исключения нет, можем смело добавлять nextIndex
            nextIndex++;

            return id;
        }

        /// <summary>
        /// Получить элемент по ID, если он существует.
        /// </summary>
        public bool TryGet(ulong id, out T item)
        {
            return data.TryGetValue(id, out item);
        }

        /// <summary>
        /// Удалить элемент по ID.
        /// </summary>
        public bool Remove(ulong id)
        {
            return data.Remove(id);
        }

        /// <summary>
        /// Проверить наличие ID.
        /// </summary>
        public bool Contains(ulong id)
        {
            return data.ContainsKey(id);
        }

        /// <summary>
        /// Получить все элементы.
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