namespace MPM.Types {

	public class ConflictResolution {

		public ConflictResolution(DependencyConflictResolution dependency, InstallationConflictResolution installation) {
			this.Dependency = dependency;
			this.Installation = installation;
		}

		public DependencyConflictResolution Dependency { get; set; }
		public InstallationConflictResolution Installation { get; set; }
	}
}
