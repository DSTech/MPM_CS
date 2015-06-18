using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Info;
using MPM.Data;
using MPM.Net.DTO;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
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
			//TODO: Persist the package-data for the NamedBuild being installed
			//TODO: Error if any builds of the same package are already installed
			/*var archiveContents = await hashRepository.RetrieveArchive(build.Name, build.Hashes);
			var archiveVFS = ArchiveFileSystem.FromData(archiveContents);
			PackageInfoParser packageInfo;
			{
				string packageJson;
				try {
					using (var packageJsonReader = new StreamReader(archiveVFS.Resolve(new Uri("package.json")).OpenRead())) {
						packageJson = packageJsonReader.ReadToEnd();
					}
				} catch {
					throw new InstallationException("Failed to read package.json from package");
				}
				try {
					packageInfo = new PackageInfoParser(packageJson);
				} catch (FormatException e) {
					throw new InstallationException("Package info was of incorrect format", e);
				}
			}
			IEnumerable<IInstallationOperation> installationScript;
			try {
				installationScript = packageInfo.InstallationScript;
			} catch (FormatException e) {
				throw new InstallationException("Installation script was of incorrect format", e);
			}
			var backup = fileIndex.Save();
			try {
				foreach (var installationOperation in installationScript) {
					//TODO: Index should store origin of the file (Essentially the file declaration) and the name of the package the installation instruction came from
					fileIndex.Update(build.Name, installationOperation);
				}
			} catch (Exception) {
				fileIndex.Restore(backup);
				throw new InstallationException();
			}
			var delta = fileIndex.CalculateDelta(fileSystem);
			//TODO: Clear non-config changes to filesystem
			//if !fileSystem.Consider(delta)://TODO: Implement a simulation of operations to internally determine consistancy with state
			//	throw new InstallationException();
			fileSystem.Apply(delta);*/
			throw new NotImplementedException();
		}
	}
}
