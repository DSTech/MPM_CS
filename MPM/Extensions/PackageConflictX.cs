using System.Linq;

namespace MPM.Types {
    public static class PackageConflictX {
        public static bool CheckPackageConflict(this Conflict packageConflict, string[] packages, string[] interfaces) {
            if (packageConflict.Condition == null) {
                return false;
            }
            return packageConflict.Condition.CheckPackageConflictCondition(packages, interfaces);
        }

        public static bool CheckPackageConflictCondition(this ConflictCondition conflictCondition, string[] packages, string[] interfaces) {
            if (conflictCondition.PackageName != null && !packages.Contains(conflictCondition.PackageName)) {
                return false;
            }
            if (conflictCondition.InterfaceName != null && !interfaces.Contains(conflictCondition.InterfaceName)) {
                return false;
            }
            bool anyOrs;
            if (conflictCondition.Or == null) {
                anyOrs = false;
            } else {
                anyOrs = conflictCondition.Or.Any(andEntry => andEntry.CheckPackageConflictCondition(packages, interfaces));
            }
            if (!anyOrs) {
                return false;
            }
            bool allAnds;
            if (conflictCondition.And == null) {
                allAnds = true;
            } else {
                allAnds = conflictCondition.And.All(andEntry => andEntry.CheckPackageConflictCondition(packages, interfaces));
            }
            if (!allAnds) {
                return false;
            }
            return true;
        }
    }
}
