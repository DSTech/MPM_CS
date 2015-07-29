using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Data {

	public class NaiveHttpHashRetriever : IHashRetriever {
		public byte[] Hash { get; }
		public Uri @Uri { get; }

		public NaiveHttpHashRetriever(string hash, Uri uri)
			: this(Convert.FromBase64String(hash), uri) { }

		public NaiveHttpHashRetriever(byte[] hash, Uri uri) {
			Hash = hash;
			this.@Uri = uri;
		}

		public async Task<byte[]> Retrieve() {
			using (var stream = await RetrieveStream()) {
				return await stream.ReadToEndAsync();
			}
		}

		public async Task<Stream> RetrieveStream() {
			var req = WebRequest.CreateHttp(this.@Uri);
			return await req.GetRequestStreamAsync();
		}
	}
}
