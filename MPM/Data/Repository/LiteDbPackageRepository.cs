using System;
using System.Collections.Generic;
using LiteDB;
using MPM.Types;
using System.Linq;
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

        public IEnumerable<Build> FetchBuilds() {
            return BuildCollection.FindAll().Select(b => b.Build).OrderByDescending(b => b.Version).ToArray();
        }

        public IEnumerable<Build> FetchBuilds(DateTime updatedAfter) {
            return BuildCollection
                .FindAll()
                .Where(b => b.LastUpdated >= updatedAfter)
                .Select(b => b.Build)
                .ToArray();
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

        public Build UpdateBuild(Build build) {
            this.BuildCollection.Update(build);
            return build;
        }

        public Build DeleteBuild(Build build) {
            var idString = build.ToIdentifierString();
            var found = this.BuildCollection.FindById(idString);
            if (found == null) {
                return null;
            }
            if (!this.BuildCollection.Delete(idString)) {
                return found;
            }
            return found.Build;
        }

        public class BuildEntry {

            public BuildEntry() {
            }

            public BuildEntry(Build build) {
                this.Build = build;
                this.LastUpdated = DateTime.UtcNow;
                this.Id = build.ToIdentifierString();
            }

            [BsonId]
            public string Id { get; set; }

            [BsonIgnore]
            public Build @Build {
                get { return BuildJson == null ? null : JsonConvert.DeserializeObject<Build>(BuildJson); }
                set { BuildJson = JsonConvert.SerializeObject(value); }
            }

            [BsonField]
            public string BuildJson { get; set; }

            [BsonField]
            public DateTime LastUpdated { get; set; } = DateTime.MinValue.AddSeconds(1);

            public static implicit operator BuildEntry(Build build) => new BuildEntry(build);

            public static implicit operator Build(BuildEntry entry) => entry.@Build;
        }
    }
}
