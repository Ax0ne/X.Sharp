// Copyright (c) Ax0ne.  All Rights Reserved

namespace X.Sharp.Redis
{
    public class RedisConfig
    {
        /// <summary>
        /// redis 连接配置字符串
        /// </summary>
        public string ConnectionConfig { get; set; } = "localhost:6379";
        /// <summary>
        /// 链接池大小 默认1
        /// </summary>
        public int PoolSize { get; set; } = 1;
        /// <summary>
        /// ConnectionMultiplexer 的 SocketManager 的 WorkCount
        /// </summary>
        public int WorkCount { get; set; }
        public int DbIndex { get; set; } = 0;
        public string? Prefix { get; set; }
        public ISerializer? Serializer { get; set; }
    }
}
