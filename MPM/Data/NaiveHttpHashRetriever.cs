using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using MPM.Util;

namespace MPM.Data {
    public class NaiveHttpHashRetriever : IHashRetriever {
        public NaiveHttpHashRetriever(string hash, Uri uri)
            : this(Convert.FromBase64String(hash), uri) {
        }

        public NaiveHttpHashRetriever(byte[] hash, Uri uri) {
            Hash = hash;
            this.@Uri = uri;
        }

        public Uri @Uri { get; }
        public byte[] Hash { get; }

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
