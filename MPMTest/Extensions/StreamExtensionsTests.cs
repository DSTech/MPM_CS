using System;
using System.IO;
using System.Reactive.Disposables;
using MPM.Extensions;
using Xunit;

namespace MPMTest.Extensions {
	/// <summary>
	/// Tests for MPM's provided <see cref="Stream"/> extensions.
	/// </summary>
	public class StreamExtensionsTests {
		[Fact]
		public void DisposerStreamWrapperTests() {
			var dsp = new BooleanDisposable();
			var dspAdditional = new BooleanDisposable();
			using (var memstr = new MemoryStream(0).AndDispose(dsp).AndDispose(dspAdditional)) {
				Assert.False(dsp.IsDisposed);
				Assert.False(dspAdditional.IsDisposed);
				var readContents = memstr.ReadToEnd();
				Assert.Equal(0, readContents.Length);
				Assert.False(dsp.IsDisposed);
				Assert.False(dspAdditional.IsDisposed);
			}
			Assert.True(dsp.IsDisposed);
			Assert.True(dspAdditional.IsDisposed);
		}
	}
}
