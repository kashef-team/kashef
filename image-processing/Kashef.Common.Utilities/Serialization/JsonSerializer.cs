using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Serialization
{
    public class JsonSerializer : ISerializerProvider
    {
        public T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        public string Serialize<T>(T t)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, t);
            string jsonString = Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            memoryStream.Dispose();
            return jsonString;
        }
    }
}
