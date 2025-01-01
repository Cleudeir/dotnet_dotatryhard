namespace dotatryhard.Utils
{
    public class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _evictionOrder;
        private readonly int _defaultExpiration;

        public Cache(int? defaultExpiration = 1)
        {
            _cache = new Dictionary<TKey, LinkedListNode<CacheItem>>();
            _evictionOrder = new LinkedList<CacheItem>();
            _defaultExpiration = defaultExpiration ?? 1;
        }

        public TValue? Get(TKey key)
        {
            if (_cache.TryGetValue(key, out LinkedListNode<CacheItem>? node))
            {
                if (node.Value.ExpirationTime > DateTime.UtcNow)
                {
                    Console.WriteLine($"Cache hit for key: {key}");
                    // Move accessed node to the end of the eviction order
                    _evictionOrder.Remove(node);
                    _evictionOrder.AddLast(node);
                    return node.Value.Value;
                }
                else
                {
                    Console.WriteLine($"Cache expired for key: {key}");
                    Remove(key);
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
            if (_cache.ContainsKey(key))
            {
                // If key exists, update its value and expiration
                Remove(key);
            }

            var expirationTime =
                DateTime.UtcNow + TimeSpan.FromMinutes(expiration ?? _defaultExpiration);
            var cacheItem = new CacheItem(key, value, expirationTime);
            var node = new LinkedListNode<CacheItem>(cacheItem);

            _cache[key] = node;
            _evictionOrder.AddLast(node);

            Console.WriteLine($"Added key: {key}, value: {value}, expires at: {expirationTime}");
        }

        public void Remove(TKey key)
        {
            if (_cache.TryGetValue(key, out LinkedListNode<CacheItem>? node))
            {
                _cache.Remove(key);
                _evictionOrder.Remove(node);
                Console.WriteLine($"Removed key: {key}");
            }
        }

        public void Clear()
        {
            _cache.Clear();
            _evictionOrder.Clear();
            Console.WriteLine("Cache cleared");
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }
            public DateTime ExpirationTime { get; }

            public CacheItem(TKey key, TValue value, DateTime expirationTime)
            {
                Key = key;
                Value = value;
                ExpirationTime = expirationTime;
            }
        }
    }
}
