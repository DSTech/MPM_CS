using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MPM.Types;

namespace MPM.Data.Repository {
    /// <summary>
    ///     Provides a package repository interface to allow the loading of package information.
    /// </summary>
    public interface IPackageRepository {
        /// <summary>
        ///     Looks up a package for any versions matching a specifier.
        /// </summary>
        /// <param name="packageName">Name of the package to look up</param>
        /// <param name="versionSpec">The version specification to check against before returning a build</param>
        /// <returns>
        ///     An enumerable of <see cref="Build"/> instances containing only builds which comply with the <paramref name="versionSpec" />
        ///     given. Null when no package of the specified name was found.
        ///     Builds must be returned in descending order of version, with side-specific returned before universal packages.
        /// </returns>
        [CanBeNull] IEnumerable<Build> FetchBuilds();

        [CanBeNull] IEnumerable<Build> FetchBuilds([NotNull] DateTime updatedAfter);
    }
}
