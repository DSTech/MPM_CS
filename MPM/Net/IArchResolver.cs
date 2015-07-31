using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Core.Protocols;

namespace MPM.Net {

	public interface IArchResolver {

		IArchInstallationProcedure EnsureCached(string packageName, ICacheManager cacheManager, IProtocolResolver protocolResolver);
	}
}
