using System;

namespace MPM.Net.DTO {
	public class ConflictResolution {
		public DependencyConflictResolution Dependencies { get; set; }
		public InstallationConflictResolution Install { get; set; }
	}
}
