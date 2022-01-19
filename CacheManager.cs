using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace testkeyvault
{
    public static class CacheManager
    {
       

        public static DateTimeOffset Offset(this DateTimeOffset offset, TimeUnit unit, int i)
        {
         switch (unit)
            {
                case TimeUnit.DAY:
                    return offset.AddDays(i);
                case TimeUnit.HOUR:
                    return offset.AddHours(i);
                case TimeUnit.MINUTE:
                    return offset.AddMinutes(i);
                case TimeUnit.SECOND:
                    return offset.AddSeconds(i);
                default:
                    return DateTimeOffset.Now;
                        
            }
        }

        public static DateTimeOffset Offset(TimeUnit unit, int i)
        {
            return DateTimeOffset.Now.Offset(unit, i);
        }
    }
    public enum TimeUnit
    {
        DAY,HOUR,MINUTE,SECOND
    }



    public class LoadingCache<V>
    {
        private CacheItemPolicy policy;
        private string signiture;
        Dictionary<string, object> locks = new Dictionary<string, object>();
        public V Get(string key)
        {
            object Lock;
            lock (locks)
            {
                if(!locks.TryGetValue(key, out Lock))
                {
                    Lock = new object();
                    locks.Add(key, Lock);
                }
            }
            V v;
            lock (Lock)
            {
                v = (V)MemoryCache.Default.Get(signiture + key);

                if (v == null)
                {
                    v = Load(key);
                    if (v != null)
                    {
                        MemoryCache.Default.Add(signiture + key, v, policy);
                    }
                }
            }
            return v;
        }

        public Func<string, V> Load;
        public LoadingCache(TimeUnit unit, int i, Func<string, V> load) : this(CacheManager.Offset(unit, i), load)
        {

        }
        public LoadingCache(DateTimeOffset offset, Func<string, V> load) : this(new CacheItemPolicy() { AbsoluteExpiration = offset }, load)
        {

        }
        public LoadingCache(CacheItemPolicy policy, Func<string, V> load)
        {
            this.policy = policy;
            Load = load;
            signiture = Guid.NewGuid().ToString();
        }

    }
        




}
