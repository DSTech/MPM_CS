using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using semver.tools;

namespace MPM.Core.Instances.Installation {

	public class DictionaryFileMap : IFileMap {
		private struct DictionaryFileMapEntry {
			public IFileOperation Operation { get; set; }
			public string PackageName { get; set; }
			public SemanticVersion PackageVersion { get; set; }
		}

		private Dictionary<String, Stack<DictionaryFileMapEntry>> operations = new Dictionary<String, Stack<DictionaryFileMapEntry>>();

		public int Count => operations.Count;

		public IEnumerable<String> Keys => operations.Keys;

		public IEnumerable<IReadOnlyCollection<IFileOperation>> Values => operations.Values.Select(opSet => opSet.Reverse().Select(entry => entry.Operation).ToArray());

		public IReadOnlyCollection<IFileOperation> this[String key] {
			get {
				if (!operations.ContainsKey(key)) {
					return new IFileOperation[0];
				}
				return operations[key].Reverse().Select(v => v.Operation).ToArray();
			}
		}

		public bool ContainsKey(String key) => operations.ContainsKey(key);

		public IEnumerator<KeyValuePair<String, IReadOnlyCollection<IFileOperation>>> GetEnumerator() {
			using (var enumer = operations.GetEnumerator()) {
				while (enumer.MoveNext()) {
					yield return new KeyValuePair<String, IReadOnlyCollection<IFileOperation>>(enumer.Current.Key, enumer.Current.Value.Reverse().Select(v => v.Operation).ToArray());
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public void Register(String path, IFileOperation operation) {
			Stack<DictionaryFileMapEntry> target;
			if (operations.ContainsKey(path)) {
				target = operations[path];
			} else {
				target = new Stack<DictionaryFileMapEntry>();
				operations.Add(path, target);
			}
			target.Push(new DictionaryFileMapEntry {
				PackageName = operation.PackageName,
				PackageVersion = operation.PackageVersion,
				Operation = operation,
			});
		}

		public bool TryGetValue(String key, out IReadOnlyCollection<IFileOperation> value) {
			Stack<DictionaryFileMapEntry> operationStack;
			var success = operations.TryGetValue(key, out operationStack);
			if (success) {
				value = operationStack.Reverse().Select(v => v.Operation).ToArray();
			} else {
				value = new IFileOperation[0];
			}
			return true;
		}

		public bool Unregister(String path, string packageName, SemanticVersion packageVersion) {
			if (!operations.ContainsKey(path)) {
				return false;
			}
			var target = operations[path];
			var found = target.RemoveAll(op => op.PackageName == packageName && op.PackageVersion == packageVersion);
			if (target.Count == 0) {
				operations.Remove(path);
			}
			return found > 0;
		}

		public bool UnregisterPackage(String path, string packageName) {
			if (!operations.ContainsKey(path)) {
				return false;
			}
			var target = operations[path];
			var found = target.RemoveAll(op => op.PackageName == packageName);
			if (target.Count == 0) {
				operations.Remove(path);
			}
			return found > 0;
		}

		public bool UnregisterPackage(string packageName) {
			int pathsChanged = 0;
			foreach (var operation in operations.ToArray()) {
				var target = operation.Value;
				var numFound = target.RemoveAll(op => op.PackageName == packageName);
				if (numFound > 0) {
					++pathsChanged;
				}
				if (target.Count == 0) {
					operations.Remove(operation.Key);
				}
			}
			return pathsChanged > 0;
		}
	}
}
