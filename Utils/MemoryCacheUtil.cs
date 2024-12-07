namespace dotatryhard.Utils
{


    public class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, CacheItem> _cache;
        private readonly int _capacity;
        private readonly int _defaultExpiration;

        public Cache(int capacity, int? defaultExpiration = 1)
        {
            _cache = new Dictionary<TKey, CacheItem>(capacity);
            _capacity = capacity;
            _defaultExpiration = defaultExpiration ?? 1;
        }
        public TValue Get(TKey key)
        {
            if (_cache.TryGetValue(key, out CacheItem item))
            {
                if (item.ExpirationTime > DateTime.UtcNow)
                {
                    Console.WriteLine($"Cache hit for key: {key}");
                    return item.Value;
                }
                else
                {
                    Console.WriteLine($"Cache expired for key: {key}");
                    _cache.Remove(key); // Remove expired item
                }
            }
            else
            {
                Console.WriteLine($"Cache miss for key: {key}");
            }

            return default(TValue);
        }

        public void Set(TKey key, TValue value, int? expiration = null)
        {
            if (_cache.Count >= _capacity)
            {
                var firstKey = new List<TKey>(_cache.Keys)[0];
                _cache.Remove(firstKey);
                Console.WriteLine($"Evicted key: {firstKey}");
            }
            var expirationTime = DateTime.UtcNow + TimeSpan.FromDays(expiration ?? _defaultExpiration);
            _cache[key] = new CacheItem(value, expirationTime);
            Console.WriteLine($"Added key: {key}, value: {value}, expires at: {expirationTime}");
        }

        public void Remove(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
                Console.WriteLine($"Removed key: {key}");
            }
        }

        public void Clear()
        {
            _cache.Clear();
            Console.WriteLine("Cache cleared");
        }

        private class CacheItem
        {
            public TValue Value { get; }
            public DateTime ExpirationTime { get; }

            public CacheItem(TValue value, DateTime expirationTime)
            {
                Value = value;
                ExpirationTime = expirationTime;
            }
        }
    }
}