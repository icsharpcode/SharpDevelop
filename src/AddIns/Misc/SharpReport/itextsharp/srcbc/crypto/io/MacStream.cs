using System;
using System.IO;
using Org.BouncyCastle.Crypto;
namespace Org.BouncyCastle.Crypto.IO
{
    public class MacStream : Stream
    {
        internal Stream stream;
        internal IMac inMac;
        internal IMac outMac;

        public MacStream(
            Stream stream,
            IMac readMac,
            IMac writeMac)
        {
            this.stream = stream;
            this.inMac = readMac;
            this.outMac = writeMac;
        }
        public IMac ReadMac()
        {
            return inMac;
        }
        public IMac WriteMac()
        {
            return outMac;
        }
        public override int ReadByte()
        {
            int b = stream.ReadByte();
            if (inMac != null)
            {
                if (b >= 0)
                {
                    inMac.Update((byte)b);
                }
            }
            return b;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int n = stream.Read(buffer, offset, count);
            if (inMac != null)
            {
                if (n > 0)
                {
                    inMac.BlockUpdate(buffer, offset, count);
                }
            }
            return n;
        }
        public override void Write(
            byte[] buffer,
            int offset,
            int count)
        {
            if (outMac != null)
            {
                if (count > 0)
                {
                    outMac.BlockUpdate(buffer, offset, count);
                }
            }
            stream.Write(buffer, offset, count);
        }
        public override void WriteByte(byte value)
        {
            if (outMac != null)
            {
                outMac.Update(value);
            }
            stream.WriteByte(value);
        }
        public override bool CanRead
        {
            get { return stream.CanRead && (inMac != null);     }
        }
        public override bool CanWrite
        {
            get { return stream.CanWrite && (outMac != null);     }
        }
        public override bool CanSeek
        {
            get { return stream.CanSeek;     }
        }
        public override long Length
        {
            get { return stream.Length;     }
        }
        public override long Position
        {
            get { return stream.Position;   }
            set { stream.Position = value;  }
        }
        public override void Close()
        {
            stream.Close();
        }
        public override void Flush()
        {
            stream.Flush();
        }
        public override long Seek(
            long offset,
            SeekOrigin origin)
        {
            return stream.Seek(offset,origin);
        }
        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }
    }
}
