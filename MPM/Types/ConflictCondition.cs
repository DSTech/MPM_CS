using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Types {

	public class ConflictCondition {

		public ConflictCondition(string packageName = null, string interfaceName = null, IEnumerable<ConflictCondition> and = null, IEnumerable<ConflictCondition> or = null) {
			this.PackageName = packageName;
			this.InterfaceName = interfaceName;
			this.And = (and ?? Enumerable.Empty<ConflictCondition>()).ToArray();
			this.Or = (or ?? Enumerable.Empty<ConflictCondition>()).ToArray();
		}

		public IReadOnlyCollection<ConflictCondition> And { get; }
		public IReadOnlyCollection<ConflictCondition> Or { get; }
		public String InterfaceName { get; }
		public String PackageName { get; }
	}
}
