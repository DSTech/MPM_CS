using MPM.Types;

namespace MPM.Core.Instances.Cache {
    public interface ICacheNamingConventionProvider {
        string GetNameForPackageArchive(string packageName, string packageArchId, SemVersion packageVersion, CompatibilitySide packageSide);
        string GetNameForPackageArchive(Build package);
        string GetNameForPackageUnarchived(Build package);
    }
}