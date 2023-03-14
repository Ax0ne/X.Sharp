// Copyright (c) Ax0ne.  All Rights Reserved

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable All
#pragma warning disable CS8603

namespace X.Sharp.Redis
{
    public class SystemTextJsonSerializer : ISerializer
    {
        private static JsonSerializerOptions? _options;

        static SystemTextJsonSerializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public byte[] Serialize<T>(T? item)
        {
            return JsonSerializer.SerializeToUtf8Bytes<T>(item!, _options);
        }

        public T Deserialize<T>(byte[] serializedObject)
        {
            return JsonSerializer.Deserialize<T>(serializedObject, _options);
        }
    }
}