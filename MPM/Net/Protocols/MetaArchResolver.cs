using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft {
    public class MetaArchInstaller : IArchResolver {
        public IArchInstallationProcedure EnsureCached(string packageName, CompatibilitySide archSide, MPM.Types.Arch archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            switch (packageName) {
                case "minecraft":
                    return new MinecraftArchInstaller().EnsureCached(archSide, archVersion, cacheManager, protocolResolver);
                default:
                    throw new NotSupportedException($"Arch {packageName} @ {archVersion} is not supported by this version.");
            }
        }
    }
}
