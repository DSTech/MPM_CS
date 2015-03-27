using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Data;
using MPM.Net.DTO;

namespace MPM.Core.Dependency {
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
		/// <param name="packageRepository">
		/// Repository (Preferably cached, due to resolution behavior requiring many requests) which will source the information for resolution.
		/// </param>
		/// <returns>Configuration with dependencies fulfilled</returns>
		Task<ResolvedConfiguration> Resolve(Configuration target, IPackageRepository packageRepository);
		/// <summary>
		/// Returns a build that qualifies with the specified dependency constraints.
		/// </summary>
		/// <param name="packageSpec">Specification for the package for which to find a build</param>
		/// <param name="packageRepository">
		/// Repository (Preferably cached, due to resolution behavior requiring many requests) which will source the information for resolution.
		/// </param>
		/// <param name="constraints">An enumerable of constraints requiring fulfillment in order to produce a result</param>
		/// <param name="resolutionMode">Specifier declaring which behavior the resolver should follow when attempting to find qualifying builds</param>
		/// <exception cref="DependencyException">Thrown if no qualifying build can be found</exception>
		/// <returns>A set of builds satisfying the spec, qualifying with all constraints, resolved with the specified behavior, ordered from most to least prefered</returns>
		Task<NamedBuild[]> ResolveDependency(PackageSpec packageSpec, IPackageRepository packageRepository, PackageSide side = PackageSide.Universal, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest);
		/// <summary>
		/// A variant of ResolveDependency which should implement additional constraints on each cycle until a dependency tree can be resolved as non-null.
		/// Should attempt to resolve each subsequent build in the package spec,
		/// At most, one copy of a package may be returned from a call, so the results should be distinctly filtered from the deeper layers.
		/// The order of the results should be in topological order from least to most dependent, with all necessary, constraint-fulfilling dependencies preceeding them.
		/// Circular dependencies are to be resolved by allowing the most dependent to follow the least dependent element once in the result. If the dependency is equal, they may be in arbitrary order.
		/// </summary>
		/// <param name="packageSpec">Specification for the package for which to find a build</param>
		/// <param name="packageRepository">
		/// Repository (Preferably cached, due to resolution behavior requiring many requests) which will source the information for resolution.
		/// </param>
		/// <param name="packageSide">Specifies which side is performing resolution, determining whether or not package dependencies are to be counted</param>
		/// <param name="constraints">An enumerable of constraints requiring fulfillment in order to produce a result</param>
		/// <param name="resolutionMode">Specifier declaring which behavior the resolver should follow when attempting to find qualifying builds</param>
		/// <exception cref="DependencyException">Thrown if no qualifying dependency tree can be found</exception>
		/// <returns>
		/// A topologically-ordered array of dependencies on successful resolution back to root.
		/// Null if resolution fails.
		/// </returns>
		Task<NamedBuild[]> ResolveRecursive(PackageSpec packageSpec, IPackageRepository packageRepository, PackageSide packageSide = PackageSide.Universal, IEnumerable<DependencyConstraint> constraints = null, ResolutionMode resolutionMode = ResolutionMode.Highest);
    }
}
