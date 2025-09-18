using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Breaddog.AssetsManagement
{
    [CreateAssetMenu(fileName = "AudioAssetLoader", menuName = "Stalway/Asset Loaders/Audio")]
    public class AssetLoaderAudio : AssetLoaderAddressables
    {
        private readonly Dictionary<string, AudioClip> loadedClips = new();

        public override T GetDedicatedValue<T>(string absolutePath)
        {
            AudioClip clip = null;
            string uri = "file://" + absolutePath;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN))
            {
                www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error loading audio from {absolutePath}: {www.error}");
                }
                else
                {
                    clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip == null)
                    {
                        Debug.LogError("Failed to load AudioClip from downloaded data.");
                        return default;
                    }
                }
            }

            loadedClips.Add(absolutePath, clip);
            return (T)Convert.ChangeType(clip, typeof(T));
        }

        public override async UniTask<T> GetDedicatedValueAsync<T>(string absolutePath, CancellationToken token = default)
        {
            AudioClip clip = null;
            string uri = "file://" + absolutePath;

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN))
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error loading audio from {absolutePath}: {www.error}");
                }
                else
                {
                    clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip == null)
                    {
                        Debug.LogError("Failed to load AudioClip from downloaded data.");
                        return default;
                    }
                }
            }

            loadedClips.Add(absolutePath, clip);
            return (T)Convert.ChangeType(clip, typeof(T));
        }

        public override bool SupportType(Type type)
        {
            return type == typeof(AudioClip);
        }

        public override void UnloadDedicatedValue<T>(string absolutePath)
        {
            if (loadedClips.TryGetValue(absolutePath, out var clip))
            {
                Destroy(clip);
                loadedClips.Remove(absolutePath);
            }
        }
    }
}
