using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MPM.Types;
using MPM.Util;

namespace MPM.Data {
    public class NaiveHttpHashRetriever : IHashRetriever {
        public NaiveHttpHashRetriever(string hash, Uri uri)
            : this(Hash.Parse(hash), uri) {
        }

        public NaiveHttpHashRetriever(Hash hash, Uri uri) {
            this.Hash = hash;
            this.@Uri = uri;
        }

        public Uri @Uri { get; }
        public Hash Hash { get; }

        public async Task<byte[]> Retrieve() {
            using (var stream = await RetrieveStream()) {
                return await stream.ReadToEndAsync();
            }
        }

        public async Task<Stream> RetrieveStream() {
            var req = WebRequest.CreateHttp(this.@Uri);
            var res = await req.GetResponseAsync();
            return new DisposerStreamWrapper(res.GetResponseStream(), res);
        }
    }
}
