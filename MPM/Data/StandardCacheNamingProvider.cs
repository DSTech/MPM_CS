using System;
using MPM.Core.Instances.Cache;
using MPM.Types;

namespace MPM.Data {
    public class StandardCacheNamingProvider : ICacheNamingConventionProvider {
        private string BaseNameForPackage(string packageName, string packageArchId, SemVersion packageVersion, CompatibilitySide packageSide) =>
            $"{packageName}_{packageArchId}_{packageVersion}_{packageSide}";

        public string GetNameForPackageArchive(string packageName, string packageArchId, SemVersion packageVersion, CompatibilitySide packageSide) {
            return $"package/{BaseNameForPackage(packageName, packageArchId, packageVersion, packageSide)}.mpk";
        }

        public string GetNameForPackageArchive(Build package) {
            return GetNameForPackageArchive(package.PackageName, package.Arch.ToString(), package.Version, package.Side);
        }

        public string GetNameForPackageUnarchived(string packageName, string packageArchId, SemVersion packageVersion, CompatibilitySide packageSide) {
            return $"unpacked/{BaseNameForPackage(packageName, packageArchId, packageVersion, packageSide)}.zip";
        }

        public string GetNameForPackageUnarchived(Build package) {
            return GetNameForPackageUnarchived(package.PackageName, package.Arch.ToString(), package.Version, package.Side);
        }
    }
}