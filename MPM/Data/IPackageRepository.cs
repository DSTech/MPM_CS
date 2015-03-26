using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPM.Net.DTO;
using semver.tools;

namespace MPM.Data {
	/// <summary>
	/// Provides a package repository interface to allow the loading of package information.
	/// </summary>
	public interface IPackageRepository {
		/// <summary>
		/// Fetches a list of all packages, but is not required to return any particular data (eg. authors) for each package.
		/// Should be used to find packages for which further information may be looked up via <see cref="FetchPackage(string)"/> or <see cref="FetchBuild(string, SemanticVersion)"/>.
		/// </summary>
		/// <returns>Multiple <see cref="Package"/> instances containing, at minimum, the name of the package.</returns>
		IObservable<Package> FetchPackageList();
		/// <summary>
		/// Similar to <see cref="FetchPackageList"/>, fetches only packages which have changed since the specified <paramref name="updatedAfter"/> time.
		/// May contain more packages than strictly those updated after the specified time, depending on the repository's server-side implementation.
		/// </summary>
		/// <param name="updatedAfter"></param>
		/// <returns>Multiple <see cref="Package"/> instances containing, at minimum, the name of the package.</returns>
		IObservable<Package> FetchPackageList(DateTime updatedAfter);
		Task<Package> FetchPackage(String packageName);
		/// <summary>
		/// Looks up a particular build of a package.
		/// </summary>
		/// <param name="packageName"></param>
		/// <param name="version"></param>
		/// <returns>
		/// A <see cref="Build"/> instance matching the specified <paramref name="packageName"/> and <paramref name="version"/>. Null when no package of the specified name was found.
		/// Builds must be returned in descending order of version.
		/// </returns>
		Task<Build> FetchBuild(String packageName, SemanticVersion version);
		/// <summary>
		/// Looks up a package for any versions matching a specifier.
		/// </summary>
		/// <param name="packageName">Name of the package to look up</param>
		/// <param name="versionSpec">The version specification to check against before returning a build</param>
		/// <returns>
		/// A <see cref="Package"/> instance containing only builds which comply with the <paramref name="versionSpec"/> given. Null when no package of the specified name was found.
		/// Builds must be returned in descending order of version.
		/// </returns>
		Task<Package> FetchBuilds(String packageName, VersionSpec versionSpec);
	}
}
