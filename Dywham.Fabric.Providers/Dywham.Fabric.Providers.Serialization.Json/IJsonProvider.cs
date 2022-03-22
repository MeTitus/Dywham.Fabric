using System;
using Newtonsoft.Json.Linq;

namespace Dywham.Fabric.Providers.Serialization.Json
{
    public interface IJsonProvider : IProvider
    {
        public JToken ParseJson(string text);

        string Serialize(object graph);

        T Deserialize<T>(string data);

        object Deserialize(string data, Type type);

        void SerializeToFile(object graph, string filename);

        object DeserializeFromFile(string filename, Type type);

        T DeserializeFromFile<T>(string filename);
    }
}