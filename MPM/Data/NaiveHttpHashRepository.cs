using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Data.Repository;
using MPM.Types;
using MPM.Util;

namespace MPM.Data {
    public class NaiveHttpHashRepository : IHashRepository {
        public NaiveHttpHashRepository() {
        }

        public NaiveHttpHashRepository(Uri baseUri) {
            BaseUri = baseUri;
        }

        public Uri BaseUri { get; set; }

        public IEnumerable<IHashRetriever> Resolve(IEnumerable<Hash> hashes) {
            return hashes
                .Select(
                    hash =>
                        new NaiveHttpHashRetriever(
                            hash,
                            new Uri(
                                BaseUri,
                                hash.ToString().Replace(':', '_')
                                )
                            )
                );
        }
    }
}
