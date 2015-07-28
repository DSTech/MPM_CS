using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Info;
using MPM.Types;

namespace MPM.Core.Dependency {

	public class InstanceConfiguration {

		public InstanceConfiguration(IEnumerable<Build> packages) {
			this.Packages = packages.ToArray();
		}

		public static InstanceConfiguration Empty { get; } = new InstanceConfiguration(Enumerable.Empty<Build>());
		public IReadOnlyCollection<Build> Packages { get; }
	}
}
