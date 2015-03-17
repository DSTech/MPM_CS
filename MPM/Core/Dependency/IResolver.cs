using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <returns>Configuration with dependencies fulfilled</returns>
		Configuration Resolve(Configuration target);
	}
}
