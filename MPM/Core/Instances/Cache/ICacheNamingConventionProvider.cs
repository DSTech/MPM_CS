using MPM.Types;

namespace MPM.Core.Instances.Cache {
    public interface ICacheNamingConventionProvider {
        string GetNameForPackageArchive(string packageName, string packageArchId, SemVersion packageVersion, CompatibilitySide packageSide);
        string GetNameForPackageArchive(Build package);
        string GetNameForPackageUnarchived(Build package);
        string GetNameForArchVersionDetails(string archName, SemVersion archVersion);
        string GetNameForArchAssetIndex(string archName, SemVersion archVersion);
        string GetNameForArchClient(string archName, SemVersion archVersion);
        string GetNameForArchServer(string archName, SemVersion archVersion);
        string GetNameForArchLibrary(string package, string name, string version, string nativeClause);
        string GetNameForArchAsset(string archName, SemVersion archVersion, string assetPath);
    }
}