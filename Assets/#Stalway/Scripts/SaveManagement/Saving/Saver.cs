using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Breaddog.SaveManagement
{
    public interface IStringSaver
    {
        public abstract void Save(string path, string value);
        public abstract string Load(string path);
        public abstract bool Exists(string path);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask SaveAsync(string path, string value, CancellationToken token = default) => Save(path, value);
        public async virtual UniTask<string> LoadAsync(string path, CancellationToken token = default) => Load(path);

#pragma warning restore CS1998
    }

    public interface IBytesSaver
    {
        public abstract void SaveBytes(string path, byte[] value);
        public abstract byte[] LoadBytes(string path);
        public abstract bool Exists(string path);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask SaveBytesAsync(string path, byte[] value, CancellationToken token = default) => SaveBytes(path, value);
        public async virtual UniTask<byte[]> LoadBytesAsync(string path, CancellationToken token = default) => LoadBytes(path);

#pragma warning restore CS1998
    }
}
