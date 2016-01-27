using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Protocols;

namespace MPM.Net {
    public interface IArchResolver {
        IArchInstallationProcedure EnsureCached(string archName, SemVer.Version archVersion, ICacheManager cacheManager, IProtocolResolver protocolResolver);
    }
}
