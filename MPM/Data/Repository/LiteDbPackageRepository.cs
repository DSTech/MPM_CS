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
using Newtonsoft.Json;

namespace MPM.Data.Repository {
    public class LiteDbPackageRepository : IPackageRepository {
        private static readonly BsonMapper mapper = new BsonMapper();
        private static void RegisterBsonWithJsonNet<T>(BsonMapper mapper) {
            mapper.RegisterType<T>(
                (T t) => JsonConvert.SerializeObject(t, typeof(T), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }),
                (BsonValue jsonT) => JsonConvert.DeserializeObject<T>(jsonT, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })
                );
        }
        static LiteDbPackageRepository() {
            RegisterBsonWithJsonNet<Build>(mapper);
            RegisterBsonWithJsonNet<Arch>(mapper);
            RegisterBsonWithJsonNet<CompatibilitySide>(mapper);
        }

        private readonly LiteCollection<PackageRepositoryBuildEntry> BuildCollection;

        public LiteDbPackageRepository(LiteCollection<PackageRepositoryBuildEntry> packageCollection) {
            if ((this.BuildCollection = packageCollection) == null) {
                throw new ArgumentNullException(nameof(packageCollection));
            }
        }

        public Build FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch) {
            var package = BuildCollection.FindOne(p => p.Name == packageName);

            var build = package?.Builds.FirstOrDefault(CreatePackageFilter(version, side, arch));
            return build;
        }

        public IEnumerable<Build> FetchBuilds(string packageName, VersionSpec versionSpec) {
            var packageEntry = BuildCollection.FindOne(b => b.Name == packageName);
            if (packageEntry == null) {
                return null;
            }
            var package = packageEntry;
            var builds = package.Builds
                .Select(build => new { build, version = build.Version })
                .Where(b => versionSpec.Satisfies(b.version))
                .OrderByDescending(build => build.version)
                .Select(b => b.build)
                .ToArray();
            return builds;
        }

        public IEnumerable<Build> FetchPackageBuilds(string packageName) {
            var builds = BuildCollection.FindOne(p => p.Name == packageName);
            if (package == null) {
                return null;
            }
            return builds.ToArray();
        }

        public IEnumerable<Build> FetchPackageList() {
            return BuildCollection.FindAll().Select(b => (Build)b).ToArray();
        }

        public IEnumerable<Build> FetchPackageList(DateTime updatedAfter) {
            return BuildCollection.FindAll().Select(b => (Build)b).ToArray();
        }

        private Func<Build, bool> CreatePackageFilter(
            SemanticVersion version,
            CompatibilitySide side,
            Arch arch
            ) {
            return b =>
                b.Version == version
                    && b.Side == side
                    && b.Arch == arch
                ;
        }

        private Package RegisterBuildSynchronous(Build build) {
            var existingPackage = BuildCollection.FindOne(p => p.Name == build.PackageName);
            if (existingPackage == null) {
                throw new KeyNotFoundException($"Participant package \"{build.PackageName}\" not yet registered");
            }
            var withEquivalentVersion = existingPackage.Builds.Where(b => b.Version.FromDTO() == build.Version);
            existingPackage.Builds.Add(build.ToDTO());
            BuildCollection.Update(existingPackage);
            return ((Net.DTO.Package)existingPackage).FromDTO();
        }

        /// <summary>
        ///     Registers a build into the system, or updates it.
        /// </summary>
        /// <param name="build">The build to register.</param>
        /// <returns>The package into which the build was registered</returns>
        public Package RegisterBuild(Build build) {
            return RegisterBuildSynchronous(build);
        }

        public Package RegisterPackage(String packageName, IEnumerable<Author> authors) {
            var existingPackage = BuildCollection.FindOne(p => p.Name == packageName);
            if (existingPackage != null) {
                existingPackage.Authors = authors.Select(DTOTranslationX.ToDTO).ToList();
                BuildCollection.Update(existingPackage);
                return existingPackage;
            }
            var newPackage = new PackageRepositoryBuildEntry {
                Name = packageName,
                Authors = authors.Select(DTOTranslationX.ToDTO).ToList(),
                Builds = Enumerable.Empty<Net.DTO.Build>().ToList(),
            };
            BuildCollection.Insert(newPackage);
            return newPackage;
        }

        public bool DeletePackage(string packageName) {
            if (String.IsNullOrWhiteSpace(packageName)) {
                throw new ArgumentException("Argument is null or whitespace", nameof(packageName));
            }
            return BuildCollection.Delete(p => p.Name == packageName) > 0;
        }

        public class PackageRepositoryBuildEntry {
            public PackageRepositoryBuildEntry() {
            }

            public PackageRepositoryBuildEntry(Build build) {
                this.Build = build;
                this.PackageName = Build.PackageName;
                this.Version = this.Build.Version;
                this.Side = this.Build.Side;
                this.Arch = this.Build.Arch;
            }

            [BsonId]
            public string PackageName { get; set; }

            [BsonField]
            public Build @Build { get; set; }

            public static implicit operator PackageRepositoryBuildEntry(Build build) => new PackageRepositoryBuildEntry(build);

            public static implicit operator Build(PackageRepositoryBuildEntry entry) {
                return entry.@Build;
            }
        }
    }
}
