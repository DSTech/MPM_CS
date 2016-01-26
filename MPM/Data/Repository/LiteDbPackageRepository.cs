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

        private readonly LiteCollection<BuildEntry> BuildCollection;

        public LiteDbPackageRepository(LiteCollection<BuildEntry> packageCollection) {
            if ((this.BuildCollection = packageCollection) == null) {
                throw new ArgumentNullException(nameof(packageCollection));
            }
        }

        public Build FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch) {
            var build = BuildCollection.FindOne(p => p.PackageName == packageName
                && p.ArchId == arch.Id
                && p.SideString == side.ToString()
                && p.VersionString == version.ToString());
            return build;
        }

        public IEnumerable<Build> FetchBuilds(string packageName, VersionSpec versionSpec) {
            var buildEntries = BuildCollection.Find(b => b.PackageName == packageName);
            if (buildEntries == null) {
                return null;
            }
            return buildEntries
                .Select(b => new {
                    @BuildEntry = b,
                    @Version = SemanticVersion.Parse(b.VersionString)
                })
                .Where(bv => versionSpec.Satisfies(bv.@Version))
                .OrderByDescending(bv => bv.@Version)
                .Select(bv => bv.@BuildEntry.@Build)
                .ToArray();
        }

        public IEnumerable<Build> FetchPackageBuilds(string packageName) {
            var builds = BuildCollection.Find(p => p.PackageName == packageName);
            return builds
                .OrderByDescending(b => SemanticVersion.Parse(b.VersionString))
                .Select(b => b.Build)
                .ToArray();
        }

        public IEnumerable<Build> FetchPackageList() {
            return BuildCollection.FindAll().Select(b => b.Build).ToArray();
        }

        public IEnumerable<Build> FetchPackageList(DateTime updatedAfter) {
            return BuildCollection.FindAll().Select(b => b.Build).ToArray();
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

        /// <summary>
        ///     Registers a build into the system, or updates it.
        /// </summary>
        /// <param name="build">The build to register.</param>
        /// <returns>The package into which the build was registered</returns>
        public Build RegisterBuild(Build build) {
            this.BuildCollection.Insert(build);
            return build;
        }

        public bool DeletePackage(string packageName) {
            if (String.IsNullOrWhiteSpace(packageName)) {
                throw new ArgumentException("Argument is null or whitespace", nameof(packageName));
            }
            return BuildCollection.Delete(p => p.PackageName == packageName) > 0;
        }

        public class BuildEntry {

            public BuildEntry() {
            }

            public BuildEntry(Build build) {
                this.Build = build;
                this.PackageName = Build.PackageName;
                this.ArchId = this.Build.Arch.Id;
                this.SideString = this.Build.Side.ToString();
                this.VersionString = this.Build.Version.ToString();
                this.Id = string.Join(",", this.PackageName, this.ArchId, this.SideString, this.VersionString);
            }

            [BsonId]
            public string Id { get; set; }

            [BsonIndex]
            public string PackageName { get; set; }

            [BsonIndex]
            public string ArchId { get; set; }

            [BsonIndex]
            public string SideString { get; set; }

            [BsonIndex]
            public string VersionString { get; set; }

            [BsonField]
            public Build @Build { get; set; }

            public static implicit operator BuildEntry(Build build) => new BuildEntry(build);

            public static implicit operator Build(BuildEntry entry) => entry.@Build;
        }
    }
}
