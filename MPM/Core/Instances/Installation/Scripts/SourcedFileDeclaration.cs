using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using MPM.Types;

namespace MPM.Core.Instances.Installation.Scripts {
    public class SourcedFileDeclaration : IFileDeclaration {
        private string packageCachedName { get; set; }
        public string PackageName { get; set; }
        public Arch PackageArch { get; set; }

        public MPM.Types.SemVersion PackageVersion { get; set; }

        public MPM.Types.CompatibilitySide PackageSide { get; set; }

        public string Description { get; set; }

        public Hash @Hash { get; set; }

        public string Source { get; set; }

        public IReadOnlyCollection<string> Targets { get; set; }

        public void EnsureCached(string packageCachedName, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            if (!cacheManager.Contains(packageCachedName)) {
                throw new Exception($"Cache did not contain package \"{packageCachedName}\"");
            }
            this.packageCachedName = packageCachedName;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IFileOperation>> GenerateOperations() {
            var result = new Dictionary<string, IReadOnlyCollection<IFileOperation>>();
            foreach (var target in Targets) {
                result.Add(target, new[] {
                    new ExtractFileOperation(this.PackageName, this.PackageVersion, packageCachedName, Source),//TODO: Allow ExtractFileOperation to check hashes
                });
            }
            return result;
        }

        public override string ToString() {
            return $"{nameof(SourcedFileDeclaration)} <{PackageName}:{PackageVersion}> =>\n  {Source} => [\n{String.Join(",\n", Targets.Select(t => $"    {t}"))}\n  ]";
        }
    }
}
