using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Instances.Installation.Scripts;

namespace MPM.Core.Instances.Info {
	public class ParsedBuildInfo : NamedBuild {
		public IReadOnlyList<IFileDeclaration> Installation { get; set; }
	}
}
