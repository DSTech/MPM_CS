using System;
using MPM.Core.Instances.Installation.Scripts;

namespace MPM.Core.Instances.Info {
	public class ScriptFileDeclaration {
		public String Type { get; set; }
		public String Description { get; set; }
		public String Source { get; set; }
		public String Hash { get; set; }
		public String Target { get; set; }
		public String[] Targets { get; set; }
		public IFileDeclaration Parse() {
			//Determine what type of file declaration to create depending upon class members
			throw new NotImplementedException();
		}
	}
}
