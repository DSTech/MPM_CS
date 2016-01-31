using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
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
using MemoryStream = System.IO.MemoryStream;
using DirectoryNotFoundException = System.IO.DirectoryNotFoundException;
using FileNotFoundException = System.IO.FileNotFoundException;
using MPM.Extensions;
using MPM.Util;

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
                    zipContent.Seek(0, System.IO.SeekOrigin.Begin);
                    Console.WriteLine("Building archive...");
                    Archival.Archive.Create(zipContent, build.PackageName, archiveContent, leaveSourceOpen: true);
                }
                archiveContent.Seek(0, System.IO.SeekOrigin.Begin);
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
                }

                //TODO: add hashes to an outer package.json without installation script


                //foreach (var chunkAndHash in archive.Select(c => new { Hash = new Hash("sha256", c.Hash()), Contents = c.ToArray() })) {
                //    //TODO: Convert Archive Chunks to use MPM.Types.Hash
                //    var illegalChars = Path.GetInvalidFileNameChars();
                //    var targetName = chunkAndHash.Hash.ToString();
                //    foreach (var illegalChar in illegalChars) {
                //        targetName = targetName.Replace(illegalChar, '_');
                //    }
                //    var targetChunkPath = Path.Combine(outputDir.FullName, targetName);
                //    Console.WriteLine("Writing chunk {0} with hash {1}...", targetChunkPath, chunkAndHash.Hash);
                //    File.WriteAllBytes(targetChunkPath, chunkAndHash.Contents);
                //    hashes.Add(chunkAndHash.Hash);
                //}

                var buildExternal = build.Clone();
                buildExternal.Hashes = hashes;
                var targetPackageJsonExternal = Path.Combine(outputDir.FullName, $"{packageName}.json");
                File.WriteAllText(targetPackageJsonExternal, JsonConvert.SerializeObject(buildExternal, typeof(Build), Formatting.Indented, new JsonSerializerSettings()));
            }
        }

        private static void AddSourcesToZip(CreatePackageArgs args, Build build, MemoryStream outputStream) {
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
                    //Consider protocol sources here, and disregard
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
