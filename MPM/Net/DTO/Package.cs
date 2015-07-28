using System;

namespace MPM.Net.DTO {

	public class Package {
		public String Name { get; set; }
		public Author[] Authors { get; set; }
		public Build[] Builds { get; set; }
	}
}
