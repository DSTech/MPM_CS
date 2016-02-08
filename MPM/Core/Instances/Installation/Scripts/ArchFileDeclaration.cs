using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using MPM.Types;

namespace MPM.Core.Instances.Installation.Scripts {
    public class ArchFileDeclaration : IFileDeclaration {
        private IArchInstallationProcedure installationProcedure { get; set; }
        public string PackageName { get; set; }

        public MPM.Types.SemVersion PackageVersion { get; set; }

        public string Description { get; set; }

        public Hash @Hash => null;

        public string Source { get; set; }

        public string ArchName { get; set; }

        public IReadOnlyCollection<string> Targets => new string[0];

        public void EnsureCached(string packageCachedName, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            var archResolver = protocolResolver.GetArchResolver();
            this.installationProcedure = archResolver.EnsureCached(ArchName, PackageVersion, cacheManager, protocolResolver);
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IFileOperation>> GenerateOperations() => installationProcedure.GenerateOperations();

        public override string ToString() {
            return $"{nameof(ArchFileDeclaration)} <{PackageName}:{PackageVersion}> =>\n  {Source} => [\n{String.Join(",\n", Targets.Select(t => $"    {t}"))}\n  ]";
        }
    }
}
