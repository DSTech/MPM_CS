using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances {
	public interface ILauncher {
		void Launch(Instance instance, IProfile profile);
    }
}
