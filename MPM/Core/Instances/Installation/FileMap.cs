using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Core.Instances.Installation {
    public static class FileMap {
        public static IFileMap AsFileMap(IReadOnlyDictionary<String, IReadOnlyCollection<IFileOperation>> pseudoMap) {
            var fileMap = new DictionaryFileMap();
            foreach (var operationList in pseudoMap) {
                foreach (var operation in operationList.Value) {
                    fileMap.Register(operationList.Key, operation);
                }
            }
            return fileMap;
        }

        public static IFileMap AsMergedFileMap(IEnumerable<IReadOnlyDictionary<String, IReadOnlyCollection<IFileOperation>>> pseudoMaps) {
            var maps = pseudoMaps.Select(pseudoMap => AsFileMap(pseudoMap));
            return MergeOrdered(maps);
        }

        public static IFileMap Merge(IEnumerable<IFileMap> fileMaps) => MergeOrdered(fileMaps);

        public static IFileMap MergeOrdered(IEnumerable<IFileMap> orderedFileMaps) {
            var res = new DictionaryFileMap();
            foreach (var map in orderedFileMaps) {
                foreach (var target in map) {
                    foreach (var operation in target.Value) {
                        res.Register(target.Key, operation);
                    }
                }
            }
            return res;
        }
    }
}
