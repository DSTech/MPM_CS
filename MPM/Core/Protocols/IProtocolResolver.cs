using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net;

namespace MPM.Core.Protocols {

	public interface IProtocolResolver {

		byte[] Resolve(string protocol, string path, byte[] hash);
		IArchResolver GetArchResolver();
	}
}
