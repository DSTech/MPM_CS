using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.Data {
	public static class IHashRepositoryExtensions {
		public static async Task<IEnumerable<IHashRetriever>> Resolve(this IHashRepository hashRepository, string[] hashes) {
			return await hashRepository.Resolve(hashes.Select(hash => System.Text.Encoding.UTF8.GetBytes(hash)));
		}
		public static async Task<IHashRetriever> Resolve(this IHashRepository hashRepository, byte[] hash) {
			return (await hashRepository.Resolve(new[] { hash })).First();
		}
		public static async Task<IHashRetriever> Resolve(this IHashRepository hashRepository, string hash) {
			return await hashRepository.Resolve(System.Text.Encoding.UTF8.GetBytes(hash));
		}
	}
	public interface IHashRepository {
		/// <summary>
		/// Resolves a series of hashes into potential methods for fetching, wherein each method may provide caching and other services, and each hash may be fetched via differing methods.
		/// </summary>
		/// <param name="hashes">Hashes for retrieval</param>
		/// <returns>
		/// An enumerable of <see cref="IHashRetriever"/>, in the order of the provided hashes.
		/// Null entries will be returned where resolution failed to find a means of fetching a hash.
		/// </returns>
		Task<IEnumerable<IHashRetriever>> Resolve(IEnumerable<byte[]> hashes);
	}
}
