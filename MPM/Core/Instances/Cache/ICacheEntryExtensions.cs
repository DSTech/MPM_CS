using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Core.Instances.Cache {
    public static class ICacheEntryExtensions {
        public static byte[] Fetch(this ICacheEntry entry) => entry.FetchStream().ReadToEndAndClose();

        public static Task<byte[]> FetchAsync(this ICacheEntry entry) => entry.FetchStream().ReadToEndAndCloseAsync();
    }
}
