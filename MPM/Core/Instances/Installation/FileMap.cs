using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using semver.tools;

namespace MPM.Core.Instances.Installation {

	internal struct FileMapEntry {
		public string PackageName { get; set; }
		public SemanticVersion PackageVersion { get; set; }
		public IFileOperation Operation { get; set; }
	}

	public class FileMap : IFileMap {

		public static IFileMap FromFileOperations(IEnumerable<IReadOnlyDictionary<String, IReadOnlyCollection<IFileOperation>>> operationSets) {
			var fileMap = new FileMap();
			foreach (var operationSet in operationSets) {
				foreach (var operationList in operationSet) {
					foreach (var operation in operationList.Value) {
						fileMap.Register(operationList.Key, operation);
					}
				}
			}
			return fileMap;
		}

		public static IFileMap FromFileOperations(IReadOnlyDictionary<String, IReadOnlyCollection<IFileOperation>> operations) {
			var fileMap = new FileMap();
			foreach (var operationList in operations) {
				foreach (var operation in operationList.Value) {
					fileMap.Register(operationList.Key, operation);
				}
			}
			return fileMap;
		}

		public static IFileMap Merge(IEnumerable<IFileMap> fileMaps) => MergeOrdered(fileMaps);

		public static IFileMap MergeOrdered(IEnumerable<IFileMap> orderedFileMaps) {
			var res = new FileMap();
			foreach (var map in orderedFileMaps) {
				foreach (var target in map) {
					foreach (var operation in target.Value) {
						res.Register(target.Key, operation);
					}
				}
			}
			return res;
		}

		private Dictionary<String, Stack<FileMapEntry>> operations = new Dictionary<String, Stack<FileMapEntry>>();

		public IReadOnlyCollection<IFileOperation> this[String key] {
			get {
				if (!operations.ContainsKey(key)) {
					return new IFileOperation[0];
				}
				return operations[key].Reverse().Select(v => v.Operation).ToArray();
			}
		}

		public int Count {
			get {
				return operations.Count;
			}
		}

		public IEnumerable<String> Keys {
			get {
				return operations.Keys;
			}
		}

		public IEnumerable<IReadOnlyCollection<IFileOperation>> Values {
			get {
				return operations.Values.Select(opSet => opSet.Reverse().Select(entry => entry.Operation).ToArray());
			}
		}

		public bool ContainsKey(String key) {
			return operations.ContainsKey(key);
		}

		public IEnumerator<KeyValuePair<String, IReadOnlyCollection<IFileOperation>>> GetEnumerator() {
			using (var enumer = operations.GetEnumerator()) {
				while (enumer.MoveNext()) {
					yield return new KeyValuePair<String, IReadOnlyCollection<IFileOperation>>(enumer.Current.Key, enumer.Current.Value.Reverse().Select(v => v.Operation).ToArray());
				}
			}
		}

		public bool TryGetValue(String key, out IReadOnlyCollection<IFileOperation> value) {
			Stack<FileMapEntry> operationStack;
			var success = operations.TryGetValue(key, out operationStack);
			if (success) {
				value = operationStack.Reverse().Select(v => v.Operation).ToArray();
			} else {
				value = new IFileOperation[0];
			}
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void Register(String path, IFileOperation operation) {
			Stack<FileMapEntry> target;
			if (operations.ContainsKey(path)) {
				target = operations[path];
			} else {
				target = new Stack<FileMapEntry>();
				operations.Add(path, target);
			}
			target.Push(new FileMapEntry {
				PackageName = operation.PackageName,
				PackageVersion = operation.PackageVersion,
				Operation = operation,
			});
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
