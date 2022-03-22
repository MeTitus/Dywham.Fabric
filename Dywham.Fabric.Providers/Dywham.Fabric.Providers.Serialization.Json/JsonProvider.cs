using System;
using System.IO;
using Dywham.Fabric.Providers.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dywham.Fabric.Providers.Serialization.Json
{
    public class JsonProvider : IJsonProvider
    {
        // ReSharper disable once InconsistentNaming
        public IIOProvider IOProvider { get; set; }


        public string Serialize(object graph)
        {
            return JsonConvert.SerializeObject(graph, Formatting.None);
        }

        public void SerializeToFile(object graph, string filename)
        {
            var text = Serialize(graph);

            IOProvider.WriteAllText(filename, text);
        }

        public T Deserialize<T>(string data)
        {
            return (T)Deserialize(data, typeof(T));
        }

        public object Deserialize(string data, Type type)
        {
            return string.IsNullOrEmpty(data) ? default : JsonConvert.DeserializeObject(data, type);
        }

        public object DeserializeFromFile(string filename, Type type)
        {
            var text = IOProvider.ReadAllText(filename);

            return Deserialize(text, type);
        }

        public T DeserializeFromFile<T>(string filename)
        {
            var text = IOProvider.ReadAllText(filename);

            return (T)Deserialize(text, typeof(T));
        }

        public JToken ParseJson(string text)
        {
            using (var reader = new JsonTextReader(new StringReader(text)))
            {
                reader.DateParseHandling = DateParseHandling.None;

                return JToken.Load(reader);
            }
        }
    }
}