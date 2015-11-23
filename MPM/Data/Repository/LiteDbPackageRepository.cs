using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using MPM.Data.Repository;
using MPM.Types;
using MPM.Net;
using semver.tools;
using System.Linq;
using System.Linq.Expressions;
using MPM.Core.Dependency;

namespace MPM.Data.Repository {
	public class LiteDbPackageRepository : IPackageRepository {
		public class PackageRepositoryEntry {
			public PackageRepositoryEntry() { }
			public PackageRepositoryEntry(Package package) {
				this.Name = package.Name;
				this.Authors = package.Authors.Select(DTOTranslationExtensions.ToDTO).ToList();
				this.Builds = package.Builds.Select(DTOTranslationExtensions.ToDTO).ToList();
			}

			[BsonId]
			public String Name { get; set; }
			[BsonField]
			public List<Net.DTO.Author> Authors { get; set; }
			[BsonField]
			public List<Net.DTO.Build> Builds { get; set; }
			[BsonField]
			public DateTime? LastUpdated { get; set; } = DateTime.Now;

			public static implicit operator PackageRepositoryEntry(Package package) {
				return new PackageRepositoryEntry(package) { LastUpdated = DateTime.Now };
			}
			public static implicit operator Package(PackageRepositoryEntry entry) {
				return new Package(
					entry.Name,
					entry.Authors.Select(DTOTranslationExtensions.FromDTO).ToArray(),
					entry.Builds.Select(DTOTranslationExtensions.FromDTO).ToArray()
				);
			}
			public static implicit operator Net.DTO.Package(PackageRepositoryEntry entry) {
				return new Net.DTO.Package {
					Name = entry.Name,
					Authors = entry.Authors,
					Builds = entry.Builds,
				};
			}
		}

		private readonly LiteCollection<PackageRepositoryEntry> PackageCollection;

		public LiteDbPackageRepository(LiteCollection<PackageRepositoryEntry> packageCollection) {
			if ((this.PackageCollection = packageCollection) == null) {
				throw new ArgumentNullException(nameof(packageCollection));
			}
		}

		private bool MeetsCompatibilityRequirements(CompatibilityPlatform assigned, CompatibilityPlatform requested) {
			return PackageSpecExtensions.IsPlatformCompatible(assigned, requested);
		}
		private Func<Net.DTO.Build, bool> CreatePackageFilter(
				SemanticVersion version,
				CompatibilitySide side,
				Arch arch,
				CompatibilityPlatform platform
			) {
			var dtoSide = side.ToDTO();
			var dtoVersion = version.ToDTO();
			var dtoArch = arch.ToDTO();
			return b =>
					b.Version == dtoVersion
					&& b.Side == dtoSide
					&& b.Arch == dtoArch
					&& MeetsCompatibilityRequirements(DTOTranslationExtensions.FromCompatibilityPlatformDTO(b.Platform), platform)//TODO: Add universal and non-bitness platform support
					;
		}

		public Task<Build> FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch, CompatibilityPlatform platform) {
			return Task.Run<Build>(() => {
				var package = PackageCollection.FindOne(p => p.Name == packageName);
				if (package == null) {
					return null;
				}

				var build = package.Builds.AsEnumerable().FirstOrDefault(CreatePackageFilter(version, side, arch, platform));
				return build?.FromDTO();
			});
		}

		public Task<IEnumerable<Build>> FetchBuilds(string packageName, VersionSpec versionSpec) {
			return Task.Run<IEnumerable<Build>>(() => {
				var packageEntry = PackageCollection.FindOne(b => b.Name == packageName);
				if (packageEntry == null) {
					return null;
				}
				var package = ((Net.DTO.Package)packageEntry).FromDTO();
				var builds = package.Builds
					.Select(build => new { build, version = build.Version })
					.Where(b => versionSpec.Satisfies(b.version))
					.OrderByDescending(build => build.version)
					.Select(b => b.build)
					.ToArray();
				return builds;
			});
		}

		public Task<Package> FetchPackage(string packageName) {
			return Task.FromResult(((Net.DTO.Package)PackageCollection.FindOne(p => p.Name == packageName))?.FromDTO());
		}

		public Task<IEnumerable<Package>> FetchPackageList() {
			return Task.FromResult(PackageCollection.FindAll().Select(p => ((Net.DTO.Package)p).FromDTO()).ToArray().AsEnumerable());
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			return Task.FromResult(PackageCollection.FindAll().Select(p => ((Net.DTO.Package)p).FromDTO()).ToArray().AsEnumerable());
		}

		private Package RegisterBuildSynchronous(Build build) {
			var existingPackage = PackageCollection.FindOne(p => p.Name == build.PackageName);
			if (existingPackage == null) {
				throw new KeyNotFoundException($"Participant package \"{build.PackageName}\" not yet registered");
			}
			var withEquivalentVersion = existingPackage.Builds.Where(b => b.Version.FromDTO() == build.Version);
			existingPackage.Builds.Add(build.ToDTO());
			PackageCollection.Update(existingPackage);
			return ((Net.DTO.Package)existingPackage).FromDTO();
		}

		/// <summary>
		/// Registers a build into the system, or updates it.
		/// </summary>
		/// <param name="build">The build to register.</param>
		/// <returns>The package into which the build was registered</returns>
		public Task<Package> RegisterBuild(Build build) {
			return Task.FromResult(RegisterBuildSynchronous(build));
		}

		public Task<Package> RegisterPackage(String packageName, IEnumerable<Author> authors) {
			return Task.FromResult(new Func<Package>(() => {
				var existingPackage = PackageCollection.FindOne(p => p.Name == packageName);
				if (existingPackage != null) {
					existingPackage.Authors = authors.Select(DTOTranslationExtensions.ToDTO).ToList();
					PackageCollection.Update(existingPackage);
					return existingPackage;
				}
				var newPackage = new PackageRepositoryEntry {
					Name = packageName,
					Authors = authors.Select(DTOTranslationExtensions.ToDTO).ToList(),
					Builds = Enumerable.Empty<Net.DTO.Build>().ToList(),
				};
				PackageCollection.Insert(newPackage);
				return newPackage;
			})());
		}
	}
}
