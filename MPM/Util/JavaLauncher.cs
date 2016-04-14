using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using MPM.Extensions;

namespace MPM.Util {
    public class JavaLauncher {
        public bool Prefer64Bit { get; set; } = true;
        public int XmsMb { get; set; } = 256;
        public int XmxMb { get; set; } = 2048;
        public DirectoryInfo UserHome { get; set; } = null;
        public DirectoryInfo WorkingDirectory { get; set; } = null;
        public List<FileSystemInfo> ClassPaths { get; private set; } = new List<FileSystemInfo>();
        public List<FileSystemInfo> NativesPaths { get; set; } = new List<FileSystemInfo>();
        public List<string> AdditionalJVMArguments { get; private set; } = new List<string>();
        public List<string> AdditionalArguments { get; private set; } = new List<string>();

        public string LaunchClass { get; set; } = null;
        public FileInfo LaunchJar { get; set; } = null;


        public JavaLauncher() { }

        public JavaLauncher(bool prefer64Bit = true) {
            this.Prefer64Bit = prefer64Bit;
        }

        public Process Launch(IEnumerable<string> launchArgs, bool showConsole = true) {
            var javaPath = GetBestJavaPath();

            var Xms = $"-Xms{this.XmsMb}M";

            var Xmx = $"-Xmx{this.XmxMb}M";

            var userHome = "";
            if (this.UserHome != null) {
                userHome = $"-Duser.home=\"{this.UserHome.FullName}\"";
            }

            string nativesPaths = "";
            if (NativesPaths.Count > 0) {
                nativesPaths = string.Join(PlatformSeparator, NativesPaths.Select(n => n.FullName));
                nativesPaths = $"-Djava.library.path=\"{nativesPaths}\" -Dorg.lwjgl.librarypath=\"{nativesPaths}\" -Dnet.java.games.input.librarypath=\"{nativesPaths}\"";
            }

            var classPath = "";
            if (this.ClassPaths.Count > 0) {
                classPath = $"-cp {string.Join(PlatformSeparator, this.ClassPaths.Select(p => $"\"{p.FullName}\""))}";
            }

            var heapDumpPath = "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump";

            var preferIPv4 = "-Djava.net.preferIPv4Stack=true";

            string launchMethod;
            if (LaunchJar != null) {
                launchMethod = $"-jar {LaunchJar.FullName}";
            } else if (LaunchClass != null) {
                launchMethod = LaunchClass;
            } else {
                throw new InvalidOperationException("No class is set");
            }

            var jvmArgs = $"{Xms} {Xmx} {nativesPaths} {userHome} {preferIPv4} {heapDumpPath} {classPath} {launchMethod}";
            var additionalJvmArgs = String.Join(" ", AdditionalJVMArguments);
            var additionalArgs = String.Join(" ", AdditionalArguments);
            var startInfo = new ProcessStartInfo {
                UseShellExecute = false,
                CreateNoWindow = !showConsole,
                WorkingDirectory = (this.WorkingDirectory?.FullName ?? Environment.CurrentDirectory),
                FileName = javaPath,
                Arguments = $"{jvmArgs} {additionalJvmArgs} {string.Join(" ", launchArgs)} {additionalArgs}".Trim(),
            };
            var proc = Process.Start(startInfo);
            return proc;
        }

        public static readonly string PlatformSeparator = Environment.OSVersion.Platform == PlatformID.Unix ? ":" : ";";

        private static string GetBestJavaPath() => Path.Combine(GetJavaInstallationPath(), "bin", "java.exe");

        private static string GetJavaInstallationPath() {
            var environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath)) {
                return environmentPath;
            }
            var toDispose = new List<IDisposable>();
            string javaRegistryKey;
            RegistryKey rk;
            if (Environment.Is64BitOperatingSystem) {//Prefer 64bit if it is an option
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                javaRegistryKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
                rk = baseKey.OpenSubKey(javaRegistryKey);
                if (rk != null) {
                    toDispose.Add(baseKey);
                } else {
                    baseKey.Dispose();
                }
                //if rk is null, it will default to trying 32-bit
            } else {
                rk = null;
            }
            if (rk == null) {
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                javaRegistryKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
                rk = baseKey.OpenSubKey(javaRegistryKey);
                if (rk != null) {
                    toDispose.Add(baseKey);
                } else {
                    baseKey.Dispose();
                }
            }
            if (rk == null) {
                foreach (var disp in toDispose) {
                    disp.Dispose();
                }
                throw new Exception("Java not found!");
            }
            string retval;
            using (rk) {
                var currentVersion = rk.GetValue("CurrentVersion")?.ToString();
                if (currentVersion == null) {
                    var lastKey = rk.GetSubKeyNames().LastOrDefault();
                    if (lastKey == null) {
                        foreach (var disp in toDispose) {
                            disp.Dispose();
                        }
                        throw new Exception("Java not found!");
                    }
                    using (var key = rk.OpenSubKey(lastKey)) {
                        retval = key.GetValue("JavaHome").ToString();
                    }
                } else {
                    using (var key = rk.OpenSubKey(currentVersion)) {
                        retval = key.GetValue("JavaHome").ToString();
                    }
                }
            }
            foreach (var disp in toDispose) {
                disp.Dispose();
            }
            return retval;
        }
    }
}