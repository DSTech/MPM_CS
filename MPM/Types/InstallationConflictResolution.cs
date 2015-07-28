using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {

	public class InstallationConflictResolution {

		public InstallationConflictResolution(IEnumerable<string> packageNames) {
			this.PackageNames = packageNames.ToArray();
		}

		public IReadOnlyCollection<String> PackageNames { get; }
	}
}
