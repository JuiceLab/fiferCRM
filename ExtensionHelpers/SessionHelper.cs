using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace ExtensionHelpers
{
    public static class SessionHelper
    {
        public static T Get<T>(this Cache cache, string key, object @lock, Func<T> selector,
          DateTime absoluteExpiration)
        {
            object value;
            if ((value = cache.Get(key)) == null)
            {
                lock (@lock)
                {
                    if ((value = cache.Get(key)) == null)
                    {
                        value = selector();
                        cache.Insert(key, value, null,
                         absoluteExpiration, Cache.NoSlidingExpiration,
                         CacheItemPriority.Normal, null);
                    }
                }
            }
            return (T)value;
        }

        public static T Update<T>(this Cache cache, string key, object @lock, Func<T> selector,
          DateTime absoluteExpiration)
        {
            object value;
            if ((value = cache.Get(key)) == null)
            {
                lock (@lock)
                {
                    value = selector();
                    cache.Insert(key, value, null,
                     absoluteExpiration, Cache.NoSlidingExpiration,
                     CacheItemPriority.Normal, null);
                }
            }
            return (T)value;
        }

        public static DateTime GetDateFrom(object session)
        {
            DateTime result = session == null ?
                new DateTime(DateTime.Now.Year, 1, 1)
                : (DateTime)session;
            return result;
        }

        public static DateTime GetDateTo(object session)
        {
            DateTime result = session == null ?
              DateTime.Now.AddDays(1)
                : (DateTime)session;
            return result;
        }
    }
}
