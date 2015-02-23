using System;
using System.Collections.Generic;

namespace MPM.Core {
	public class Package {
		public string Name { get; set; }
		public string[] Authors { get; set; }
		public Build[] Builds { get; set; }
	}
}
