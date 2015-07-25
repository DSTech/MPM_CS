using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public class NaiveHttpHashRepository : IHashRepository {
		public NaiveHttpHashRepository() { }
		public NaiveHttpHashRepository(Uri baseUri) {
			BaseUri = baseUri;
		}
		public Uri BaseUri { get; set; }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<IEnumerable<IHashRetriever>> Resolve(IEnumerable<byte[]> hashes) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
			return hashes
				.Select(
					hash =>
						new NaiveHttpHashRetriever(
							hash,
							new Uri(
								BaseUri,
								Convert.ToBase64String(hash)
							)
						)
				);
		}
	}
}
