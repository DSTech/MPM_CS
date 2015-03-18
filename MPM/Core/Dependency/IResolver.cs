using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;

namespace MPM.Core.Dependency {
	/// <summary>
	/// Looks up a package, returning named builds qualifying for the specification.
	/// Must return in descending order of version.
	/// </summary>
	/// <param name="packageSpec">Specification to look up</param>
	/// <returns>Builds in descending order of version</returns>
	/// <remarks>Should be converted to IQueryable to allow optimized behavior with constraint lookup</remarks>
	public delegate IEnumerable<NamedBuild> PackageSpecLookup(PackageSpec packageSpec);
	public static class IResolverExtensions {
	}
	public interface IResolver {
		/// <summary>
		/// Returns a configuration containing the non-manual dependencies required for installation.
		/// All targetted "Manual" package specifications must be included in the output.
		/// Non-manual specifications may be included in the target to specify that they are suggested.
		/// In the event that non-manuals are included, they may be used to fulfill dependencies,
		/// but if they are superceded by a higher version, the supercedant will not be in the resultant configuration.
		/// </summary>
		/// <param name="target">
		/// Configuration containing all packages selected for installation, and optionally any that are already installed
		/// </param>
		/// <param name="lookupPackageSpec">
		/// Looks up a package, returning named builds in descending order of version.
		/// </param>
		/// <returns>Configuration with dependencies fulfilled</returns>
		Configuration Resolve(Configuration target, PackageSpecLookup lookupPackageSpec);
		/// <summary>
		/// Returns a build that qualifies with the specified dependency constraints.
		/// </summary>
		/// <param name="packageSpec">Specification for the package for which to find a build</param>
		/// <param name="lookupPackageSpec">Callback for which the system may look up a package from the index, optionally allowing caching to take place at higher layers</param>
		/// <param name="constraints">An enumerable of constraints requiring fulfillment in order to produce a result</param>
		/// <param name="resolutionMode">Specifier declaring which behavior the resolver should follow when attempting to find qualifying builds</param>
		/// <returns>A build satisfying the spec, qualifying with all constraints, resolved with the specified behavior</returns>
		NamedBuild ResolveDependency(PackageSpec packageSpec, PackageSpecLookup lookupPackageSpec, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest);
	}
}
