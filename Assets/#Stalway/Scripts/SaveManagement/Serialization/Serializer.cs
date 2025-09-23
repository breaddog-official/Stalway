using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Breaddog.SaveManagement
{
    public interface IStringSerializer
    {
        public string Serialize(object value);
        public object Deserialize(string value);

        public virtual string SerializeType<T>(T value) => Serialize(value);
        public virtual T DeserializeType<T>(string value) => (T)Deserialize(value);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask<string> SerializeAsync(object value, CancellationToken token = default) => Serialize(value);
        public async virtual UniTask<string> SerializeTypeAsync<T>(T value, CancellationToken token = default) => SerializeType<T>(value);
        public async virtual UniTask<object> DeserializeAsync(string value, CancellationToken token = default) => Deserialize(value);
        public async virtual UniTask<T> DeserializeTypeAsync<T>(string value, CancellationToken token = default) => DeserializeType<T>(value);

#pragma warning restore CS1998
    }

    public interface IBytesSerializer
    {
        public byte[] SerializeBytes(object value);
        public object DeserializeBytes(byte[] value);

        public virtual byte[] SerializeBytesType<T>(T value) => SerializeBytes(value);
        public virtual T DeserializeBytesType<T>(byte[] value) => (T)DeserializeBytes(value);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask<byte[]> SerializeBytesAsync(object value, CancellationToken token = default) => SerializeBytes(value);
        public async virtual UniTask<byte[]> SerializeBytesTypeAsync<T>(T value, CancellationToken token = default) => SerializeBytesType<T>(value);
        public async virtual UniTask<object> DeserializeBytesAsync(byte[] value, CancellationToken token = default) => DeserializeBytes(value);
        public async virtual UniTask<T> DeserializeBytesTypeAsync<T>(byte[] value, CancellationToken token = default) => DeserializeBytesType<T>(value);

#pragma warning restore CS1998
    }
}
