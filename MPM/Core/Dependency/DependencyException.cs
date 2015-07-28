using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Info;
using MPM.Types;

namespace MPM.Core.Dependency {

	public class DependencyException : Exception {
		public PackageSpec PackageSpec { get; private set; } = null;
		public Build Dependent { get; private set; } = null;

		public DependencyException() {
		}

		public DependencyException(string message, PackageSpec packageSpec = null, Build dependent = null)
			: base(message) {
			this.PackageSpec = packageSpec;
			this.Dependent = dependent;
		}

		public DependencyException(string message, Exception innerException, PackageSpec packageSpec = null, Build dependent = null) : base(message, innerException) {
			this.PackageSpec = packageSpec;
			this.Dependent = dependent;
		}

		public override string ToString() {
			return String.Format("({0})=>{1}:\n{2}", this.Dependent?.PackageName ?? "<dependent>", this.PackageSpec, base.ToString());
		}
	}
}
