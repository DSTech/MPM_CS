using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;
using semver.tools;

namespace MPM.Net.Protocols.Minecraft {
	public class MinecraftArchInstaller {
		public IArchInstallationProcedure EnsureCached(SemanticVersion archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {

			throw new NotImplementedException();
		}
	}
}
