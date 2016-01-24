using System.Collections.Generic;
using System.Linq;

namespace MPM.Core.Instances.Installation {
    public class ArchInstallationProcedure : IArchInstallationProcedure {
        public ArchInstallationProcedure(IEnumerable<ArchInstallationOperation> operations) {
            this.Operations = operations;
        }

        public IEnumerable<ArchInstallationOperation> Operations { get; }

        public IFileMap GenerateOperations() => FileMap.MergeOrdered(Operations.Select(op => op.GenerateOperations()));
    }
}
