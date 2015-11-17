using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM;
using MPM.Extensions;
using MPM.Data;
using MPM.Core.Instances.Cache;
using Xunit;
using System.Threading;
using LiteDB;

namespace MPMTest.Core.Instances.Cache {

	public class CacheTests {
		[Fact]
		public void FileSystemCacheManagerFileManagement() {
			var cachePath = "./testCache/";
			if (!Directory.Exists(cachePath)) {
				Directory.CreateDirectory(cachePath);
			}
			var cache = new FileSystemCacheManager(cachePath);
			cache.Clear();
			var keyToStore = "test";
			var valueToStore = "testData";
			cache.Store(keyToStore, Encoding.UTF8.GetBytes(valueToStore));
			Assert.True(cache.Contains(keyToStore));
			var cachedByteValue = cache.Fetch(keyToStore).Fetch();
			var cachedValue = Encoding.UTF8.GetString(cachedByteValue);
			Assert.Equal(valueToStore, cachedValue);
			cache.Delete(keyToStore);
			Assert.False(cache.Contains(keyToStore));
			Assert.Null(cache.Fetch(keyToStore));
		}
	}
}
