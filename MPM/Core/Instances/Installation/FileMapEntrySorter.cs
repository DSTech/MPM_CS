using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Dependency;
using MPM.Types;

namespace MPM.Core.Instances.Installation {
    public class FileMapEntrySorter {
        public IFileMap Sort(IReadOnlyCollection<Tuple<Build, IFileMap>> packageMapping) {
            return Sort(packageMapping.ToDictionary(mapping => mapping.Item1, mapping => mapping.Item2));
        }

        public IFileMap Sort(IReadOnlyDictionary<Build, IFileMap> packageMapping) {
            IReadOnlyCollection<Build> sortedConfiguration = DependencyResolver.SortBuilds(
                    packageMapping.Select(
                        mapping => mapping.Key
                        )
                        .Solidify()
                )
                .ToArray();
            return Sort(sortedConfiguration, packageMapping);
        }

        public IFileMap Sort(IReadOnlyCollection<Build> sortedConfiguration, IReadOnlyCollection<Tuple<Build, IFileMap>> packageMapping) {
            return Sort(sortedConfiguration, packageMapping.ToDictionary(mapping => mapping.Item1, mapping => mapping.Item2));
        }

        public IFileMap Sort(IReadOnlyCollection<Build> sortedConfiguration, IReadOnlyDictionary<Build, IFileMap> packageMapping) {
            //Register all operations from each package in order of package dependency
            return FileMap.MergeOrdered(sortedConfiguration.Select(b => packageMapping[b]).ToArray());
        }
    }
}
