using System;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Core.Instances.Info {
    public static class ScriptFileDeclarationExtensions {
        public const string ProtocolSeparator = "://";
        public const string ArchSourcePrefix = "arch";
        public const string ArchProtocolPrefix = ArchSourcePrefix + ProtocolSeparator;

        public static IFileDeclaration Parse(this ScriptFileDeclaration declaration, String packageName, SemVer.Version packageVersion) {
            var targets = (declaration.Target != null ? new[] { declaration.Target } : declaration.Targets ?? new String[0]);
            var hash = (declaration.Hash != null) ? Hash.Parse(declaration.Hash) : null;

            //Determine what type of file declaration to create depending upon class members
            var source = declaration.Source;
            if (source == null) {
                if (declaration.Type != null) {
                    var sourcelessDecl = new SourcelessFileDeclaration() {
                        Description = declaration.Description,
                        PackageName = packageName,
                        PackageVersion = packageVersion,
                        Targets = targets,
                    };
                    switch (declaration.Type) {
                        case "configuration":
                            sourcelessDecl.Type = SourcelessType.Configuration;
                            return sourcelessDecl;
                        case "cache":
                            sourcelessDecl.Type = SourcelessType.Cache;
                            return sourcelessDecl;
                        default:
                            throw new NotSupportedException($"File declaration type {declaration.Type} not supported");
                    }
                }
            } else {
                if (source.StartsWith(ArchProtocolPrefix)) {
                    var sourceArchName = source.Substring(ArchProtocolPrefix.Length);
                    var archDecl = new ArchFileDeclaration() {
                        Description = declaration.Description,
                        PackageName = packageName,
                        PackageVersion = packageVersion,
                        Source = sourceArchName,
                    };
                    return archDecl;
                } else {
                    if (targets.Length > 0) {
                        var sourcedDecl = new SourcedFileDeclaration() {
                            Description = declaration.Description,
                            PackageName = packageName,
                            PackageVersion = packageVersion,
                            Targets = targets,
                            Hash = hash,
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
