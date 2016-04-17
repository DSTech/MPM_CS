using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MPM.Core.Instances.Cache {
    public static class CacheManagerX {
        public static void StoreFromStream(this MPM.Core.Instances.Cache.ICacheManager cacheManager, string cacheEntryName, Stream content, bool closeTarget = true) {
            using (var memStr = new MemoryStream()) {
                if (closeTarget) {
                    content.CopyToAndClose(memStr);
                } else {
                    content.CopyTo(memStr);
                }
                cacheManager.Store(cacheEntryName, memStr.ToArrayFromStart());
            }
        }

        public static void StoreAsJson<T>(this MPM.Core.Instances.Cache.ICacheManager cacheManager, string cacheEntryName, T toSerialize) {
            cacheManager.StoreString(cacheEntryName, JsonConvert.SerializeObject(toSerialize, typeof(T), Formatting.Indented, new JsonSerializerSettings()));
        }

        public static void StoreString(this MPM.Core.Instances.Cache.ICacheManager cacheManager, string cacheEntryName, string data) {
            cacheManager.Store(cacheEntryName, Encoding.UTF8.GetBytes(data));
        }
        public static string FetchString(this MPM.Core.Instances.Cache.ICacheManager cacheManager, string cacheEntryName) {
            var entry = cacheManager.Fetch(cacheEntryName);
            if (entry == null) {
                return null;
            }
            var fetched = entry.Fetch();
            return Encoding.UTF8.GetString(fetched);
        }

        public static T FetchFromJson<T>(this MPM.Core.Instances.Cache.ICacheManager cacheManager, string cacheEntryName) {
            var fetched = cacheManager.FetchString(cacheEntryName);
            if (fetched == null) {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(fetched);
        }
    }
}