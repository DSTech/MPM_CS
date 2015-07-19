using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Instances;
using semver.tools;

namespace MPM.CLI {
	public class InitActionProvider {
		public void Init(SemanticVersion instanceArch, InstanceSide instanceSide, InstancePlatform instancePlatform, string instancePath) {
			var instance = new Instance(instancePath) {
				Name = $"{instanceArch}_{instanceSide}_{instancePlatform}",//TODO: make configurable and able to be immediately registered upon creation
				LauncherType = typeof(MinecraftLauncher),//TODO: make configurable via instanceSide, instanceArch and able to be overridden
				Configuration = InstanceConfiguration.Empty,
			};
			using (var meta = instance.GetDbMeta()) {
				meta.Set<String>("testMetaKeyD", "testMetaValue");
			}
		}
	}
}
