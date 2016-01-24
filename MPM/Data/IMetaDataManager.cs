using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
    /// <summary>
    ///     Acts as a means of storing arbitrarily-typed small data keyed to strings, as long as that type is nullable and
    ///     serializable.
    /// </summary>
    public interface IMetaDataManager {
        IEnumerable<String> Keys { get; }
        bool Contains(String key);
        TValue Get<TValue>(String key) where TValue : class;
        void Set<TVALUE>(String key, TVALUE value) where TVALUE : class;
        void Delete(String key);
        void Clear();
    }
}
