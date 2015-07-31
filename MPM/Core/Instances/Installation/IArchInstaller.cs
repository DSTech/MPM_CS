using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Installation.Scripts;

namespace MPM.Core.Instances.Installation {

	public interface IArchInstaller {

		IFileMap GenerateOperations(IEnumerable<ArchInstallationOperation> cacheEntries);
	}
}
