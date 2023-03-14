// Copyright (c) Ax0ne.  All Rights Reserved

using StackExchange.Redis;

#pragma warning disable CS8618

namespace X.Sharp.Redis
{
    public class RedisClient
    {
        private ConnectionMultiplexer _connection;
        private int _dbIndex;
        private string? _prefix;
        private ISerializer Serializer = new SystemTextJsonSerializer();
        private static readonly string TypeName;

        static RedisClient()
        {
            TypeName = Type.DefaultBinder.GetType().Name;
        }

        public RedisClient(string connectionConfig)
        {
            InitRedis(new RedisConfig { ConnectionConfig = connectionConfig });
        }

        public RedisClient(RedisConfig config)
        {
            InitRedis(config);
        }

        public RedisClient(string connectionConfig, int dbIndex)
        {
            InitRedis(new RedisConfig { ConnectionConfig = connectionConfig, DbIndex = dbIndex });
        }

        private void InitRedis(RedisConfig config)
        {
            _dbIndex = config.DbIndex;
            _prefix = config.Prefix;
            if (config.Serializer != null)
                Serializer = config.Serializer;

            var options = ConfigurationOptions.Parse(config.ConnectionConfig);
            if (config.WorkCount > 5)
            {
                options.SocketManager = new SocketManager(TypeName, config.WorkCount);
            }

            _connection = ConnectionMultiplexer.Connect(options);
            //if (config.PoolSize > 0)
            //{
            //    for (var i = 0; i < config.PoolSize; i++)
            //    {
            //        _connections[i] = ConnectionMultiplexer.Connect(options);
            //    }
            //}
        }

        private IDatabase Database => _connection.GetDatabase(_dbIndex);

        //public bool StringSet<T>(string key, T value, TimeSpan? expiry = default(TimeSpan?))
        //{
        //    key = AddPrefix(key);
        //    var v = Serializer.Serialize(value);
        //    return Database.StringSet(key, v, expiry);
        //}
        public async Task<bool> StringSetAsync<T>(string key, T value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefix(key);
            return await Database.StringSetAsync(key, Serializer.Serialize(value), expiry).ConfigureAwait(false);
        }

        public async Task<bool> StringSetAsync<T>(List<KeyValuePair<string, T>> keyValues)
        {
            var redisKeyValues = keyValues.Select(s =>
                new KeyValuePair<RedisKey, RedisValue>(AddPrefix(s.Key), Serializer.Serialize(s.Value))).ToArray();
            return await Database.StringSetAsync(redisKeyValues).ConfigureAwait(false);
        }

