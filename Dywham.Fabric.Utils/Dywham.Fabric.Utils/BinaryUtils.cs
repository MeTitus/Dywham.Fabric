using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dywham.Fabric.Utils
{
#pragma warning disable SYSLIB0011
#pragma warning disable CS0618
    public static class BinaryUtils
    {
        public static byte[] Serialize(object o)
        {
            var fmt = new BinaryFormatter();
            byte[] data;

            using (var ms = new MemoryStream())
            {
                fmt.Serialize(ms, o);

                data = ms.GetBuffer();
            }

            return data;
        }

        public static object Deserialize(byte[] data)
        {
            var fmt = new BinaryFormatter();

            return fmt.Deserialize(new MemoryStream(data));
        }

        public static void SaveToFile(string path, byte[] data, byte[] signature)
        {
            using (var f = new FileStream(path, FileMode.Create))
            {
                SaveToStream(f, data, signature);
            }
        }

        public static Stream SaveToStream(Stream stream, byte[] data, byte[] signature)
        {
            var fmt = new BinaryFormatter();

            fmt.Serialize(stream, data);
            fmt.Serialize(stream, signature);

            return stream;
        }

        public static void LoadFromFile(string path, out byte[] data, out byte[] signature)
        {
            var fmt = new BinaryFormatter();

            using (var f = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = (byte[])fmt.Deserialize(f);
                signature = (byte[])fmt.Deserialize(f);
            }
        }
    }
#pragma warning restore CS0618
#pragma warning restore SYSLIB0011
}