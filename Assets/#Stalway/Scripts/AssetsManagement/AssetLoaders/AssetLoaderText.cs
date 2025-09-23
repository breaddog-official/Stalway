using Cysharp.Threading.Tasks;
using Breaddog.SaveManagement;
using System;
using System.Threading;
using UnityEngine;

namespace Breaddog.AssetsManagement
{
    public class AssetLoaderText<TText> : AssetLoaderContainer
    {
        public IStringSaver Saver = new SaverIO();
        public IStringSerializer Serializer = new SerializerJson();

        public override T GetDedicatedValue<T>(string absolutePath)
        {
            var type = typeof(TText);
            var loaded = SaveManager.LoadAndDeserialize<T>(absolutePath, Saver, Serializer);

            return (T)Convert.ChangeType(loaded, type);
        }

        public override async UniTask<T> GetDedicatedValueAsync<T>(string absolutePath, CancellationToken token = default)
        {
            var type = typeof(TText);
            var loaded = await SaveManager.LoadAndDeserializeAsync<TText>(absolutePath, Saver, Serializer);

            return (T)Convert.ChangeType(loaded, type);
        }

        public override bool SupportType(Type type)
        {
            return type == typeof(TText);
        }

        public override void UnloadDedicatedValue<T>(string assetsPath) { }
    }
}