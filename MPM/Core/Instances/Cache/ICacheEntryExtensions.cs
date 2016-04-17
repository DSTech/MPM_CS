using System.IO;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Cache {
    public static class ICacheEntryExtensions {
        public static byte[] Fetch(this ICacheEntry entry) => entry.FetchStream().ReadToEndAndClose();

        public static Task<byte[]> FetchAsync(this ICacheEntry entry) => entry.FetchStream().ReadToEndAndCloseAsync();
    }
}
