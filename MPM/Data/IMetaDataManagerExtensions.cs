using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public static class IMetaDataManagerExtensions {
		public static void Set<T>(this IMetaDataManager manager, String key, T value) where T : class {
			manager.Set(key, value, typeof(T));
		}
		public static T Get<T>(this IMetaDataManager manager, String key) where T : class {
			return (T)manager.Get(key, typeof(T));
		}
		public static T Get<T>(this IMetaDataManager manager, String key, T @default) where T : class {
			return (T)manager.Get(key, typeof(T)) ?? @default;
		}
		public static object Get(this IMetaDataManager manager, String key, object @default, Type type) {
			return manager.Get(key, type) ?? @default;
		}
	}
}
