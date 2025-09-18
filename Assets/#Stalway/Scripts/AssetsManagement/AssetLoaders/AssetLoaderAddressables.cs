using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Breaddog.AssetsManagement
{
    /// <summary>
    /// ��������� �� ������� ������ ������� ����. �������� ��� ����������� ��������, ����� ��� Audio ��� Texture
    /// </summary>
    public abstract class AssetLoaderAddressables : AssetLoader
    {
        private readonly Dictionary<string, object> loadedObjects = new();

        public override T GetAssetsValue<T>(string assetsPath)
        {
            //var type = typeof(T);
            //var loaded = Resources.Load(assetsPath, type);
            T loaded = Addressables.LoadAssetAsync<T>(assetsPath).WaitForCompletion();
            loadedObjects.Add(assetsPath, loaded);

            return loaded;//(T)Convert.ChangeType(loaded, type);
        }

        public override async UniTask<T> GetAssetsValueAsync<T>(string assetsPath, CancellationToken token = default)
        {
            //var type = typeof(T);
            //var loaded = await Resources.LoadAsync(assetsPath, type).WithCancellation(token);
            T loaded = await Addressables.LoadAssetAsync<T>(assetsPath).WithCancellation(token);
            loadedObjects.Add(assetsPath, loaded);

            return loaded;//(T)Convert.ChangeType(loaded, type);
        }

        public override void UnloadAssetsValue<T>(string assetsPath)
        {
            //var loaded = Resources.Load(assetsPath, type); // ���� ����� ��� ��� ��������, �� �������� �� ����

            //Resources.UnloadAsset(loaded);
            if (loadedObjects.TryGetValue(assetsPath, out var obj))
            {
                Addressables.Release(obj);
                loadedObjects.Remove(assetsPath);
            }
        }
    }
}