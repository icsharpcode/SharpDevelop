using System;
using System.IO;

namespace iTextSharp.text.pdf {
    public class OutputStreamCounter : Stream {
        protected Stream outc;
        protected int counter = 0;

        public OutputStreamCounter(Stream _outc) {
            outc = _outc;
        }  

        public int Counter {
            get {
                return counter;
            }
        }

        public void ResetCounter() {
            counter = 0;
        }

        public override bool CanRead {
            get {
                return false;
            }
        }
    
        public override bool CanSeek {
            get {
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                return true;
            }
        }
    
        public override long Length {
            get {
                throw new NotSupportedException();
            }
        }
    
        public override long Position {
            get {
                throw new NotSupportedException();
            }
            set {
                throw new NotSupportedException();
            }
        }
    
        public override void Flush() {
            outc.Flush();
        }
    
        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }
    
        public override void SetLength(long value) {
            throw new NotSupportedException();
        }
    
        public override void Write(byte[] buffer, int offset, int count) {
            counter += count;
            outc.Write(buffer, offset, count);
        }
    
        public override void Close() {
            outc.Close ();
        }
    }
}
