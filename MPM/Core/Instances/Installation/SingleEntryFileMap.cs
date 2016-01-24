using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using semver.tools;

namespace MPM.Core.Instances.Installation {
    struct SingleEntryFileMapEntry {
        public String Target { get; set; }
        public IFileOperation Operation { get; set; }
        public string PackageName { get; set; }
        public SemanticVersion PackageVersion { get; set; }
    }

    struct SingleEntryFileMapEnumerabilityTracker : IReadOnlyList<SingleEntryFileMapEntry>, IReadOnlyDictionary<String, IFileOperation>, IEnumerable<SingleEntryFileMapEntry> {
        public SingleEntryFileMapEnumerabilityTracker(SingleEntryFileMapEntry entry) {
            operation = entry;
        }

        public SingleEntryFileMapEntry? operation { get; set; }

        public SingleEntryFileMapEntry this[int index] {
            get {
                if (index != 0 || !operation.HasValue) {
                    throw new IndexOutOfRangeException();
                }
                return operation.Value;
            }
        }

        public int Count => operation.HasValue ? 1 : 0;

        public IEnumerable<string> Keys => operation.HasValue ? new[] { operation.Value.Target } : Enumerable.Empty<string>();

        public IEnumerable<IFileOperation> Values => operation.HasValue ? new[] { operation.Value.Operation } : Enumerable.Empty<IFileOperation>();

        public IFileOperation this[string key] => this[0].Operation;

        public IEnumerator<SingleEntryFileMapEntry> GetEnumerator() {
            if (operation.HasValue) {
                return new[] { operation.Value }.AsEnumerable().GetEnumerator();
            } else {
                return Enumerable.Empty<SingleEntryFileMapEntry>().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public bool ContainsKey(string key) => operation.HasValue && operation.Value.Target == key;

        public bool TryGetValue(string key, out IFileOperation value) {
            if (!operation.HasValue || operation.Value.Target != key) {
                value = null;
                return false;
            }
            value = operation.Value.Operation;
            return true;
        }

        IEnumerator<KeyValuePair<string, IFileOperation>> IEnumerable<KeyValuePair<string, IFileOperation>>.GetEnumerator() {
            if (operation.HasValue) {
                return new[] { new KeyValuePair<string, IFileOperation>(operation.Value.Target, operation.Value.Operation) }.AsEnumerable().GetEnumerator();
            } else {
                return Enumerable.Empty<KeyValuePair<string, IFileOperation>>().GetEnumerator();
            }
        }
    }

    public class SingleEntryFileMap : IFileMap {
        private SingleEntryFileMapEnumerabilityTracker entries = new SingleEntryFileMapEnumerabilityTracker();

        public SingleEntryFileMap() {
        }

        public SingleEntryFileMap(String path, IFileOperation operation) {
            this.SetRegistration(path, operation);
        }

        public int Count => entries.Count;

        public IEnumerable<String> Keys => entries.Keys;

        public IEnumerable<IReadOnlyCollection<IFileOperation>> Values => entries.Count == 0 ? Enumerable.Empty<IReadOnlyCollection<IFileOperation>>() : new[] { entries.Values.ToArray() };

        public IReadOnlyCollection<IFileOperation> this[String key] => entries.Count == 0 ? new IFileOperation[0] : new[] { entries[key] };

        public bool ContainsKey(String key) => entries.ContainsKey(key);

        public IEnumerator<KeyValuePair<String, IReadOnlyCollection<IFileOperation>>> GetEnumerator() {
            using (var enumer = entries.GetEnumerator()) {
                while (enumer.MoveNext()) {
                    yield return new KeyValuePair<String, IReadOnlyCollection<IFileOperation>>(enumer.Current.Target, new[] { enumer.Current.Operation });
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool TryGetValue(String key, out IReadOnlyCollection<IFileOperation> value) {
            IFileOperation val;
            if (!entries.TryGetValue(key, out val)) {
                value = null;
                return false;
            }
            value = new[] { val };
            return true;
        }

        public void Register(string path, IFileOperation operation) {
            throw new NotSupportedException();
        }

        public bool Unregister(string path, string packageName, SemanticVersion packageVersion) {
            throw new NotSupportedException();
        }

        public bool UnregisterPackage(string packageName) {
            throw new NotSupportedException();
        }

        public bool UnregisterPackage(string path, string packageName) {
            throw new NotSupportedException();
        }

        public void SetRegistration(String path, IFileOperation operation) {
            entries.operation = new SingleEntryFileMapEntry {
                Target = path,
                PackageName = operation.PackageName,
                PackageVersion = operation.PackageVersion,
                Operation = operation,
            };
        }

        public void ClearRegistration() {
            entries.operation = null;
        }
    }
}
