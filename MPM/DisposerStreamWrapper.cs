using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MPM {
    /// <summary>
    ///     Disposes any additional registered disposables when the stream is disposed.
    /// </summary>
    public class DisposerStreamWrapper : Stream, IDisposable {
        private readonly IList<IDisposable> Disposables = new List<IDisposable>();

        bool disposed = false;

        public DisposerStreamWrapper(Stream internalStream) {
            AddDisposable(this.InternalStream = internalStream);
        }

        public DisposerStreamWrapper(
            Stream internalStream,
            IEnumerable<IDisposable> additionalDisposables
            ) : this(
                internalStream
                ) {
            foreach (var disposable in additionalDisposables) {
                AddDisposable(disposable);
            }
        }

        public DisposerStreamWrapper(
            Stream internalStream,
            params IDisposable[] additionalDisposables
            ) : this(
                internalStream,
                additionalDisposables.AsEnumerable()
                ) {
        }

        private Stream InternalStream { get; }

        public override bool CanRead => InternalStream.CanRead;

        public override bool CanSeek => InternalStream.CanSeek;

        public override bool CanWrite => InternalStream.CanWrite;

        public override long Length => InternalStream.Length;

        public override long Position {
            get { return InternalStream.Position; }
            set { InternalStream.Position = value; }
        }

        /// <summary>
        ///     Adds a disposable to the list, which will be disposed in order after the internal stream is disposed.
        /// </summary>
        /// <param name="disposable">The <see cref="IDisposable" /> to register for disposal.</param>
        public void AddDisposable(IDisposable disposable) => Disposables.Add(disposable);

        public void AddDisposables(IEnumerable<IDisposable> disposables) {
            foreach (var disposable in disposables) {
                Disposables.Add(disposable);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing && !disposed) {
                disposed = true;
                InternalStream.Dispose();
                foreach (IDisposable disposable in Disposables) {
                    if (disposable != null) {
                        disposable.Dispose();
                    }
                }
            }
        }

        public override void Flush() => InternalStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => InternalStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => InternalStream.Seek(offset, origin);

        public override void SetLength(long value) => InternalStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => InternalStream.Write(buffer, offset, count);
    }
}
