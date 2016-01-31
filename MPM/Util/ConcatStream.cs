using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Util {
    public class ConcatStream : Stream {
        private readonly IEnumerator<Stream> _streams;
        private readonly bool _leaveOpen = false;
        private long _position = 0;
        private bool _didMoveNext = false;
        public ConcatStream(IEnumerable<Stream> streams, bool leaveOpen = false) {
            this._streams = streams.GetEnumerator();
            _didMoveNext = _streams.MoveNext();
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

        public override int Read(byte[] buffer, int offset, int count) {
            if (!_didMoveNext) {
                return 0;
            }
            var ret = _streams.Current.Read(buffer, offset, count);
            while (ret == 0) {
                if (!_leaveOpen) {
                    _streams.Current?.Dispose();
                }
                if ((_didMoveNext = _streams.MoveNext()) == false) {
                    return 0;
                }
                ret = _streams.Current.Read(buffer, offset, count);
            }
            _position += ret;
            return ret;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }

        public override void Close() {
            if (!_leaveOpen) {
                if(_didMoveNext) {
                    _streams.Current?.Dispose();
                }
                while((_didMoveNext = _streams.MoveNext()) == true) {
                    _streams.Current?.Dispose();
                }
            }
            _streams.Dispose();
            base.Close();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length {
            get {
                throw new NotSupportedException();
            }
        }
        public override long Position {
            get { return _position; }
            set {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}
