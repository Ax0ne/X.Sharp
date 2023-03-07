using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Sharp.Redis
{
    public class NewtonsoftSerializer : ISerializer
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public byte[] Serialize<T>(T? item)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(byte[] serializedObject)
        {
            throw new NotImplementedException();
        }
    }
}