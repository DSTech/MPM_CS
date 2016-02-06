using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Info;
using MPM.Types;

namespace MPM.Core.Dependency {
    public class InstanceConfiguration {
        public InstanceConfiguration() {
        }

        public InstanceConfiguration(IEnumerable<Build> packages) {
            this.Packages = packages.ToList();
        }

        public static InstanceConfiguration Empty { get; } = new InstanceConfiguration();
        public List<Build> Packages { get; set; } = new List<Build>();
    }
}
