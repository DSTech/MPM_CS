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

        public string GetNameForArchVersionDetails(string archName, SemVersion archVersion) {
            return $"arch/{archName}/{archVersion}/versionDetails.json";
        }

        public string GetNameForArchAssetIndex(string archName, SemVersion archVersion) {
            return $"arch/{archName}/{archVersion}/assetIndex.json";
        }

        public string GetNameForArchClient(string archName, SemVersion archVersion) {
            return $"minecraft_{archVersion}_client.jar";
        }

        public string GetNameForArchServer(string archName, SemVersion archVersion) {
            return $"minecraft_{archVersion}_server.jar";
        }

        public string GetNameForArchLibrary(string package, string name, string version, string nativeClause) {
            return $"{package}_{name}_{version}{nativeClause}";
        }

        public string GetNameForArchAsset(string archName, SemVersion archVersion, string assetPath) {
            return $"arch/{archName}/{archVersion}/asset/{assetPath}";
        }
    }
}