using System.IO;

namespace Org.BouncyCastle.Asn1
{
    class IndefiniteLengthInputStream
        : LimitedInputStream
    {
        private int     _b1;
        private int     _b2;
        private bool _eofReached = false;
        private bool _eofOn00 = true;

        internal IndefiniteLengthInputStream(
            Stream inStream)
            : base(inStream)
        {
            _b1 = inStream.ReadByte();
            _b2 = inStream.ReadByte();
            _eofReached = (_b2 < 0);
        }

        internal void SetEofOn00(
            bool eofOn00)
        {
            _eofOn00 = eofOn00;
        }

        internal bool CheckForEof()
        {
            if (_eofOn00 && (_b1 == 0x00 && _b2 == 0x00))
            {
                _eofReached = true;
                SetParentEofDetect(true);
            }

			return _eofReached;
        }

		public override int Read(
			byte[]	buffer,
			int		offset,
			int		count)
		{
			// Only use this optimisation if we aren't checking for 00
			if (_eofOn00 || count < 3)
				return base.Read(buffer, offset, count);

			if (_eofReached)
				return 0;

			int numRead = _in.Read(buffer, offset + 2, count - 2);

			if (numRead <= 0)
			{
				// Corrupted stream
				throw new EndOfStreamException();
			}

			buffer[offset] = (byte)_b1;
			buffer[offset + 1] = (byte)_b2;

			_b1 = _in.ReadByte();
			_b2 = _in.ReadByte();

			if (_b2 < 0)
			{
				// Corrupted stream
				throw new EndOfStreamException();
			}

			return numRead + 2;
		}

		public override int ReadByte()
        {
            if (CheckForEof())
                return -1;

			int b = _in.ReadByte();

			if (b < 0)
            {
				// Corrupted stream
				throw new EndOfStreamException();
            }

			int v = _b1;

            _b1 = _b2;
            _b2 = b;

            return v;
        }
    }
}
