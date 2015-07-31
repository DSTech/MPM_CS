using System.Collections.Generic;
using System.Linq;

namespace MPM.Core.Instances.Installation {

	public class ArchInstallationProcedure : IArchInstallationProcedure {
		public IEnumerable<ArchInstallationOperation> operations { get; set; }

		public IFileMap GenerateOperations() => FileMap.MergeOrdered(operations.Select(op => op.GenerateOperations()));
	}
}
