using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Breaddog.SaveManagement
{
    [Serializable]
    public class MultipleStringSerializer : IStringSerializer
    {
        public IStringSerializer[] serializers;

        public IStringSerializer Serializer
        {
            get
            {
                foreach (var saver in serializers)
                    if (saver.IsAvailable())
                        return saver;

                return null;
            }
        }

        public string Serialize(object value) => Serializer.Serialize(value);
        public object Deserialize(string value) => Serializer.Deserialize(value);
        public string SerializeType<T>(T value) => Serializer.SerializeType<T>(value);
        public T DeserializeType<T>(string value) => Serializer.DeserializeType<T>(value);

        public UniTask<string> SerializeAsync(object value, CancellationToken token = default) => Serializer.SerializeAsync(value, token);
        public UniTask<object> DeserializeAsync(string value, CancellationToken token = default) => Serializer.DeserializeAsync(value, token);
        public UniTask<string> SerializeTypeAsync<T>(T value, CancellationToken token = default) => Serializer.SerializeTypeAsync<T>(value, token);
        public UniTask<T> DeserializeTypeAsync<T>(string value, CancellationToken token = default) => Serializer.DeserializeTypeAsync<T>(value, token);

        public bool IsAvailable() => Serializer != null;
    }
}
