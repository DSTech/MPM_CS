namespace MPM.Types {

	public class DependencyConflictResolution {

		public DependencyConflictResolution(ForcedDependencySet force, DeclinedDependencySet decline) {
			this.Force = force;
			this.Decline = decline;
		}

		public ForcedDependencySet Force { get; }
		public DeclinedDependencySet Decline { get; }
	}
}
