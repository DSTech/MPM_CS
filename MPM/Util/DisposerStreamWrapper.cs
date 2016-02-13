using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MPM.Util {
    /// <summary>
    ///     Disposes any additional registered disposables when the stream is disposed.
    /// </summary>
    public class DisposerStreamWrapper : Stream, IDisposable {
        private readonly IList<IDisposable> Disposables = new List<IDisposable>();

        bool disposed = false;

        public DisposerStreamWrapper(Stream internalStream) {
            this.AddDisposable(this.InternalStream = internalStream);
        }

        public DisposerStreamWrapper(
            Stream internalStream,
            IEnumerable<IDisposable> additionalDisposables
            ) : this(
                internalStream
                ) {
            foreach (var disposable in additionalDisposables) {
                this.AddDisposable(disposable);
            }
        }

        public DisposerStreamWrapper(
            Stream internalStream,
            params IDisposable[] additionalDisposables
            ) : this(
                internalStream,
                (IEnumerable<IDisposable>) additionalDisposables.AsEnumerable()
                ) {
        }

        private Stream InternalStream { get; }

        public override bool CanRead => this.InternalStream.CanRead;

        public override bool CanSeek => this.InternalStream.CanSeek;

        public override bool CanWrite => this.InternalStream.CanWrite;

        public override long Length => this.InternalStream.Length;

        public override long Position {
            get { return this.InternalStream.Position; }
            set { this.InternalStream.Position = value; }
        }

        /// <summary>
        ///     Adds a disposable to the list, which will be disposed in order after the internal stream is disposed.
        /// </summary>
        /// <param name="disposable">The <see cref="IDisposable" /> to register for disposal.</param>
        public void AddDisposable(IDisposable disposable) => this.Disposables.Add(disposable);

        public void AddDisposables(IEnumerable<IDisposable> disposables) {
            foreach (var disposable in disposables) {
                this.Disposables.Add(disposable);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing && !this.disposed) {
                this.disposed = true;
                this.InternalStream.Dispose();
                foreach (IDisposable disposable in this.Disposables) {
                    if (disposable != null) {
                        disposable.Dispose();
                    }
                }
            }
        }

        public override void Flush() => this.InternalStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => this.InternalStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => this.InternalStream.Seek(offset, origin);

        public override void SetLength(long value) => this.InternalStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => this.InternalStream.Write(buffer, offset, count);
    }
}
