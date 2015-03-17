using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Dependency {
	public class DependencyException : Exception {
		public DependencyException() {
		}
		public DependencyException(string message) : base(message) {
		}
		public DependencyException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}
