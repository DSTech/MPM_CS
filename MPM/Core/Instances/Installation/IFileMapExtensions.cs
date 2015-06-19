using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Installation {
	public static class IFileMapExtensions {
		/// <summary>
		/// Produces a list of operations that must be undertaken to produce the difference between one FileMap and another.
		/// </summary>
		/// <param name="currentMap"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<Uri, IFileOperation[]>> Difference(this IFileMap currentMap, IFileMap destination) {
			//TODO: Update to fit the new IFileOperation
			var currentUris = currentMap.Keys.ToArray();
			var destinationUris = destination.Keys.ToArray();

			var deleted = currentUris.Except(destinationUris).ToArray();//Entries to be deleted in the operations
			var modified = currentUris.Intersect(destinationUris).ToArray();//Entries to be modified
			var created = destinationUris.Except(currentUris).ToArray();//Entries to be created

			var deletionOperations = new List<Tuple<Uri, IFileOperation[]>>(deleted.Length);
			//Create deletions for removed paths.
			foreach (var deletionUri in deleted) {
				deletionOperations.Add(new Tuple<Uri, IFileOperation[]>(deletionUri, new[] {
					new DeleteFileOperation(),
				}));
			}

			var modificationOperations = new List<Tuple<Uri, IFileOperation[]>>(modified.Length);
			//Create deletions in front of modified paths.
			foreach (var modificationUri in modified) {
				modificationOperations.Add(new Tuple<Uri, IFileOperation[]>(
					modificationUri,
					Enumerable.Concat(
						new[] {
							new DeleteFileOperation(),
						},
						destination[modificationUri]
					).ToArray()
				));
			}

			var creationOperations = new List<Tuple<Uri, IFileOperation[]>>(created.Length);
			//Fill with operation contents from destination entry 
			foreach (var creationUri in created) {
				Debug.Assert(destination.ContainsKey(creationUri));
				var creationSteps = destination[creationUri];
				creationOperations.Add(Tuple.Create(creationUri, creationSteps.ToArray()));
			}

			return EnumerableEx.Concat(deletionOperations, modificationOperations, creationOperations).ToArray();
		}
	}
}
