using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Threading;

namespace Breaddog.SaveManagement
{
    [Serializable]
    public class SaverIO : IStringSaver, IBytesSaver
    {
        public bool Exists(string path) => File.Exists(path);
        public bool IsAvailable() => SaveManager.SupportIO;


        #region String Saver
        public void Save(string path, string value)
        {
            File.WriteAllText(path, value);
        }

        public string Load(string path)
        {
            return File.ReadAllText(path);
        }


        public async UniTask SaveAsync(string path, string value, CancellationToken token = default)
        {
            await File.WriteAllTextAsync(path, value, token);
        }

        public async UniTask<string> LoadAsync(string path, CancellationToken token = default)
        {
            return await File.ReadAllTextAsync(path, token);
        }

        #endregion

        #region Bytes Saver
        public void SaveBytes(string path, byte[] value)
        {
            File.WriteAllBytes(path, value);
        }

        public byte[] LoadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }


        public async UniTask SaveBytesAsync(string path, byte[] value, CancellationToken token = default)
        {
            await File.WriteAllBytesAsync(path, value, token);
        }

        public async UniTask<byte[]> LoadBytesAsync(string path, CancellationToken token = default)
        {
            return await File.ReadAllBytesAsync(path, token);
        }

        #endregion
    }
}
