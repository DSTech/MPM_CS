using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using MPM.ActionProviders;
using MPM.Core;
using MPM.Core.Dependency;
using MPM.Core.Instances;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Installation;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Types;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using MPM.Extensions;
using MPM.Util;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace MPM.CLI {
    public class CreatePackageActionProvider {
        public void Provide(IContainer factory, CreatePackageArgs args) {
            if (!args.PackageDirectory.Exists) { throw new DirectoryNotFoundException(); }
            if (!args.PackageSpecFile.Exists) { throw new FileNotFoundException(); }
            Console.WriteLine($"Creating package at path:\n\t{args.PackageDirectory}");
            Environment.CurrentDirectory = args.PackageDirectory.FullName;

            var build = JsonConvert.DeserializeObject<Build>(File.ReadAllText(args.PackageSpecFile.FullName));

            var packageName = $"{build.PackageName}_{build.Version}_{build.Side}";

            Console.WriteLine($"{build.PackageName} ({build.Version}:{build.Side})=>");
            Console.WriteLine(build);
            Console.WriteLine(JsonConvert.SerializeObject(build, Formatting.Indented));

            var outputDir = args.PackageDirectory.CreateSubdirectory("output");
            foreach (var file in outputDir.GetFiles()) {
                file.Delete();
            }

            using (var archiveContent = new MemoryStream()) {
                using (var zipContent = new MemoryStream()) {
                    Console.WriteLine("Adding source files...");
                    AddSourcesToZip(args, build, outputStream: zipContent);
                    zipContent.SeekToStart();
                    Console.WriteLine("Building archive...");
                    Archival.Archive.Create(zipContent, build.PackageName, archiveContent, leaveSourceOpen: true);
                }
                archiveContent.SeekToStart();
                var hashes = new List<Hash>();
                //TODO: Replace following with chunkification
                {
                    Hash fileHash;
                    var targetChunkPath = Path.Combine(outputDir.FullName, $"{packageName}.mpmp");
                    Console.WriteLine("Writing chunk {0}...", targetChunkPath);
                    using (var hashfileStream = new HashingStreamForwarder(File.OpenWrite(targetChunkPath))) {
                        archiveContent.CopyTo(hashfileStream);
                        fileHash = hashfileStream.GetHash();
                    }
                    Console.WriteLine("\tHash: {0}", fileHash.ToString());
                    hashes.Add(fileHash);
                }

                {
                    var buildExternal = build.Clone();
                    buildExternal.Hashes = hashes;
                    buildExternal.Installation = null;
                    var targetExternalPackagePath = Path.Combine(outputDir.FullName, $"{packageName}.json");
                    File.WriteAllText(targetExternalPackagePath, JsonConvert.SerializeObject(buildExternal, typeof(Build), Formatting.Indented, new JsonSerializerSettings()));
                }
            }
        }

        private static void AddSourcesToZip(CreatePackageArgs args, Build build, Stream outputStream) {
            using (var zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(outputStream) { IsStreamOwner = false }) {
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
                    var sourcePath = Path.Combine(args.PackageDirectory.FullName, sourced.Source);
                    if (!File.Exists(sourcePath)) {
                        Console.WriteLine("Failed to find file {0}", sourcePath);
                        continue;
                    }
                    Console.WriteLine("Packing file {0}", sourcePath);
                    {
                        zipStream.PutNextEntry(new ZipEntry(sourced.Source));
                        {
                            File.OpenRead(sourced.Source).CopyToAndClose(zipStream);
                        }
                        zipStream.CloseEntry();
                    }
                }
            }
        }
    }
}
