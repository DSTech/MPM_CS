using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public interface IMetaDataManager : IDisposable {
		IEnumerable<string> Keys { get; }
		void Set(String key, object value, Type type);
		object Get(String key, Type type);
		void Clear(String key);
	}
}
