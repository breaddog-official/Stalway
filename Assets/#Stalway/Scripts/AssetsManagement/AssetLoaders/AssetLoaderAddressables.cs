using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Breaddog.AssetsManagement
{
    public abstract class AssetLoaderAddressables : AssetLoader
    {
        private readonly Dictionary<string, object> loadedObjects = new();

        public override T GetAssetsValue<T>(string assetsPath)
        {
            if (loadedObjects.TryGetValue(assetsPath, out var obj))
                return (T)Convert.ChangeType(obj, typeof(T));

            T loaded = Addressables.LoadAssetAsync<T>(assetsPath).WaitForCompletion();
            loadedObjects.Add(assetsPath, loaded);

            return loaded;
        }

        public override async UniTask<T> GetAssetsValueAsync<T>(string assetsPath, CancellationToken token = default)
        {
            if (loadedObjects.TryGetValue(assetsPath, out var obj))
                return (T)Convert.ChangeType(obj, typeof(T));

            T loaded = await Addressables.LoadAssetAsync<T>(assetsPath).WithCancellation(token);
            loadedObjects.Add(assetsPath, loaded);

            return loaded;
        }

        public override void UnloadAssetsValue<T>(string assetsPath)
        {
            if (loadedObjects.TryGetValue(assetsPath, out var obj))
            {
                Addressables.Release((T)Convert.ChangeType(obj, typeof(T)));
                loadedObjects.Remove(assetsPath);
            }
        }
    }
}