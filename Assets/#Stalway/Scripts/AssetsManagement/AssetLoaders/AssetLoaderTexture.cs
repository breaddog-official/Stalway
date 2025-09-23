using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Breaddog.AssetsManagement
{
    [CreateAssetMenu(fileName = "TextureAssetLoader", menuName = "Stalway/Asset Loaders/Texture")]
    public class AssetLoaderTexture : AssetLoaderAddressables
    {
        private readonly Dictionary<string, Texture2D> loadedTextures = new();
        private readonly Dictionary<string, Sprite> loadedSprites = new();

        public override T GetDedicatedValue<T>(string absolutePath)
        {
            if (loadedTextures.TryGetValue(absolutePath, out var loadedTex))
                return (T)Convert.ChangeType(loadedTex, typeof(T));

            else if (typeof(T) == typeof(Sprite) && loadedSprites.TryGetValue(absolutePath, out var loadedSprite))
                return (T)Convert.ChangeType(loadedSprite, typeof(T));

            Texture2D tex = null;
            string uri = "file://" + absolutePath;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
            {
                www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error loading texture from {absolutePath}: {www.error}");
                }
                else
                {
                    tex = DownloadHandlerTexture.GetContent(www);
                    if (tex == null)
                    {
                        Debug.LogError("Failed to load Texture2D from downloaded data.");
                        return default;
                    }
                }
            }

            loadedTextures.Add(absolutePath, tex);
            return (T)Convert.ChangeType(tex, typeof(T));
        }

        public override async UniTask<T> GetDedicatedValueAsync<T>(string absolutePath, CancellationToken token = default)
        {
            if (loadedTextures.TryGetValue(absolutePath, out var loadedTex))
                return (T)Convert.ChangeType(loadedTex, typeof(T));

            else if (typeof(T) == typeof(Sprite) && loadedSprites.TryGetValue(absolutePath, out var loadedSprite))
                return (T)Convert.ChangeType(loadedSprite, typeof(T));

            Texture2D tex = null;
            string uri = "file://" + absolutePath;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error loading texture from {absolutePath}: {www.error}");
                }
                else
                {
                    tex = DownloadHandlerTexture.GetContent(www);
                    if (tex == null)
                    {
                        Debug.LogError("Failed to load Texture2D from downloaded data.");
                        return default;
                    }
                }
            }

            loadedTextures.Add(absolutePath, tex);

            if (typeof(T) == typeof(Sprite))
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                loadedSprites.Add(absolutePath, sprite);
                return (T)Convert.ChangeType(sprite, typeof(T));
            }

            return (T)Convert.ChangeType(tex, typeof(T));
        }

        public override bool SupportType(Type type)
        {
            return type == typeof(Texture2D) || type == typeof(Sprite);
        }

        public override void UnloadDedicatedValue<T>(string absolutePath)
        {
            if (loadedSprites.TryGetValue(absolutePath, out var sprite))
            {
                Destroy(sprite);
                loadedSprites.Remove(absolutePath);
            }

            if (loadedTextures.TryGetValue(absolutePath, out var texture))
            {
                Destroy(texture);
                loadedTextures.Remove(absolutePath);
            }
        }
    }
}
