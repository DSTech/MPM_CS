using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;

namespace MPM.Core.Instances.Installation {
	public class FileMapEntrySorter {
		public FileMap Sort(IReadOnlyCollection<Tuple<NamedBuild, IFileMap>> packageMapping) {
			return Sort(packageMapping.ToDictionary(mapping => mapping.Item1, mapping => mapping.Item2));
		}
        public FileMap Sort(IReadOnlyDictionary<NamedBuild, IFileMap> packageMapping) {
			IReadOnlyCollection<NamedBuild> sortedConfiguration = new Resolver()
				.SortBuilds(
					packageMapping.Select(
						mapping => mapping.Key
					)
					.Solidify()
				)
				.ToArray();
			return Sort(sortedConfiguration, packageMapping);
		}
		public FileMap Sort(IReadOnlyCollection<NamedBuild> sortedConfiguration, IReadOnlyCollection<Tuple<NamedBuild, IFileMap>> packageMapping) {
			return Sort(sortedConfiguration, packageMapping.ToDictionary(mapping => mapping.Item1, mapping => mapping.Item2));
		}
		public FileMap Sort(IReadOnlyCollection<NamedBuild> sortedConfiguration, IReadOnlyDictionary<NamedBuild, IFileMap> packageMapping) {
			var result = new FileMap();
			//Register all operations from each package in order of package dependency
			foreach (var build in sortedConfiguration) {
				foreach (var fileMapEntry in packageMapping[build]) {
					foreach (var operation in fileMapEntry.Value) {
						result.Register(fileMapEntry.Key, operation);
					}
				}
			}
			return result;
		}
	}
}
