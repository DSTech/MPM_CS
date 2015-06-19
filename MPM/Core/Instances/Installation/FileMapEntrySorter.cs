using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;

namespace MPM.Core.Instances.Installation {
	public class FileMapEntrySorter {
		public FileMap Sort(IReadOnlyCollection<Tuple<NamedBuild, IFileMap>> packageMaps) {
			IReadOnlyCollection<NamedBuild> sortedConfiguration = new Resolver()
				.SortBuilds(
					packageMaps.Select(
						mapping => mapping.Item1
					)
					.Solidify()
				)
				.ToArray();
			var packageMapping = packageMaps.ToDictionary(nb => nb.Item1, nb => nb.Item2);
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
