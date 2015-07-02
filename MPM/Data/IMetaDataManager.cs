using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public interface IMetaDataManager : IUntypedKeyValueStore<String>, IDisposable {
	}
}
