using System.Linq;
using MPM.Net.DTO;

namespace MPM.Net {
	public static class PackageConflictExtensions {
		public static bool CheckPackageConflict(this PackageConflict packageConflict, string[] packages, string[] interfaces) {
			if(packageConflict.Condition == null) {
				return false;
			}
			return packageConflict.Condition.CheckPackageConflictCondition(packages, interfaces);
		}
		public static bool CheckPackageConflictCondition(this ConflictCondition conflictCondition, string[] packages, string[] interfaces) {
			if (conflictCondition.Package != null && !packages.Contains(conflictCondition.Package)) {
				return false;
			}
			if (conflictCondition.Interface != null && !interfaces.Contains(conflictCondition.Interface)) {
				return false;
			}
			bool anyOrs;
			if (conflictCondition.Or == null) {
				anyOrs = false;
			} else {
				anyOrs = conflictCondition.Or.Any(andEntry => andEntry.CheckPackageConflictCondition(packages, interfaces));
			}
			if(anyOrs == false) {
				return false;
			}
			bool allAnds;
			if (conflictCondition.And == null) {
				allAnds = true;
			} else {
				allAnds = conflictCondition.And.All(andEntry => andEntry.CheckPackageConflictCondition(packages, interfaces));
			}
			if(allAnds == false) {
				return false;
			}
			return true;
		}
	}
}
