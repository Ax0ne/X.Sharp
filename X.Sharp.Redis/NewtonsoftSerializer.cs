// Copyright (c) Ax0ne.  All Rights Reserved

using System.Text;

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