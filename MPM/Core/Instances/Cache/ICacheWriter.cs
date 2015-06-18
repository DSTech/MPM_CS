using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Cache {
	public interface ICacheWriter {
		void Store(string cacheEntryName, byte[] entryData);
		void Delete(string cacheEntryName);
		void Clear();
	}
}
