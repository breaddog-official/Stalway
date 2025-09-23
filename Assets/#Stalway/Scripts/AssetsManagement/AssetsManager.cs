using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Breaddog.AssetsManagement
{
    public readonly struct Asset<T> : IDisposable
    {
        public readonly T asset;
        public readonly string path;

        public Asset(T asset, string path)
        {
            this.asset = asset;
            this.path = path;
        }

        public void Dispose()
        {
            AssetsManager.Release<T>(path);
        }
    }

    public static class AssetsManager
    {
        /// <summary>
        /// Key: Internal path (Assets path) <br />
        /// Value: Absolute IO path
        /// </summary>
        public static readonly Dictionary<string, string> DedicatedAssets = new();

        /// <summary>
        /// Key: Assets path <br />
        /// Value: Count of links
        /// </summary>
        public static readonly Dictionary<string, int> LinksCount = new();
        public static readonly HashSet<AssetLoader> Loaders = new();

        private const string loaders_path = "";

        static AssetsManager()
        {
            var loadersFromResources = Resources.LoadAll<AssetLoader>(loaders_path);

            foreach (var loader in loadersFromResources)
                Loaders.Add(loader);
        }

        public static bool IsDedicated(in string key, out string dedicatedPath)
        {
            return DedicatedAssets.TryGetValue(key, out dedicatedPath);
        }

        public static string GetDedicatedPath(in string key)
        {
            return DedicatedAssets[key];
        }

        public static T Get<T>(in string path, AssetLoader loader = null)
        {
            T value;

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is empty");

            if (loader == null)
                loader = SelectLoaderByType(typeof(T));


            if (IsDedicated(path, out var dedicatedPath))
            {
                value = loader.GetDedicatedValue<T>(dedicatedPath);
            }
            else
            {
                value = loader.GetAssetsValue<T>(path);
            }

            if (LinksCount.ContainsKey(path))
                LinksCount[path] += 1;
            else
                LinksCount.Add(path, 1);

            return value;
        }

        public static async UniTask<T> GetAsync<T>(string path, AssetLoader loader = null, CancellationToken token = default)
        {
            T value;

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is empty");

            if (loader == null)
                loader = SelectLoaderByType(typeof(T));


            if (IsDedicated(path, out var overridedPath))
            {
                value = await loader.GetDedicatedValueAsync<T>(overridedPath, token);
            }
            else
            {
                value = await loader.GetAssetsValueAsync<T>(path, token);
            }

            if (LinksCount.ContainsKey(path))
                LinksCount[path] += 1;
            else
                LinksCount.Add(path, 1);

            return value;
        }



        public static Asset<T> GetAsset<T>(in string path, AssetLoader loader = null)
        {
            return new Asset<T>(Get<T>(path, loader), path);
        }

        public static async UniTask<Asset<T>> GetAssetAsync<T>(string path, AssetLoader loader = null, CancellationToken token = default)
        {
            return new Asset<T>(await GetAsync<T>(path, loader, token), path);
        }

        public static void Release<T>(in string path, AssetLoader loader = null)
        {
            int links = 0;

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is empty");

            if (!LinksCount.TryGetValue(path, out links))
                LinksCount.Add(path, 0);

            if (links == 1)
            {
                links--;
                LinksCount[path] = links;
            }
            else if (links > 1)
            {
                links--;
                LinksCount[path] = links;
                return;
            }
            else
            {
                return;
            }


            if (loader == null)
                loader = SelectLoaderByType(typeof(T));


            if (IsDedicated(path, out var overridedPath))
            {
                loader.UnloadDedicatedValue<T>(overridedPath);
            }
            else
            {
                loader.UnloadAssetsValue<T>(path);
            }
        }


        public static AssetLoader SelectLoaderByType(Type type)
        {
            foreach (var loader in Loaders)
            {
                if (loader.SupportType(type))
                    return loader;
            }

            throw new Exception($"Loader for {type} not founded");
        }
    }
}