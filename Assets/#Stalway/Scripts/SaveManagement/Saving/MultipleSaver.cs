using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Breaddog.SaveManagement
{
    [Serializable]
    public class MultipleStringSaver : IStringSaver
    {
        public IStringSaver[] savers;

        public IStringSaver Saver
        {
            get
            {
                foreach (var saver in savers)
                    if (saver.IsAvailable())
                        return saver;

                return null;
            }
        }


        public void Save(string path, string value) => Saver.Save(path, value);
        public string Load(string path) => Saver.Load(path);


        public bool Exists(string path) => Saver.Exists(path);
        public bool IsAvailable() => Saver != null;


        public async UniTask SaveAsync(string path, string value, CancellationToken token = default) => await Saver.SaveAsync(path, value, token);
        public async UniTask<string> LoadAsync(string path, CancellationToken token = default) => await Saver.LoadAsync(path, token);
    }

    public class MultipleBytesSaver : IBytesSaver
    {
        public IBytesSaver[] savers;

        public IBytesSaver Saver
        {
            get
            {
                foreach (var saver in savers)
                    if (saver.IsAvailable())
                        return saver;

                return null;
            }
        }


        public void SaveBytes(string path, byte[] value) => Saver.SaveBytes(path, value);
        public byte[] LoadBytes(string path) => Saver.LoadBytes(path);


        public bool Exists(string path) => Saver.Exists(path);
        public bool IsAvailable() => Saver != null;


        public async UniTask SaveBytesAsync(string path, byte[] value, CancellationToken token = default) => await Saver.SaveBytesAsync(path, value, token);
        public async UniTask<byte[]> LoadBytesAsync(string path, CancellationToken token = default) => await Saver.LoadBytesAsync(path, token);
    }
}
