using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using semver.tools;

namespace MPM.Core.Instances.Installation.Scripts {
	public class SourcelessFileDeclaration : IFileDeclaration {
		public string PackageName { get; set; }

		public SemanticVersion PackageVersion { get; set; }

		public string Description { get; set; }

		public byte[] Hash => null;

		public string Source => null;

		public SourcelessType @Type { get; set; }

		public IReadOnlyCollection<string> Targets { get; set; }

		public void EnsureCached(string packageCachedName, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
			return;
		}

		public IReadOnlyDictionary<string, IReadOnlyCollection<IFileOperation>> GenerateOperations() {
			var result = new Dictionary<string, IReadOnlyCollection<IFileOperation>>();
			foreach (var target in Targets) {
				result.Add(target, new[] {
					new ClaimFileOperation(this.PackageName, this.PackageVersion),
				});
			}
			return result;
		}

		public override string ToString() {
			return $"{nameof(SourcelessFileDeclaration)} <{PackageName}:{PackageVersion}> => [\n{String.Join(",\n", Targets.Select(t => $"  {t}"))}\n]";
		}
	}
}
