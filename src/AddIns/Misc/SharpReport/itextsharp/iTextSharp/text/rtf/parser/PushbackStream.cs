using System;
using System.IO;

namespace iTextSharp.text.rtf.parser {
    public class PushbackStream : Stream {
        private int buf = -1;
        private readonly Stream s;

        public PushbackStream(Stream s) {
            this.s = s;
        }

        public override bool CanRead
        {
            get { return s.CanRead; }
        }
        public override bool CanSeek
        {
            get { return s.CanSeek; }
        }
        public override bool CanWrite
        {
            get { return s.CanWrite; }
        }
        public override long Length
        {
            get { return s.Length; }
        }
        public override long Position
        {
            get { return s.Position; }
            set { s.Position = value; }
        }
        public override void Close()
        {
            s.Close();
        }
        public override void Flush()
        {
            s.Flush();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return s.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            s.SetLength(value);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            s.Write(buffer, offset, count);
        }
        public override void WriteByte(byte value)
        {
            s.WriteByte(value);
        }
        public override int ReadByte() {
            if (buf != -1) {
                int tmp = buf;
                buf = -1;
                return tmp;
            }

            return s.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            if (buf != -1 && count > 0) {
                // TODO Can this case be made more efficient?
                buffer[offset] = (byte) buf;
                buf = -1;
                return 1;
            }

            return s.Read(buffer, offset, count);
        }

        public virtual void Unread(int b) {
            if (buf != -1)
                throw new InvalidOperationException("Can only push back one byte");

            buf = b & 0xFF;
        }
    }
}