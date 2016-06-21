using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPM.Types;
using MPM.Util;

namespace MPM.Data.Repository {
    public static class IHashRepositoryExtensions {
        public static IEnumerable<IHashRetriever> Resolve(this IHashRepository hashRepository, params Hash[] hashes) => hashRepository.Resolve(hashes.AsEnumerable());

        public static Task<byte[]> RetrieveArchive(this IHashRepository hashRepository, string packageName, IEnumerable<Hash> hashes) =>
            hashRepository.RetrieveArchive(packageName, hashes.ToArray());

        /// <summary>
        ///     Collects and assembles an archive's hashes from the given hash repository.
        /// </summary>
        /// <param name="hashRepository">The repository within which to search for hashes.</param>
        /// <param name="packageName">The package to look up</param>
        /// <param name="hashes">The hashes which must be assembled, in order, to produce the package archive</param>
        /// <returns>A byte array of the unpacked archive.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when a package could not be resolved to an archive or retrieved.</exception>
        /// <exception cref="FormatException">Thrown when the retrieved archive was invalid.</exception>
        public static async Task<byte[]> RetrieveArchive(this IHashRepository hashRepository, string packageName, params Hash[] hashes) {
            if (hashes.Length == 0) {
                throw new ArgumentOutOfRangeException(nameof(hashes), "hash array contained no elements");
            }
            var hashRetrievers = hashRepository.Resolve(hashes);
            var retrievers = hashes.Zip(hashRetrievers, Tuple.Create).ToArray();
            {
                var failedResolutions = retrievers.Where(retr => retr.Item2 == null).ToArray();
                if (failedResolutions.Length > 0) {
                    throw new KeyNotFoundException(
                        String.Format(
                            "Could not resolve method of retreival for hashes:\n[\n{0}\n]",
                            String.Join(
                                ",\n",
                                failedResolutions.Select(item => $"\t\"{item.Item1}\"")
                                )
                            ),
                        new AggregateException(
                            failedResolutions.Select(failedResolution => new KeyNotFoundException(failedResolution.Item1.ToString()))//An exception is created with each hash that could not be located.
                            )
                        );//Outputs a json-esque array to allow easy export by a user alongside an aggregate containing a more easily computer-understandable output
                }
            }
            //WhenAll preserves order of provided tasks, so each value will be associated with its parent hash
            var retrievedHashes = (await Task.WhenAll(retrievers.Select(async retriever => await retriever.Item2.Retrieve())));

            using (var unpackedContent = new MemoryStream()) {
                var hashContent = new MemoryStream();
                foreach (var ret in retrievedHashes) {
                    hashContent.Write(ret);
                }
                hashContent.SeekToStart();
                Archival.Archive.Unpack(hashContent, packageName, unpackedContent);
                unpackedContent.SeekToStart();
                return unpackedContent.ToArray();
            }
        }
    }

    public interface IHashRepository {
        /// <summary>
        ///     Resolves a series of hashes into potential methods for fetching, wherein each method may provide caching and other
        ///     services, and each hash may be fetched via differing methods.
        /// </summary>
        /// <param name="hashes">Hashes for retrieval</param>
        /// <returns>
        ///     An enumerable of <see cref="IHashRetriever" />, in the order of the provided hashes.
        ///     Null entries will be returned where resolution failed to find a means of fetching a hash.
        /// </returns>
        IEnumerable<IHashRetriever> Resolve(IEnumerable<Hash> hashes);
    }
}
