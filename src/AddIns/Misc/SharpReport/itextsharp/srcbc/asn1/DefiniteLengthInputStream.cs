using System;
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
    class DefiniteLengthInputStream
        : LimitedInputStream
    {
		private static readonly byte[] EmptyBytes = new byte[0];

		private int _length;

        internal DefiniteLengthInputStream(
            Stream	inStream,
            int		length)
            : base(inStream)
        {
			if (length < 0)
				throw new ArgumentException("negative lengths not allowed", "length");

			this._length = length;
        }

		public override int ReadByte()
        {
            if (_length > 0)
            {
				int b = _in.ReadByte();

				if (b < 0)
					throw new EndOfStreamException();

				--_length;
				return b;
            }

			SetParentEofDetect(true);

			return -1;
        }

		public override int Read(
			byte[]	buf,
			int		off,
			int		len)
		{
			if (_length > 0)
			{
				int toRead = System.Math.Min(len, _length);
				int numRead = _in.Read(buf, off, toRead);

				if (numRead < 1)
					throw new EndOfStreamException();

				_length -= numRead;
				return numRead;
			}

			SetParentEofDetect(true);

			return 0;
		}

		internal byte[] ToArray()
		{
			byte[] bytes;
			if (_length > 0)
			{
				bytes = new byte[_length];
				if (Streams.ReadFully(_in, bytes) < _length)
					throw new EndOfStreamException();
				_length = 0;
			}
			else
			{
				bytes = EmptyBytes;
			}

			SetParentEofDetect(true);

			return bytes;
		}
    }
}
