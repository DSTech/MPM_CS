using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;

namespace MPM.Core.Instances.Info {

	public class RawBuildInfo : NamedBuild {
		public ScriptFileDeclaration[] Installation { get; set; }
	}
}
