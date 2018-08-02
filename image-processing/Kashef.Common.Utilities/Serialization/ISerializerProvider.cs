using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashef.Common.Utilities.Serialization
{
    public interface ISerializerProvider
    {
        T JsonDeserialize<T>(string jsonString);

        string Serialize<T>(T t);
    }
}
