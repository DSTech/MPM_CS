using System;
using System.Runtime.Serialization;

namespace MPM.Core.Instances {
	[Serializable]
	internal class InstallationException : Exception {
		public InstallationException() {
		}

		public InstallationException(string message) : base(message) {
		}

		public InstallationException(string message, Exception innerException) : base(message, innerException) {
		}

		protected InstallationException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}
