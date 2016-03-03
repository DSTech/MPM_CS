using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using LiteDB;
using MPM.Core.Dependency;
using MPM.Core.Instances.Installation;
using MPM.Data;
using MPM.Extensions;
using MPM.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Platform.VirtualFileSystem;
using Platform.VirtualFileSystem.Providers.Local;

namespace MPM.Core.Instances {
    public class Instance : ICancelable {
        private const string MpmDirectoryName = ".mpm";
        private const string ConfigurationMetaKey = "configuration";
        private const string MetaName = "meta";
        private const string PackageName = "packages";
        private const string PackageCacheName = "packagecache";

        public Instance(DirectoryInfo location) {
            this.Location = location;
            var cb = new ContainerBuilder();

            cb.Register<LiteDatabase>(ctxt => new LiteDatabase($"filename={DbPath}; journal=false"))
                .AsSelf()
                .SingleInstance()
                .Named<LiteDatabase>("InstanceDb");

            cb.Register<IFileSystem>(ctxt => FileSystemManager.Default.ResolveDirectory(Location.FullName).CreateView())
                .As<IFileSystem>()
                .SingleInstance()
                .Named<IFileSystem>("InstanceFileSystem");

            cb.Register<IMetaDataManager>(ctxt => new LiteDbMetaDataManager(ctxt.Resolve<LiteDatabase>().GetCollection<LiteDbMetaDataManager.MetaDataEntry>(MetaName)))
                .As<IMetaDataManager>()
                .SingleInstance()
                .Named<IMetaDataManager>("InstanceMetaData");

            this.Factory = cb.Build();
        }

        public IContainer Factory { get; private set; }
        public DirectoryInfo Location { get; set; }
        public DirectoryInfo MpmDirectory => Location.CreateSubdirectory(MpmDirectoryName);
        public FileInfo DbPath => MpmDirectory.SubFile($"instance.litedb");

        public String Name {
            get { return FetchDbMeta().Get<String>("name"); }
            set { FetchDbMeta().Set<String>("name", value); }
        }

        public Type LauncherType {
            get { return FetchDbMeta().Get<Type>("launcherType"); }
            set { FetchDbMeta().Set<Type>("launcherType", value); }
        }

        public InstanceConfiguration @Configuration {
            get {
                var conf = FetchDbMeta().Get<InstanceConfiguration>(ConfigurationMetaKey);
                return conf ?? InstanceConfiguration.Empty;
            }
            set { FetchDbMeta().Set<InstanceConfiguration>(ConfigurationMetaKey, value); }
        }

        public CompatibilitySide Side {
            get {
                var _s = FetchDbMeta().Get<String>("side");
                if (_s == null) {
                    return CompatibilitySide.Universal;
                }
                return JsonConvert.DeserializeObject<CompatibilitySide>(_s, new StringEnumConverter());
            }
            set {
                var valueToSet = JsonConvert.SerializeObject(value, new StringEnumConverter());
                FetchDbMeta().Set<String>("side", valueToSet);
            }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose() {
            Dispose(true);
        }

        private LiteDatabase FetchDbConnection() {
            return Factory.Resolve<LiteDatabase>();
        }

        public IMetaDataManager FetchDbMeta() {
            return Factory.Resolve<IMetaDataManager>();
        }

        public IFileSystem FetchFileSystem() {
            return Factory.Resolve<IFileSystem>();
        }

        public ILauncher CreateLauncher() {
            var ctor = LauncherType.GetConstructor(new Type[0]);
            return (ILauncher)ctor.Invoke(new object[0]);
        }

        protected void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            Factory.Dispose();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }
    }
}
