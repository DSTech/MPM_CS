using System;

namespace MPM.Net.DTO {
	public class Package {
		public String Name { get; set; }
		public String[] Authors { get; set; }
		public Build[] Builds { get; set; }
	}
}
