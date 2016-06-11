using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;
using MPM.Util;
using PowerArgs;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Alphaleonis.Win32.Filesystem;

namespace MPM.CLI {
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgShortcut("pack"), ArgShortcut("create"), ArgShortcut("build")]
        public void CreatePackage(CreatePackageArgs args) {
            var createPackageActionProvider = new CreatePackageActionProvider();
            createPackageActionProvider.Provide(Resolver, args);
        }
    }

    public class CreatePackageActionProvider : IActionProvider<CreatePackageArgs> {
        public void Provide(IContainer factory, CreatePackageArgs args) {
            if (!args.PackageDirectory.Exists) { throw new DirectoryNotFoundException(); }
            if (!args.PackageSpecFile.Exists) { throw new FileNotFoundException(); }
            Console.WriteLine($"Creating package at path:\n\t{args.PackageDirectory}");
            Environment.CurrentDirectory = args.PackageDirectory.FullName;

            var build = JsonConvert.DeserializeObject<Build>(File.ReadAllText(args.PackageSpecFile.FullName));

            var packageName = $"{build.PackageName}_{build.Version}_{build.Side}";

            Console.WriteLine($"{build.PackageName} ({build.Version}:{build.Side})=>");
            using (new ConsoleIndenter()) {
                Console.WriteLine(build);
                Console.WriteLine(JsonConvert.SerializeObject(build, Formatting.Indented));
            }

            var outputDir = args.PackageDirectory.CreateSubdirectory("output");
            foreach (var file in outputDir.GetFiles()) {
                file.Delete();
            }

            using (var archiveContent = new MemoryStream()) {
                using (var zipContent = new MemoryStream()) {
                    Console.WriteLine("Adding source files... ");
                    using (new ConsoleIndenter()) {
                        AddSourcesToZip(args, build, outputStream: zipContent);
                    }
                    zipContent.SeekToStart();

                    Console.WriteLine("Building archive...");
                    Archival.Archive.Create(zipContent, build.PackageName, archiveContent, leaveSourceOpen: true);
                }
                archiveContent.SeekToStart();
                var hashes = new List<Hash>();
                //TODO: Replace following with chunkification
                {
                    Hash fileHash;
                    var targetChunkPath = Path.Combine(outputDir.FullName, $"{packageName}.mpk");
                    Console.WriteLine("Writing chunk {0}...", targetChunkPath);
                    using (var hashfileStream = new HashingStreamForwarder(File.OpenWrite(targetChunkPath))) {
                        archiveContent.CopyTo(hashfileStream);
                        fileHash = hashfileStream.GetHash();
                    }
                    Console.WriteLine("\tHash: {0}", fileHash);
                    hashes.Add(fileHash);
                }

                {
                    var buildExternal = build.Clone();
                    buildExternal.Hashes = hashes;
                    buildExternal.Installation = null;
                    var targetExternalPackageFile = outputDir.SubFile($"{packageName}.json");
                    File.WriteAllText(
                        targetExternalPackageFile.FullName,
                        JsonConvert.SerializeObject(
                            buildExternal,
                            typeof(Build),
                            Formatting.Indented,
                            new JsonSerializerSettings()
                        )
                    );
                }
            }
        }

        private static void AddSourcesToZip(CreatePackageArgs args, Build build, Stream outputStream) {
            using (var zipStream = new ZipOutputStream(outputStream) { IsStreamOwner = false }) {
                zipStream.SetLevel(9);
                Console.WriteLine("Packing file {0} as \"package.json\"", args.PackageSpecFile.FullName);
                {
                    zipStream.PutNextEntry(new ZipEntry("package.json"));
                    {
                        var deserializedBuild = JsonConvert.DeserializeObject<Build>(File.ReadAllText(args.PackageSpecFile.FullName));
                        deserializedBuild.Hashes = null;
                        var reserializedBuild = JsonConvert.SerializeObject(deserializedBuild, typeof(Build), Formatting.Indented, new JsonSerializerSettings());
                        zipStream.Write(Encoding.UTF8.GetBytes(reserializedBuild));
                    }
                    zipStream.CloseEntry();
                }
                foreach (var decl in build.Installation) {
                    var sourced = decl as SourcedFileDeclaration;
                    if (sourced == null) {
                        continue;
                    }
                    //TODO: Consider protocol sources here, and disregard
                    var sourcePath = args.PackageDirectory.SubFile(sourced.Source);
                    if (!sourcePath.Exists) {
                        Console.WriteLine("Failed to find file {0}", sourcePath.FullName);
                        continue;
                    }
                    Console.WriteLine("Packing file {0}", sourcePath.FullName);
                    {
                        zipStream.PutNextEntry(new ZipEntry(sourced.Source));
                        File.OpenRead(sourcePath.FullName).CopyToAndClose(zipStream);
                        zipStream.CloseEntry();
                    }
                }
            }
        }
    }
}
