using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Installation {
	struct FileMapEntry {
		public string PackageId { get; set; }
		public string OperationId { get; set; }
		public IFileOperation Operation { get; set; }
	}
	public class FileMap : IFileMap {
		private Dictionary<Uri, Stack<FileMapEntry>> operations = new Dictionary<Uri, Stack<FileMapEntry>>();
		public IEnumerable<IFileOperation> this[Uri key] {
			get {
				if (!operations.ContainsKey(key)) {
					return Enumerable.Empty<IFileOperation>();
				}
				return operations[key].Reverse().Select(v => v.Operation);
			}
		}
		public int Count {
			get {
				return operations.Count;
			}
		}
		public IEnumerable<Uri> Keys {
			get {
				return operations.Keys;
			}
		}
		public IEnumerable<IEnumerable<IFileOperation>> Values {
			get {
				return operations.Values.Select(opSet => opSet.Reverse().Select(entry => entry.Operation));
			}
		}
		public bool ContainsKey(Uri key) {
			return operations.ContainsKey(key);
		}
		public IEnumerator<KeyValuePair<Uri, IEnumerable<IFileOperation>>> GetEnumerator() {
			using (var enumer = operations.GetEnumerator()) {
				while (enumer.MoveNext()) {
					yield return new KeyValuePair<Uri, IEnumerable<IFileOperation>>(enumer.Current.Key, enumer.Current.Value.Reverse().Select(v => v.Operation));
				}
			}
		}
		public bool TryGetValue(Uri key, out IEnumerable<IFileOperation> value) {
			Stack<FileMapEntry> operationStack;
			var success = operations.TryGetValue(key, out operationStack);
			if (success) {
				value = operationStack.Reverse().Select(v => v.Operation);
			} else {
				value = new IFileOperation[0];
			}
			return true;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
		public void Register(Uri uri, string packageId, string operationId, IFileOperation operation) {
			Stack<FileMapEntry> target;
			if (operations.ContainsKey(uri)) {
				target = operations[uri];
			} else {
				target = new Stack<FileMapEntry>();
				operations.Add(uri, target);
			}
			target.Push(new FileMapEntry {
				PackageId = packageId,
				OperationId = operationId,
				Operation = operation,
			});
		}
		public bool Unregister(Uri uri, string packageId, string operationId) {
			if (!operations.ContainsKey(uri)) {
				return false;
			}
			var target = operations[uri];
			var found = target.RemoveFirst(op => op.PackageId == packageId && op.OperationId == operationId);
			if (target.Count == 0) {
				operations.Remove(uri);
			}
			return found;
		}
	}
}
