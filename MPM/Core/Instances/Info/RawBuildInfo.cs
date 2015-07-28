using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;

namespace MPM.Core.Instances.Info {

	/// <summary>
	/// A pseudo-DTO used due to persistence being similar to networking in use-cases.
	/// </summary>
	public class RawBuildInfo : MPM.Net.DTO.Build {
		public ScriptFileDeclaration[] Installation { get; set; }
	}
}
