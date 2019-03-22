using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.CacheHelper
{
    public class CacheContext : ICacheContext
    {
        private readonly System.Web.Caching.Cache _objCache = HttpRuntime.Cache;
        public override T Get<T>(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return (T)objCache[key];
        }

        public override bool Set<T>(string key, T t, DateTime expire)
        {
            var obj = Get<T>(key);
            if (obj != null)
            {
                Remove(key);
            }

            _objCache.Insert(key, t, null, expire, System.Web.Caching.Cache.NoSlidingExpiration);
            return true;
        }

        public override bool Remove(string key)
        {
            _objCache.Remove(key);
            return true;
        }
    }
}
