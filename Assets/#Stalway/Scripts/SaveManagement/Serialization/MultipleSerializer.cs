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
        public T Deserialize<T>(string value) => Serializer.Deserialize<T>(value);

        public UniTask<string> SerializeAsync(object value, CancellationToken token = default) => Serializer.SerializeAsync(value, token);
        public UniTask<object> DeserializeAsync(string value, CancellationToken token = default) => Serializer.DeserializeAsync(value, token);
        public UniTask<T> DeserializeAsync<T>(string value, CancellationToken token = default) => Serializer.DeserializeAsync<T>(value, token);

        public bool IsAvailable() => Serializer != null;
    }
}
