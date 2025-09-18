using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;

namespace Breaddog.SaveManagement
{
    [Serializable]
    public class SerializerJson : IStringSerializer
    {
        enum NamingStraregy
        {
            [Tooltip("camelCase")]
            CamelCase,
            [Tooltip("kebab-case")]
            KebabCase,
            [Tooltip("snake_case")]
            SnakeCase
        }

        [SerializeField] private Formatting formatting;
        [SerializeField] private NamingStraregy namingStrategy;

        public JsonSerializerSettings SerializerSettings => new()
        {
            Formatting = formatting,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = namingStrategy switch
                {
                    NamingStraregy.CamelCase => new CamelCaseNamingStrategy(),
                    NamingStraregy.KebabCase => new KebabCaseNamingStrategy(),
                    NamingStraregy.SnakeCase => new SnakeCaseNamingStrategy(),
                    _ => throw new NotImplementedException()
                }
            },
        };


        public virtual string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        public virtual object Deserialize(string value)
        {
            return JsonConvert.DeserializeObject(value, SerializerSettings);
        }

        public virtual T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }
    }
}
