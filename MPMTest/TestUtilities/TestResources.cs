using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit.Abstractions;

namespace MPMTest.TestUtilities {
    public static class TestResources {
        public static DirectoryInfo ResourceDirectory => new DirectoryInfo("TestResources");

        public static Process LaunchPathProcess(string command, params string[] arguments) {
            var proc = Process.Start(new ProcessStartInfo {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/k {command} \"{String.Join(" ", arguments.Select(s => $"\"{s.Replace("\"", "\"\"") }\""))}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            });
            return proc;
        }

        public static Process LaunchOutputComparison(FileInfo first, FileInfo second) {
            return LaunchPathProcess("compare", first.FullName, second.FullName);
        }
    }
}
