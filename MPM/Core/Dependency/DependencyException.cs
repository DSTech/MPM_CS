using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Dependency {
	public class DependencyException : Exception {
		public PackageSpec PackageSpec { get; } = null;
		public NamedBuild Dependent { get; } = null;
		public DependencyException() {
		}
		public DependencyException(string message, PackageSpec packageSpec = null, NamedBuild dependent = null)
			: base(message) {
			PackageSpec = packageSpec;
			Dependent = dependent;
		}
		public DependencyException(string message, Exception innerException, PackageSpec packageSpec = null, NamedBuild dependent = null) : base(message, innerException) {
			PackageSpec = packageSpec;
			Dependent = dependent;
		}
		public override string ToString() {
			return $"({Dependent?.Name ?? "<dependent>"})=>{PackageSpec}:\n{base.ToString()}";
        }
	}
}
