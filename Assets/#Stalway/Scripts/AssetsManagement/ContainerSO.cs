using NaughtyAttributes;
using UnityEngine;

namespace Breaddog.AssetsManagement
{
    /// <summary>
    /// �����-������, ����������� ������� ������� ���� � ScriptableObject
    /// </summary>
    public class ContainerSO<T> : ScriptableObject
    {
        [field: SerializeReference]
        public virtual T Value { get; private set; }

#if UNITY_EDITOR
        /// <summary>
        /// Иногда изменение ассета (например Item) не полностью сохраняется без вызова SaveAssets, поэтому мы делаем кнопку для быстрого сохранения ассета.
        /// </summary>
        [Button]
        protected void SaveAssets()
        {
            UnityEditor.AssetDatabase.SaveAssets();
        }

#endif
    }
}