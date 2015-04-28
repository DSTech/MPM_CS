using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Data;
using MPM.Net.DTO;

namespace MPM.Core.Instances {
	public class Installer {
		private readonly Instance instance;
		private readonly IFileSystem fileSystem;
		private readonly IPackageRepository packageRepository;
		private readonly IHashRepository hashRepository;

		public Installer(Instance instance, IPackageRepository packageRepository, IHashRepository hashRepository) {
			this.instance = instance;
			this.packageRepository = packageRepository;
			this.hashRepository = hashRepository;
			fileSystem = instance.GetFileSystem();
		}
		/// <summary>
		/// Adds a package to an instance's configuration, then performs necessary filesystem modifications the package requests.
		/// </summary>
		/// <param name="build"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">Thrown when a package could not be resolved to an archive or retrieved.</exception>
		/// <exception cref="Exception">Thrown when a package fails to resolve or install. All existing changes will be reverted before throwing.</exception>
		/// <exception cref="InvalidOperationException">Thrown when a version of the specified package is already installed.</exception>
		/// <exception cref="FormatException">Thrown when the retrieved archive was invalid.</exception>
		public async Task<InstanceConfiguration> Install(NamedBuild build) {
			//TODO: Error if any builds of the same package are already installed
			var archiveContents = await RetrieveArchive(hashRepository, build.Name, build.Hashes);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Collects and assembles an archive's hashes from the given hash repository.
		/// </summary>
		/// <param name="hashRepository">The repository within which to search for hashes.</param>
		/// <param name="hashes">The hashes which must be assembled, in order, to produce the package archive</param>
		/// <returns>A byte array of the unpacked archive.</returns>
		/// <exception cref="KeyNotFoundException">Thrown when a package could not be resolved to an archive or retrieved.</exception>
		/// <exception cref="FormatException">Thrown when the retrieved archive was invalid.</exception>
		private async Task<byte[]> RetrieveArchive(IHashRepository hashRepository, string packageName, string[] hashes) {
			var hashRetrievers = await hashRepository.Resolve(hashes);
			var retrievers = hashes.Zip(hashRetrievers, (hash, retriever) => Tuple.Create(hash, retriever));
			{
				var failedResolutions = retrievers.Where(retr => retr.Item2 == null).ToArray();
				if (failedResolutions.Length > 0) {
					throw new KeyNotFoundException(
						String.Format(
							"Could not resolve method of retreival for hashes:\n[\n{0}\n]",
							String.Join(
								",\n",
								failedResolutions.Select(
									item => String.Format("\t\"{0}\"", item.Item1)
								)
							)
						),
						new AggregateException(
							failedResolutions.Select(failedResolution => new KeyNotFoundException(failedResolution.Item1))//An exception is created with each hash that could not be located.
						)
					);//Outputs a json-esque array to allow easy export by a user alongside an aggregate containing a more easily computer-understandable output
				}
			}
			//TODO: Switch to TPL Dataflow as described in https://msdn.microsoft.com/en-us/library/hh228603.aspx for parallelism control to prevent flooding
			//WhenAll preserves order of provided tasks, so each value will be associated with its parent hash
			var retrievedHashes = (await Task.WhenAll(retrievers.Select(async retriever => await retriever.Item2.Retrieve())));
			var archive = new Archival.Archive(retrievedHashes.Select(retrievedHash => new Archival.EncryptedChunk(retrievedHash)));
			return await archive.Unpack(packageName);
		}
	}
}
