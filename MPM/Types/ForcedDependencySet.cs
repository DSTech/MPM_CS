using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {

	public class ForcedDependencySet {

		public ForcedDependencySet(IEnumerable<string> packageNames, IEnumerable<string> interfaceNames) {
			this.PackageNames = packageNames.ToArray();
			this.InterfaceNames = interfaceNames.ToArray();
		}

		public IReadOnlyCollection<String> PackageNames { get; }
		public IReadOnlyCollection<String> InterfaceNames { get; }
	}
}