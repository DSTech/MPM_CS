using System;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Core.Instances.Info {
    public static class ScriptFileDeclarationExtensions {
        public const string ProtocolSeparator = "://";
        public const string ArchSourcePrefix = "arch";
        public const string ArchProtocolPrefix = ArchSourcePrefix + ProtocolSeparator;

        public static IFileDeclaration Parse(this ScriptFileDeclaration declaration, String packageName, MPM.Types.SemVersion packageVersion) {
            var targets = declaration.Targets ?? new string[0];

            //Determine what type of file declaration to create depending upon class members
            var source = declaration.Source;
            if (source == null) {
                if (declaration.Type.HasValue) {
                    var sourcelessDecl = new SourcelessFileDeclaration {
                        Description = declaration.Description,
                        PackageName = packageName,
                        PackageVersion = packageVersion,
                        Targets = targets,
                        Type = declaration.Type.Value,
                    };
                }
            } else {
                if (source.StartsWith(ArchProtocolPrefix)) {
                    var sourceArchName = source.Substring(ArchProtocolPrefix.Length);
                    var archDecl = new ArchFileDeclaration() {
                        Description = declaration.Description,
                        PackageName = packageName,
                        PackageVersion = packageVersion,
                        Source = source,
                        ArchName = sourceArchName,
                    };
                    return archDecl;
                } else {
                    if (targets.Length > 0) {
                        var sourcedDecl = new SourcedFileDeclaration() {
                            Description = declaration.Description,
                            PackageName = packageName,
                            PackageVersion = packageVersion,
                            Targets = targets,
                            Hash = declaration.Hash,
                            Source = declaration.Source,
                        };
                        return sourcedDecl;
                    }
                }
            }
            //TODO: Add support for other declaration types
            {
                throw new NotSupportedException($"File declaration type not supported: {JsonConvert.SerializeObject(declaration, Formatting.Indented)}");
            }
        }
    }
}