        //public T StringGet<T>(string key)
        //{
        //    key = AddPrefix(key);
        //    var valueBytes = Database.StringGet(key);
        //    return !valueBytes.HasValue ? default : Serializer.Deserialize<T>(valueBytes);
        //}
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = AddPrefix(key);
            var valueBytes = await Database.StringGetAsync(key).ConfigureAwait(false);
            return !valueBytes.HasValue ? default : Serializer.Deserialize<T>(valueBytes);
        }

        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.ListLeftPushAsync(key, Serializer.Serialize(value)).ConfigureAwait(false);
        }

        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = AddPrefix(key);
            var redisValue = await Database.ListLeftPopAsync(key).ConfigureAwait(false);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.ListRightPushAsync(key, Serializer.Serialize(value)).ConfigureAwait(false);
        }

        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = AddPrefix(key);
            var redisValue = await Database.ListRightPopAsync(key).ConfigureAwait(false);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = AddPrefix(key);
            var values = await Database.ListRangeAsync(key).ConfigureAwait(false);
            return values.Length > 0 ? values.Select(s => Serializer.Deserialize<T>(s)).ToList() : new List<T>();
        }

        public async Task<long> ListLengthAsync(string key)
        {
            return await Database.ListLengthAsync(key).ConfigureAwait(false);
        }

        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.ListRemoveAsync(key, Serializer.Serialize(value)).ConfigureAwait(false);
        }

        public async Task<bool> SetAddAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.SetAddAsync(key, Serializer.Serialize(value));
        }

        public async Task<bool> SetContainsAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.SetContainsAsync(key, Serializer.Serialize(value));
        }

        public async Task<List<T>> SetMembersAsync<T>(string key)
        {
            key = AddPrefix(key);
            var redisValues = await Database.SetMembersAsync(key);
            return redisValues.Length > 0
                ? redisValues.Select(s => Serializer.Deserialize<T>(s)).ToList()
                : new List<T>();
        }

        public async Task<bool> SetRemoveAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.SetRemoveAsync(key, Serializer.Serialize(value));
        }

        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T value)
        {
            key = AddPrefix(key);
            return await Database.HashSetAsync(key, dataKey, Serializer.Serialize(value)).ConfigureAwait(false);
        }

        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            key = AddPrefix(key);
            var redisValue = await Database.HashGetAsync(key, dataKey).ConfigureAwait(false);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public async Task<List<T>> HashGetAllAsync<T>(string key)
        {
            key = AddPrefix(key);
            var redisValues = await Database.HashGetAllAsync(key).ConfigureAwait(false);
            return redisValues.Length > 0
                ? redisValues.Select(s => Serializer.Deserialize<T>(s.Value)).ToList()
                : new List<T>();
        }

        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = AddPrefix(key);
            return await Database.HashDeleteAsync(key, dataKey).ConfigureAwait(false);
        }

        public async Task<long> HashDeleteAsync(string key, string[] dataKeys)
        {
            key = AddPrefix(key);
            return await Database.HashDeleteAsync(key, dataKeys.Select(s => (RedisValue)s).ToArray())
                .ConfigureAwait(false);
        }

        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = AddPrefix(key);
            return await Database.HashExistsAsync(key, dataKey).ConfigureAwait(false);
        }

        public async Task<List<string>> HashKeysAsync<T>(string key)
        {
            key = AddPrefix(key);
            var values = await Database.HashKeysAsync(key).ConfigureAwait(false);
            return values.Select(s => s.ToString()).ToList();
        }

        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefix(key);
            return await Database.HashIncrementAsync(key, dataKey, val).ConfigureAwait(false);
        }

        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefix(key);
            return await Database.HashDecrementAsync(key, dataKey, val).ConfigureAwait(false);
        }

        public Dictionary<string, T> HashScan<T>(string key, string pattern, int pageSize = 10)
        {
            key = AddPrefix(key);
            var values = Database.HashScan(key, pattern, pageSize);
            var result = new Dictionary<string, T>();
            if (!values.Any()) return result;
            foreach (var v in values)
            {
                var value = v.Value.HasValue ? Serializer.Deserialize<T>(v.Value) : default;
                result.Add(v.Name.ToString(), value);
            }

            return result;
        }

        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = AddPrefix(key);
            return await Database.SortedSetAddAsync(key, Serializer.Serialize(value), score).ConfigureAwait(false);
        }

        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = AddPrefix(key);
            return await Database.SortedSetRemoveAsync(key, Serializer.Serialize(value)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T?>> SortedSetRangeByScoreAsync<T>(
            string key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None,
            Order order = Order.Ascending,
            long skip = 0L,
            long take = -1L)
        {
            key = AddPrefix(key);
            var result = await Database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take)
                .ConfigureAwait(false);

            return result.Select(m => m == RedisValue.Null ? default : Serializer.Deserialize<T>(m));
        }

        public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            key = AddPrefix(key);
            var result = await Database.SortedSetRangeByRankAsync(key).ConfigureAwait(false);
            return result.Select(m => m == RedisValue.Null ? default : Serializer.Deserialize<T>(m));
        }

        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = AddPrefix(key);
            return await Database.SortedSetLengthAsync(key).ConfigureAwait(false);
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            key = AddPrefix(key);
            return await Database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task<long> KeyDeleteAsync(List<string> keys)
        {
            return await Database.KeyDeleteAsync(ConvertRedisKeys(keys.Select(AddPrefix))).ConfigureAwait(false);
        }

        public async Task<bool> KeyRename(string key, string newKey)
        {
            key = AddPrefix(key);
            newKey = AddPrefix(newKey);
            return await Database.KeyRenameAsync(key, newKey).ConfigureAwait(false);
        }

        public async Task<bool> KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddPrefix(key);
            return await Database.KeyExpireAsync(key, expiry).ConfigureAwait(false);
        }

        public Task<long> PublishAsync<T>(string channel, T message)
        {
            ISubscriber sub = _connection.GetSubscriber();
            return sub.PublishAsync(channel, Serializer.Serialize(message));
        }

        public Task SubscribeAsync<T>(string channel, Func<T, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            ISubscriber sub = _connection.GetSubscriber();

            async void Handler(RedisChannel redisChannel, RedisValue value) =>
                await handler(Serializer.Deserialize<T>(value)).ConfigureAwait(false);

            return sub.SubscribeAsync(channel, Handler);
        }

        public Task UnsubscribeAsync<T>(string channel, Func<T, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            ISubscriber sub = _connection.GetSubscriber();
            return sub.UnsubscribeAsync(channel, (_, value) => handler(Serializer.Deserialize<T>(value)));
        }

        public Task UnsubscribeAllAsync()
        {
            ISubscriber sub = _connection.GetSubscriber();
            return sub.UnsubscribeAllAsync();
        }

        private string AddPrefix(string key)
        {
            return _prefix == null ? key : $"{_prefix}:{key}";
        }

        private static RedisKey[] ConvertRedisKeys(IEnumerable<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }
    }
}