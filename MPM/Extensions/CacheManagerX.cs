using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Extensions {
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
    }
}
