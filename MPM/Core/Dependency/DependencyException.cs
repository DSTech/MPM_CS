using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Info;
using MPM.Types;

namespace MPM.Core.Dependency {
    public class DependencyException : Exception {
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

        public PackageSpec PackageSpec { get; private set; } = null;
        public Build Dependent { get; private set; } = null;

        public override string ToString() {
            return $"({this.Dependent?.PackageName ?? "<dependent>"})=>{this.PackageSpec}:\n{base.ToString()}";
        }
    }
}
