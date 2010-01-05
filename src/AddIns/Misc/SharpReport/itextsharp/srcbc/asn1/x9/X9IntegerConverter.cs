using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
    public sealed class X9IntegerConverter
    {
		private X9IntegerConverter()
		{
		}

		public static int GetByteLength(
            ECFieldElement fe)
        {
			return (fe.FieldSize + 7) / 8;
        }

		public static int GetByteLength(
			ECCurve c)
		{
			return (c.FieldSize + 7) / 8;
		}

		public static byte[] IntegerToBytes(
            BigInteger	s,
            int			qLength)
        {
			// TODO Add methods to allow writing BigInteger to existing byte array?
			byte[] bytes = s.ToByteArrayUnsigned();

			if (bytes.Length > qLength)
				throw new ArgumentException("s does not fit in specified number of bytes", "s");

			if (qLength > bytes.Length)
            {
                byte[] tmp = new byte[qLength];
                Array.Copy(bytes, 0, tmp, tmp.Length - bytes.Length, bytes.Length);
                return tmp;
            }

			return bytes;
        }
    }
}
