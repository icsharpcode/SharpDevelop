using System;
using System.Text;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    public class DerBitString
		: DerStringBase
    {
        private static readonly char[] table
			= { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

		private readonly byte[]	data;
        private readonly int	padBits;

		/**
         * return the correct number of pad bits for a bit string defined in
         * a 32 bit constant
         */
        static internal int GetPadBits(
            int bitString)
        {
            int val = 0;
            for (int i = 3; i >= 0; i--)
            {
                //
                // this may look a little odd, but if it isn't done like this pre jdk1.2
                // JVM's break!
                //
                if (i != 0)
                {
                    if ((bitString >> (i * 8)) != 0)
                    {
                        val = (bitString >> (i * 8)) & 0xFF;
                        break;
                    }
                }
                else
                {
                    if (bitString != 0)
                    {
                        val = bitString & 0xFF;
                        break;
                    }
                }
            }

			if (val == 0)
            {
                return 7;
            }

			int bits = 1;

			while (((val <<= 1) & 0xFF) != 0)
            {
                bits++;
            }

			return 8 - bits;
        }

		/**
         * return the correct number of bytes for a bit string defined in
         * a 32 bit constant
         */
        static internal byte[] GetBytes(
			int bitString)
        {
            int bytes = 4;
            for (int i = 3; i >= 1; i--)
            {
                if ((bitString & (0xFF << (i * 8))) != 0)
                {
                    break;
                }
                bytes--;
            }

			byte[] result = new byte[bytes];
            for (int i = 0; i < bytes; i++)
            {
                result[i] = (byte) ((bitString >> (i * 8)) & 0xFF);
            }

			return result;
        }

        /**
         * return a Bit string from the passed in object
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerBitString GetInstance(
            object obj)
        {
            if (obj == null || obj is DerBitString)
            {
                return (DerBitString) obj;
            }

            if (obj is Asn1OctetString)
            {
                byte[]  bytes = ((Asn1OctetString) obj).GetOctets();
                int     padBits = bytes[0];
                byte[]  data = new byte[bytes.Length - 1];

				Array.Copy(bytes, 1, data, 0, bytes.Length - 1);

				return new DerBitString(data, padBits);
            }

			if (obj is Asn1TaggedObject)
            {
                return GetInstance(((Asn1TaggedObject) obj).GetObject());
            }

			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return a Bit string from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerBitString GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(obj.GetObject());
        }

		internal DerBitString(
            byte	data,
            int		padBits)
        {
			this.data = new byte[]{ data };
            this.padBits = padBits;
        }

		/**
         * @param data the octets making up the bit string.
         * @param padBits the number of extra bits at the end of the string.
         */
        public DerBitString(
            byte[]	data,
            int		padBits)
        {
			// TODO Deep copy?
            this.data = data;
            this.padBits = padBits;
        }

		public DerBitString(
            byte[] data)
        {
			// TODO Deep copy?
			this.data = data;
        }

		public DerBitString(
            Asn1Encodable obj)
        {
            this.data = obj.GetDerEncoded();
//            this.padBits = 0;
        }

		public byte[] GetBytes()
        {
            return data;
        }

		public int PadBits
        {
			get { return padBits; }
        }

		/**
		 * @return the value of the bit string as an int (truncating if necessary)
		 */
		public int IntValue
		{
			get
			{
				int value = 0;

				for (int i = 0; i != data.Length && i != 4; i++)
				{
					value |= (data[i] & 0xff) << (8 * i);
				}

				return value;
			}
		}

		internal override void Encode(
            DerOutputStream derOut)
        {
            byte[] bytes = new byte[GetBytes().Length + 1];

			bytes[0] = (byte) PadBits;
            Array.Copy(GetBytes(), 0, bytes, 1, bytes.Length - 1);

			derOut.WriteEncoded(Asn1Tags.BitString, bytes);
        }

		protected override int Asn1GetHashCode()
		{
			return padBits.GetHashCode() ^ Arrays.GetHashCode(data);
        }

        protected override bool Asn1Equals(
            Asn1Object asn1Object)
        {
			DerBitString other = asn1Object as DerBitString;

			if (other == null)
				return false;

			return this.padBits == other.padBits
				&& Arrays.AreEqual(this.data, other.data);
        }

		public override string GetString()
        {
            StringBuilder buffer = new StringBuilder("#");

			byte[] str = GetDerEncoded();

			for (int i = 0; i != str.Length; i++)
            {
                uint ubyte = str[i];
                buffer.Append(table[(ubyte >> 4) & 0xf]);
                buffer.Append(table[str[i] & 0xf]);
            }

			return buffer.ToString();
        }
	}
}
