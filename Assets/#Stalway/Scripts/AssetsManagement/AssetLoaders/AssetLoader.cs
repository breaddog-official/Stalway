using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Breaddog.AssetsManagement
{
    public abstract class AssetLoader : ScriptableObject
    {
        public abstract T GetAssetsValue<T>(string assetsPath);

        public abstract T GetDedicatedValue<T>(string absolutePath);


        public abstract void UnloadAssetsValue<T>(string assetsPath);

        public abstract void UnloadDedicatedValue<T>(string absolutePath);


        public abstract bool SupportType(Type type);


#pragma warning disable CS1998
        public virtual async UniTask<T> GetAssetsValueAsync<T>(string assetsPath, CancellationToken token = default) => GetAssetsValue<T>(assetsPath);
        public virtual async UniTask<T> GetDedicatedValueAsync<T>(string absolutePath, CancellationToken token = default) => GetDedicatedValue<T>(absolutePath);
#pragma warning restore CS1998
    }
}