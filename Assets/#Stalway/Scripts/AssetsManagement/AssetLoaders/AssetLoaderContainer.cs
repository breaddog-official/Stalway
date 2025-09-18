using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Breaddog.AssetsManagement
{
    /// <summary>
    /// ��������� ContainerSO, ���������� ������ ���. �������� ��� �����, ������� ����� ��������� ������ � ScriptableObject (�������� �������)
    /// </summary>
    public abstract class AssetLoaderContainer : AssetLoader
    {
        private readonly Dictionary<string, object> loadedObjects = new();

        public override T GetAssetsValue<T>(string assetsPath)
        {
            var loaded = Addressables.LoadAssetAsync<ContainerSO<T>>(assetsPath).WaitForCompletion(); //Resources.Load<ContainerSO<T>>(assetsPath).Value;
            loadedObjects.Add(assetsPath, loaded);

            return loaded.Value;
        }

        public override async UniTask<T> GetAssetsValueAsync<T>(string assetsPath, CancellationToken token = default)
        {
            //var type = typeof(T);
            var loaded = await Addressables.LoadAssetAsync<ContainerSO<T>>(assetsPath).WithCancellation(token);//await Resources.LoadAsync<ContainerSO<T>>(assetsPath).WithCancellation(token);
            loadedObjects.Add(assetsPath, loaded);

            return loaded.Value;//(T)Convert.ChangeType(loaded, type);
        }

        public override void UnloadAssetsValue<T>(string assetsPath)
        {
            //var loaded = Resources.Load<ContainerSO<T>>(assetsPath); // ���� ����� ��� ��� ��������, �� �������� �� ����

            //Resources.UnloadAsset(loaded);
            if (loadedObjects.TryGetValue(assetsPath, out var obj))
            {
                Addressables.Release(obj);
                loadedObjects.Remove(assetsPath);
            }
        }
    }
}