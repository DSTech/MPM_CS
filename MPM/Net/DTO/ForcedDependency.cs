using System;
using System.Collections.Generic;

namespace MPM.Net.DTO {
	public class ForcedDependency {
		public List<String> Interfaces { get; set; }
		public List<String> Packages { get; set; }
	}
}
