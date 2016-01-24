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
    public class MetaArchInstaller : IArchResolver {
        public IArchInstallationProcedure EnsureCached(string packageName, SemanticVersion archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            switch (packageName) {
                case "minecraft":
                    return new MinecraftArchInstaller().EnsureCached(archVersion, cacheManager, protocolResolver);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
