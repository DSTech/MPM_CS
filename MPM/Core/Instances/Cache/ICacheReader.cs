using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Cache {
    public interface ICacheReader {

        IEnumerable<ICacheEntry> Entries { get; }

        bool Contains(string cacheEntryName);

        ICacheEntry Fetch(string cacheEntryName);
    }
}
