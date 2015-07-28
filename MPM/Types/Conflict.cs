namespace MPM.Types {

	public class Conflict {

		public Conflict(ConflictCondition condition, ConflictResolution resolution) {
			this.Condition = condition;
			this.Resolution = resolution;
		}

		public ConflictCondition Condition { get; }
		public ConflictResolution Resolution { get; }
	}
}
