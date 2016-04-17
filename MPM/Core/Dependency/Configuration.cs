using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using MPM.Types;

namespace MPM.Core.Dependency {
    public static class ConfigurationExtensions {
        //Returns conflicts caused by particular builds
        //LookupBuild should return a series of builds that match the package specification
        [NotNull]
        public static IEnumerable<Tuple<PackageSpec, Build, Conflict>> FindConflicts(this Configuration configuration, Func<PackageSpec, Build[]> lookupMatchingBuilds) {
            if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (lookupMatchingBuilds == null) {
                throw new ArgumentNullException(nameof(lookupMatchingBuilds));
            }
            foreach (var package in configuration.Packages) {
                var matchingBuilds = lookupMatchingBuilds(package).DenullArray();
                if (matchingBuilds.Length == 0) {
                    throw new Exception("Package details could not be found");
                }
                Debug.Assert(matchingBuilds.Length > 0);

                var build = matchingBuilds
                    .OrderByDescending(b => b.Version)
                    .ThenByDescending(b => b.Side != CompatibilitySide.Universal)
                    .First();

                var conflicts = build.FindConflicts(
                    configuration
                        .Packages
                        .Where(spec => spec.Name != build.PackageName)
                        .ToArray(),
                    lookupMatchingBuilds
                    );
                foreach (var conflict in conflicts) {
                    yield return Tuple.Create(package, build, conflict);
                }
            }
        }

        //Returns any conflicts triggered by a particular set of packages interacting with the given package
        //LookupBuild should return a package with exactly one build
        //These interactions are one-way: other packages must check their conflicts with the source as well
        public static IEnumerable<Conflict> FindConflicts([NotNull] this Build build, [NotNull] PackageSpec[] otherPackageSpecs, [NotNull] Func<PackageSpec, Build[]> lookupMatchingBuilds) {
            if (otherPackageSpecs == null) {
                throw new ArgumentNullException(nameof(otherPackageSpecs));
            }
            if (lookupMatchingBuilds == null) {
                throw new ArgumentNullException(nameof(lookupMatchingBuilds));
            }
            var highestSpecs = otherPackageSpecs
                .Select(spec => {
                    var matchingBuilds = lookupMatchingBuilds(spec).DenullArray();
                    Debug.Assert(matchingBuilds!= null && matchingBuilds.Length > 0);
                    return matchingBuilds
                        .OrderByDescending(b => b.Version)
                        .ThenByDescending(b => b.Side != CompatibilitySide.Universal)
                        .First();
                })
                .ToArray();
            var packageNames = highestSpecs
                .Select(b => b.PackageName)
                .ToArray();
            var interfaceNames = highestSpecs
                .SelectMany(b => b.Interfaces)
                .Select(b => b.InterfaceName)
                .ToArray();
            foreach (var conflict in build.Conflicts) {
                if (conflict.CheckPackageConflict(packageNames, interfaceNames)) {
                    yield return conflict;
                }
            }
        }
    }

    public class Configuration {
        public Configuration() {
        }

        public Configuration([NotNull] IEnumerable<PackageSpec> packageSpecifications, CompatibilitySide side = CompatibilitySide.Universal) {
            if (packageSpecifications == null) { throw new ArgumentNullException(nameof(packageSpecifications)); }
            this.Packages = packageSpecifications.ToList();
            this.Side = side;
        }

        public List<PackageSpec> Packages { get; set; } = new List<PackageSpec>();
        public CompatibilitySide Side { get; set; } = CompatibilitySide.Universal;

        public static Configuration Empty { get; } = new Configuration(Enumerable.Empty<PackageSpec>());
    }
}
