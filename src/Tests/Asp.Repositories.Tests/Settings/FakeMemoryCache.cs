using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Asp.Repositories.Tests.Settings
{
    public class FakeMemoryCache : IMemoryCache
    {
        public void Dispose()
        {
        }

        public bool TryGetValue(object key, out object value)
        {
            value = null;
            return false;
        }

        public ICacheEntry CreateEntry(object key)
        {
            return new FakeCacheEntry(key, null, null);
        }

        public void Remove(object key)
        {
        }

        public void Set<T>(object key, T value, MemoryCacheEntryOptions options)
        {
        }

        public class FakeCacheEntry : ICacheEntry
        {
            public FakeCacheEntry(object key, Action<FakeCacheEntry> notifyCacheEntryDisposed, Action<FakeCacheEntry> notifyCacheOfExpiration)
            {
            }

            public void Dispose()
            {
            }

            public object Key { get; }
            public object Value { get; set; }
            public DateTimeOffset? AbsoluteExpiration { get; set; }
            public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
            public TimeSpan? SlidingExpiration { get; set; }
            public IList<IChangeToken> ExpirationTokens { get; }
            public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
            public CacheItemPriority Priority { get; set; }
            public long? Size { get; set; }
        }
    }
}