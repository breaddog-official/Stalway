using System.Threading;
using Cysharp.Threading.Tasks;

namespace Breaddog.SaveManagement
{
    public interface IStringHasher
    {
        public ulong Hash(string value);
        public virtual ulong Hash(IHashable value) => Hash(value.GetHashString());

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask<ulong> HashAsync(string value, CancellationToken token = default) => Hash(value);
        public async virtual UniTask<ulong> HashAsync(IHashable value, CancellationToken token = default) => Hash(value);

#pragma warning restore CS1998
    }

    public interface IBytesHasher
    {
        public ulong HashBytes(byte[] value);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask<ulong> HashBytesAsync(byte[] value, CancellationToken token = default) => HashBytes(value);

#pragma warning restore CS1998
    }
}
