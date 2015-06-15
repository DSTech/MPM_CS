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
			var currentUris = currentMap.Keys.ToArray();
			var destinationUris = destination.Keys.ToArray();

			var deleted = currentUris.Except(destinationUris).ToArray();//Entries to be deleted in the operations
			var modified = currentUris.Intersect(destinationUris).ToArray();//Entries to be modified
			var created = destinationUris.Except(currentUris).ToArray();//Entries to be created

			var deletionOperations = new List<Tuple<Uri, IFileOperation[]>>(deleted.Length);
			//TODO: Fill with deletions
			throw new NotImplementedException();

			var modificationOperations = new List<Tuple<Uri, IFileOperation[]>>(modified.Length);
			//TODO: Delta & Fill
			throw new NotImplementedException();

			var creationOperations = new List<Tuple<Uri, IFileOperation[]>>(created.Length);
			//Fill with destination uri's operation contents
			foreach (var creationUri in created) {
				Debug.Assert(destination.ContainsKey(creationUri));
				var creationSteps = destination[creationUri];
				creationOperations.Add(Tuple.Create(creationUri, creationSteps.ToArray()));
			}

			return EnumerableEx.Concat(deletionOperations, modificationOperations, creationOperations);
		}
	}
}
