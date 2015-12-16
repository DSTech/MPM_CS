using System;
using System.Collections.Generic;

namespace MPM.Net.DTO {
	public class ConflictCondition {
		public List<ConflictCondition> And { get; set; }
		public List<ConflictCondition> Or { get; set; }
		public String Interface { get; set; }
		public String Package { get; set; }
	}
}
