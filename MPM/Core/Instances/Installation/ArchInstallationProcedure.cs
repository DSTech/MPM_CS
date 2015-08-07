using System.Collections.Generic;
using System.Linq;

namespace MPM.Core.Instances.Installation {

	public class ArchInstallationProcedure : IArchInstallationProcedure {
		public IEnumerable<ArchInstallationOperation> Operations { get; }

		public ArchInstallationProcedure(IEnumerable<ArchInstallationOperation> operations) {
			this.Operations = operations;
		}

		public IFileMap GenerateOperations() => FileMap.MergeOrdered(Operations.Select(op => op.GenerateOperations()));
	}
}
