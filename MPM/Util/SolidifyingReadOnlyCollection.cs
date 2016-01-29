using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Util {
    public class SolidifyingReadOnlyCollection<T> : IReadOnlyCollection<T> {
        private IReadOnlyCollection<T> _solidified;
        private readonly IEnumerable<T> _source;

        public SolidifyingReadOnlyCollection(IEnumerable<T> source) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            this._source = source;
            this._solidified = null;
            //Precache sources that do not need copied with ToArray
            var readOnlySource = source as IReadOnlyCollection<T>;
            if (readOnlySource != null) {
                this._solidified = readOnlySource;
            }
            var listSource = source as List<T>;
            if (listSource != null) {
                this._solidified = listSource;
            }
        }

        public int Count => Cache().Count;

        public IEnumerator<T> GetEnumerator() {
            return Cache().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Cache().GetEnumerator();
        }

        private IReadOnlyCollection<T> Cache() {
            if (this._solidified == null) {
                this._solidified = this._source.ToArray();
            }
            return this._solidified;
        }
    }
}
