using System;
using System.IO;
using System.Security.Cryptography;

namespace MPM.Archival {
    public class HashingForwarderStream : Stream {
        private readonly Stream _outputStream;
        private readonly bool _ownsOutputStream;
        private readonly HashAlgorithm _hashAlgorithm;

        public HashingForwarderStream(Stream outputStream, bool ownsStream = true) {
            this._outputStream = outputStream;
            this._ownsOutputStream = ownsStream;
            this._hashAlgorithm = new SHA256Managed();
            this._hashAlgorithm.Initialize();
        }

        public byte[] HashBytes => this._hashAlgorithm.Hash;

        #region Overrides of Stream

        public override void Close() {
            if (this._ownsOutputStream) {
                this._outputStream.Dispose();
            }
            this._hashAlgorithm?.Dispose();
            base.Close();
        }

        public override void Flush() {
            this._outputStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException($"{nameof(HashingForwarderStream)} does not support anything but writing to the wrapped stream");
        }

        public override void SetLength(long value) {
            throw new NotSupportedException($"{nameof(HashingForwarderStream)} does not support anything but writing to the wrapped stream");
        }

        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException($"{nameof(HashingForwarderStream)} does not support anything but writing to the wrapped stream");
        }

        public override void Write(byte[] buffer, int offset, int count) {
            this._hashAlgorithm.TransformBlock(buffer, offset, count, null, 0);
            this._outputStream.Write(buffer, offset, count);
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => this._outputStream.Length;
        public override long Position {
            get {
                return this._outputStream.Position;
            }
            set {
                throw new NotSupportedException($"{nameof(HashingForwarderStream)} does not support anything but writing to the wrapped stream");
            }
        }

        #endregion
    }
}