using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;

namespace MPM.Net.Protocols.Minecraft {
    public class MetaArchInstaller : IArchResolver {
        public IArchInstallationProcedure EnsureCached(string packageName, MPM.Types.SemVersion archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            //TODO: Handle packageName / archVersion sanely
            switch (packageName.Split('_').FirstOrDefault()) {
                case "minecraft":
                    return new MinecraftArchInstaller().EnsureCached(new MPM.Types.SemVersion(packageName.Split('_').Skip(1).First()), cacheManager, protocolResolver);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
