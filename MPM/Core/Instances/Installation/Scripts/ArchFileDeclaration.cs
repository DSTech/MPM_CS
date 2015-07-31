using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using MPM.Extensions;
using MPM.Net;
using semver.tools;

namespace MPM.Core.Instances.Installation.Scripts {

	public class ArchFileDeclaration : IFileDeclaration {
		public string PackageName { get; set; }

		public SemanticVersion PackageVersion { get; set; }

		public string Description { get; set; }

		public byte[] Hash => null;

		public string Source { get; set; }

		public IReadOnlyCollection<string> Targets => new string[0];

		private IArchInstallationProcedure installationProcedure { get; set; }

		public void EnsureCached(string packageCachedName, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
			IArchResolver archResolver = protocolResolver.GetArchResolver();
			this.installationProcedure = archResolver.EnsureCached(PackageName, cacheManager, protocolResolver);
		}

		public IReadOnlyDictionary<string, IReadOnlyCollection<IFileOperation>> GenerateOperations() {
			return installationProcedure.GenerateOperations();
		}

		public override string ToString() {
			return $"{nameof(ArchFileDeclaration)} <{PackageName}:{PackageVersion}> =>\n  {Source} => [\n{String.Join(",\n", Targets.Select(t => $"    {t}"))}\n  ]";
		}
	}
}
