using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Core.Instances.Cache {
    public static class ICacheEntryExtensions {
        public static byte[] Fetch(this ICacheEntry entry) {
            using (var entryStream = entry.FetchStream()) {
                return entryStream.ReadToEnd();
            }
        }

        public static async Task<byte[]> FetchAsync(this ICacheEntry entry) {
            using (var entryStream = entry.FetchStream()) {
                return await entryStream.ReadToEndAsync();
            }
        }
    }
}
