using System;
using System.Collections.Generic;
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

        public Build FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch, CompatibilityPlatform platform) {
            var package = PackageCollection.FindOne(p => p.Name == packageName);
            if (package == null) {
                return null;
            }

            var build = package.Builds.AsEnumerable().FirstOrDefault(CreatePackageFilter(version, side, arch, platform));
            return build?.FromDTO();
        }

        public IEnumerable<Build> FetchBuilds(string packageName, VersionSpec versionSpec) {
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
        }

        public Package FetchPackage(string packageName) {
            var package = PackageCollection.FindOne(p => p.Name == packageName);
            if (package == null) {
                return null;
            }
            return ((Net.DTO.Package)package)?.FromDTO();
        }

        public IEnumerable<Package> FetchPackageList() {
            return PackageCollection.FindAll().Select(p => ((Net.DTO.Package)p).FromDTO()).ToArray().AsEnumerable();
        }

        public IEnumerable<Package> FetchPackageList(DateTime updatedAfter) {
            return PackageCollection.FindAll().Select(p => ((Net.DTO.Package)p).FromDTO()).ToArray().AsEnumerable();
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
        public Package RegisterBuild(Build build) {
            return RegisterBuildSynchronous(build);
        }

        public Package RegisterPackage(String packageName, IEnumerable<Author> authors) {
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
        }

        public bool DeletePackage(string packageName) {
            if (String.IsNullOrWhiteSpace(packageName)) {
                throw new ArgumentException("Argument is null or whitespace", nameof(packageName));
            }
            return PackageCollection.Delete(p => p.Name == packageName) > 0;
        }
    }
}
