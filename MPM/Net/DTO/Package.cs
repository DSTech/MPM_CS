using System;
using System.Collections.Generic;

namespace MPM.Net.DTO {
	public class Package {
		public string Name { get; set; }
		public string[] Authors { get; set; }
		public Build[] Builds { get; set; }
	}
}
