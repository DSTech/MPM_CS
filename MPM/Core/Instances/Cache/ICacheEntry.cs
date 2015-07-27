using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Cache {

	public interface ICacheEntry {

		Stream FetchStream();
	}
}
