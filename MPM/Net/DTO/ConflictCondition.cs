using System;

namespace MPM.Net.DTO {

	public class ConflictCondition {
		public ConflictCondition[] And { get; set; }
		public ConflictCondition[] Or { get; set; }
		public String Interface { get; set; }
		public String Package { get; set; }
	}
}
