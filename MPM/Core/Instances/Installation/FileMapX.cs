using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Instances.Installation {
    public static class FileMapX {
        /// <summary>
        ///     Produces a list of operations that must be undertaken to produce the difference between one FileMap and another.
        /// </summary>
        /// <param name="currentMap"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<String, IFileOperation[]>> Difference(this IFileMap currentMap, IFileMap destination) {
            //TODO: Update to fit the new IFileOperation
            var currentUris = currentMap.Keys.ToArray();
            var destinationUris = destination.Keys.ToArray();

            var deleted = currentUris.Except(destinationUris).ToArray();//Entries to be deleted in the operations
            var modified = currentUris.Intersect(destinationUris).ToArray();//Entries to be modified
            var created = destinationUris.Except(currentUris).ToArray();//Entries to be created

            var deletionOperations = new List<Tuple<String, IFileOperation[]>>(deleted.Length);
            //Create deletions for removed paths.
            foreach (var deletionPath in deleted) {
                deletionOperations.Add(new Tuple<String, IFileOperation[]>(deletionPath, new[] {
                    new DeleteFileOperation(),
                }));
            }

            var modificationOperations = new List<Tuple<String, IFileOperation[]>>(modified.Length);
            //Create deletions in front of modified paths.
            foreach (var modificationPath in modified) {
                modificationOperations.Add(new Tuple<String, IFileOperation[]>(
                    modificationPath,
                    Enumerable.Concat(
                        new[] {
                            new DeleteFileOperation(),
                        },
                        destination[modificationPath]
                        ).ToArray()
                    ));
            }

            var creationOperations = new List<Tuple<String, IFileOperation[]>>(created.Length);
            //Fill with operation contents from destination entry
            foreach (var creationPath in created) {
                Debug.Assert(destination.ContainsKey(creationPath));
                var creationSteps = destination[creationPath];
                creationOperations.Add(Tuple.Create(creationPath, creationSteps.ToArray()));
            }

            return EnumerableEx.Concat(deletionOperations, modificationOperations, creationOperations).ToArray();
        }
    }
}
