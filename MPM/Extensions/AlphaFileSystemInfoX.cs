using System;
using JetBrains.Annotations;

namespace Alphaleonis.Win32.Filesystem {
    public static class AlphaFileSystemInfoX {
        private static string _getPath(FileSystemInfo fileSystemInfo) {
            var d = fileSystemInfo as DirectoryInfo;
            return d == null ? fileSystemInfo.FullName : (d.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
        }

        public static string GetRelativePathTo([NotNull] this FileSystemInfo @from, [NotNull] FileSystemInfo to) {
            if (@from == null) {
                throw new ArgumentNullException(nameof(@from));
            }
            if (to == null) {
                throw new ArgumentNullException(nameof(to));
            }

            var fromUri = new Uri(_getPath(@from));
            var toUri = new Uri(_getPath(to));

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo @from) {
            return @from.GetRelativePathTo(to);
        }

        public static string SubPath(this DirectoryInfo directory, string relativePath) {
            return Path.Combine(directory.FullName, relativePath);
        }

        public static FileInfo SubFile(this DirectoryInfo directory, string subFilePath) {
            return new FileInfo(directory.SubPath(subFilePath));
        }
    }
}
