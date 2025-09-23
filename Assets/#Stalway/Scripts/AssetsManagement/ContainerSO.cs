using NaughtyAttributes;
using UnityEngine;

namespace Breaddog.AssetsManagement
{
    public abstract class ContainerSO<T> : ScriptableObject
    {
        public abstract T Value { get; }

        /*#if UNITY_EDITOR
                /// <summary>
                /// Иногда изменение ассета (например Item) не полностью сохраняется без вызова SaveAssets, поэтому мы делаем кнопку для быстрого сохранения ассета.
                /// </summary>
                [Button]
                protected void SaveAssets()
                {
                    UnityEditor.AssetDatabase.SaveAssets();
                }

        #endif*/
    }
}