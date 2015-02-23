using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core {
	public interface IServerManager {
		string Arch { get; }//Eg 1.7.10, 1.8
        string Version { get; }//Eg 1230
		void Update(string version);
		IEnumerable<string> FindUpdates();//Returns the highest version last
	}
}
