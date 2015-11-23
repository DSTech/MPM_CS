using System;
using System.Collections.Generic;

namespace MPM.Net.DTO {

	public class Package {
		public String Name { get; set; }
		public List<Author> Authors { get; set; }
		public List<Build> Builds { get; set; }
	}
}
