using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Protocols {
	public interface IProtocolResolver {
		byte[] Resolve(string protocol, string path, byte[] hash);
	}
}
