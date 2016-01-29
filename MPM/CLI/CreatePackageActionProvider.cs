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

namespace MPM.CLI {
    public class CreatePackageActionProvider {
        public void Provide(IContainer factory, CreatePackageArgs args) {
            if (!args.PackageDirectory.Exists) { throw new DirectoryNotFoundException(); }
            if (!args.PackageSpecFile.Exists) { throw new FileNotFoundException(); }
            Console.WriteLine($"Creating package at path:\n\t{args.PackageDirectory}");
            Environment.CurrentDirectory = args.PackageDirectory.FullName;

            var build = JsonConvert.DeserializeObject<Build>(File.ReadAllText(args.PackageSpecFile.FullName));

            Console.WriteLine(build);
            Console.WriteLine(JsonConvert.SerializeObject(build, Formatting.Indented));

            var outputDir = args.PackageDirectory.CreateSubdirectory("output");
            foreach (var file in outputDir.GetFiles()) {
                file.Delete();
            }

            //TODO: Change to MemoryStream and chunkify afterward
            using (var memStr = new MemoryStream()) {
                using (var zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(memStr) { IsStreamOwner = false }) {

                    Console.WriteLine("Packing file {0} as \"package.json\"", args.PackageSpecFile.FullName);
                    {
                        zipStream.PutNextEntry(new ZipEntry("package.json"));
                        var deserializedBuild = JsonConvert.DeserializeObject<Build>(File.ReadAllText(args.PackageSpecFile.FullName));
                        var reserializedBuild = JsonConvert.SerializeObject(deserializedBuild, typeof(Build), Formatting.Indented, new JsonSerializerSettings());
                        var reserializedBuildBytes = Encoding.UTF8.GetBytes(reserializedBuild);
                        zipStream.Write(reserializedBuildBytes, 0, reserializedBuildBytes.Length);
                        zipStream.CloseEntry();
                    }
                    foreach (var decl in build.Installation) {
                        var sourced = decl as SourcedFileDeclaration;
                        if (sourced == null) {
                            continue;
                        }
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
                memStr.Seek(0, System.IO.SeekOrigin.Begin);
                //TODO: Chunkify and add hashes to an outer package.json without installation script
                Console.WriteLine("Building archive...");
                var archive = Archival.Archive.CreateArchive(
                    build.PackageName,
                    memStr.ToArray(),
                    args.MaximumChunkLength
                    );
                var hashes = new List<Hash>();

                foreach (var chunkAndHash in archive.Select(c => new { Hash = new Hash("sha256", c.Hash()), Contents = c.ToArray() })) {
                    //TODO: Convert Archive Chunks to use MPM.Types.Hash
                    var illegalChars = Path.GetInvalidFileNameChars();
                    var targetName = chunkAndHash.Hash.ToString();
                    foreach (var illegalChar in illegalChars) {
                        targetName = targetName.Replace(illegalChar, '_');
                    }
                    var targetChunkPath = Path.Combine(outputDir.FullName, targetName);
                    Console.WriteLine("Writing chunk {0} with hash {1}...", targetChunkPath, chunkAndHash.Hash);
                    File.WriteAllBytes(targetChunkPath, chunkAndHash.Contents);
                    hashes.Add(chunkAndHash.Hash);
                }

                var buildExternal = build.Clone();
                buildExternal.Hashes = hashes;
                var targetPackageJsonExternal = Path.Combine(outputDir.FullName, $"{build.PackageName}_{build.Version}_{build.Side}.json");
                File.WriteAllText(targetPackageJsonExternal, JsonConvert.SerializeObject(buildExternal, typeof(Build), Formatting.Indented, new JsonSerializerSettings()));
            }
        }
    }
}
