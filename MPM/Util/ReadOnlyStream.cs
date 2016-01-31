using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Util {
    public class ReadOnlyStream : Stream {
        private readonly Stream _stream;
        private readonly bool _leaveOpen = false;
        public ReadOnlyStream(Stream stream, bool leaveOpen = false) {
            this._stream = stream;
            this._leaveOpen = leaveOpen;
        }

        #region Overrides of Stream

        public override void Flush() {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }

        public override void Close() {
            if (!_leaveOpen) {
                _stream.Dispose();
            }
            base.Close();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _stream.Length;
        public override long Position {
            get { return _stream.Position; }
            set {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}
