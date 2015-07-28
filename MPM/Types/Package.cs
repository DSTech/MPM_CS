using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {

	public class Package {

		public Package(string name, IEnumerable<Author> authors, IEnumerable<Build> builds) {
			this.Name = name;
			this.Authors = authors.ToArray();
			this.Builds = builds.ToArray();
		}

		public String Name { get; }
		public IReadOnlyCollection<Author> Authors { get; }
		public IReadOnlyCollection<Build> Builds { get; }
	}
}
