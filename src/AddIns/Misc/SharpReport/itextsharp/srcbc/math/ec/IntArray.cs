using System;
using System.Text;

namespace Org.BouncyCastle.Math.EC
{
	internal class IntArray
		: ICloneable
	{
		// TODO make m fixed for the IntArray, and hence compute T once and for all

		// TODO Use uint's internally?
		private int[] m_ints;

		public IntArray(int intLen)
		{
			m_ints = new int[intLen];
		}

		private IntArray(int[] ints)
		{
			m_ints = ints;
		}

		public IntArray(BigInteger bigInt)
			: this(bigInt, 0)
		{
		}

		public IntArray(BigInteger bigInt, int minIntLen)
		{
			if (bigInt.SignValue == -1)
				throw new ArgumentException("Only positive Integers allowed", "bigint");

			if (bigInt.SignValue == 0)
			{
				m_ints = new int[] { 0 };
				return;
			}

			byte[] barr = bigInt.ToByteArrayUnsigned();
			int barrLen = barr.Length;

			int intLen = (barrLen + 3) / 4;
			m_ints = new int[System.Math.Max(intLen, minIntLen)];

			int rem = barrLen % 4;
			int barrI = 0;

			if (0 < rem)
			{
				int temp = (int) barr[barrI++];
				while (barrI < rem)
				{
					temp = temp << 8 | (int) barr[barrI++];
				}
				m_ints[--intLen] = temp;
			}

			while (intLen > 0)
			{
				int temp = (int) barr[barrI++];
				for (int i = 1; i < 4; i++)
				{
					temp = temp << 8 | (int) barr[barrI++];
				}
				m_ints[--intLen] = temp;
			}
		}

		public int GetUsedLength()
		{
			int highestIntPos = m_ints.Length;

			if (highestIntPos < 1)
				return 0;

			// Check if first element will act as sentinel
			if (m_ints[0] != 0)
			{
				while (m_ints[--highestIntPos] == 0)
				{
				}
				return highestIntPos + 1;
			}

			do
			{
				if (m_ints[--highestIntPos] != 0)
				{
					return highestIntPos + 1;
				}
			}
			while (highestIntPos > 0);

			return 0;
		}

		public int BitLength
		{
			get
			{
				// JDK 1.5: see Integer.numberOfLeadingZeros()
				int intLen = GetUsedLength();
				if (intLen == 0)
					return 0;

				int last = intLen - 1;
				uint highest = (uint) m_ints[last];
				int bits = (last << 5) + 1;

				// A couple of binary search steps
				if (highest > 0x0000ffff)
				{
					if (highest > 0x00ffffff)
					{
						bits += 24;
						highest >>= 24;
					}
					else
					{
						bits += 16;
						highest >>= 16;
					}
				}
				else if (highest > 0x000000ff)
				{
					bits += 8;
					highest >>= 8;
				}

				while (highest > 1)
				{
					++bits;
					highest >>= 1;
				}

				return bits;
			}
		}

		private int[] resizedInts(int newLen)
		{
			int[] newInts = new int[newLen];
			int oldLen = m_ints.Length;
			int copyLen = oldLen < newLen ? oldLen : newLen;
			Array.Copy(m_ints, 0, newInts, 0, copyLen);
			return newInts;
		}

		public BigInteger ToBigInteger()
		{
			int usedLen = GetUsedLength();
			if (usedLen == 0)
			{
				return BigInteger.Zero;
			}

			int highestInt = m_ints[usedLen - 1];
			byte[] temp = new byte[4];
			int barrI = 0;
			bool trailingZeroBytesDone = false;
			for (int j = 3; j >= 0; j--)
			{
				byte thisByte = (byte)((int)((uint) highestInt >> (8 * j)));
				if (trailingZeroBytesDone || (thisByte != 0))
				{
					trailingZeroBytesDone = true;
					temp[barrI++] = thisByte;
				}
			}

			int barrLen = 4 * (usedLen - 1) + barrI;
			byte[] barr = new byte[barrLen];
			for (int j = 0; j < barrI; j++)
			{
				barr[j] = temp[j];
			}
			// Highest value int is done now

			for (int iarrJ = usedLen - 2; iarrJ >= 0; iarrJ--)
			{
				for (int j = 3; j >= 0; j--)
				{
					barr[barrI++] = (byte)((int)((uint)m_ints[iarrJ] >> (8 * j)));
				}
			}
			return new BigInteger(1, barr);
		}

		public void ShiftLeft()
		{
			int usedLen = GetUsedLength();
			if (usedLen == 0)
			{
				return;
			}
			if (m_ints[usedLen - 1] < 0)
			{
				// highest bit of highest used byte is set, so shifting left will
				// make the IntArray one byte longer
				usedLen++;
				if (usedLen > m_ints.Length)
				{
					// make the m_ints one byte longer, because we need one more
					// byte which is not available in m_ints
					m_ints = resizedInts(m_ints.Length + 1);
				}
			}

			bool carry = false;
			for (int i = 0; i < usedLen; i++)
			{
				// nextCarry is true if highest bit is set
				bool nextCarry = m_ints[i] < 0;
				m_ints[i] <<= 1;
				if (carry)
				{
					// set lowest bit
					m_ints[i] |= 1;
				}
				carry = nextCarry;
			}
		}

		public IntArray ShiftLeft(int n)
		{
			int usedLen = GetUsedLength();
			if (usedLen == 0)
			{
				return this;
			}

			if (n == 0)
			{
				return this;
			}

			if (n > 31)
			{
				throw new ArgumentException("shiftLeft() for max 31 bits "
					+ ", " + n + "bit shift is not possible", "n");
			}

			int[] newInts = new int[usedLen + 1];

			int nm32 = 32 - n;
			newInts[0] = m_ints[0] << n;
			for (int i = 1; i < usedLen; i++)
			{
				newInts[i] = (m_ints[i] << n) | (int)((uint)m_ints[i - 1] >> nm32);
			}
			newInts[usedLen] = (int)((uint)m_ints[usedLen - 1] >> nm32);

			return new IntArray(newInts);
		}

		public void AddShifted(IntArray other, int shift)
		{
			int usedLenOther = other.GetUsedLength();
			int newMinUsedLen = usedLenOther + shift;
			if (newMinUsedLen > m_ints.Length)
			{
				m_ints = resizedInts(newMinUsedLen);
				//Console.WriteLine("Resize required");
			}

			for (int i = 0; i < usedLenOther; i++)
			{
				m_ints[i + shift] ^= other.m_ints[i];
			}
		}

		public int Length
		{
			get { return m_ints.Length; }
		}

		public bool TestBit(int n)
		{
			// theInt = n / 32
			int theInt = n >> 5;
			// theBit = n % 32
			int theBit = n & 0x1F;
			int tester = 1 << theBit;
			return ((m_ints[theInt] & tester) != 0);
		}

		public void FlipBit(int n)
		{
			// theInt = n / 32
			int theInt = n >> 5;
			// theBit = n % 32
			int theBit = n & 0x1F;
			int flipper = 1 << theBit;
			m_ints[theInt] ^= flipper;
		}

		public void SetBit(int n)
		{
			// theInt = n / 32
			int theInt = n >> 5;
			// theBit = n % 32
			int theBit = n & 0x1F;
			int setter = 1 << theBit;
			m_ints[theInt] |= setter;
		}

		public IntArray Multiply(IntArray other, int m)
		{
			// Lenght of c is 2m bits rounded up to the next int (32 bit)
			int t = (m + 31) >> 5;
			if (m_ints.Length < t)
			{
				m_ints = resizedInts(t);
			}

			IntArray b = new IntArray(other.resizedInts(other.Length + 1));
			IntArray c = new IntArray((m + m + 31) >> 5);
			// IntArray c = new IntArray(t + t);
			int testBit = 1;
			for (int k = 0; k < 32; k++)
			{
				for (int j = 0; j < t; j++)
				{
					if ((m_ints[j] & testBit) != 0)
					{
						// The kth bit of m_ints[j] is set
						c.AddShifted(b, j);
					}
				}
				testBit <<= 1;
				b.ShiftLeft();
			}
			return c;
		}

		// public IntArray multiplyLeftToRight(IntArray other, int m) {
		// // Lenght of c is 2m bits rounded up to the next int (32 bit)
		// int t = (m + 31) / 32;
		// if (m_ints.Length < t) {
		// m_ints = resizedInts(t);
		// }
		//
		// IntArray b = new IntArray(other.resizedInts(other.getLength() + 1));
		// IntArray c = new IntArray((m + m + 31) / 32);
		// // IntArray c = new IntArray(t + t);
		// int testBit = 1 << 31;
		// for (int k = 31; k >= 0; k--) {
		// for (int j = 0; j < t; j++) {
		// if ((m_ints[j] & testBit) != 0) {
		// // The kth bit of m_ints[j] is set
		// c.addShifted(b, j);
		// }
		// }
		// testBit >>>= 1;
		// if (k > 0) {
		// c.shiftLeft();
		// }
		// }
		// return c;
		// }

		// TODO note, redPol.Length must be 3 for TPB and 5 for PPB
		public void Reduce(int m, int[] redPol)
		{
			for (int i = m + m - 2; i >= m; i--)
			{
				if (TestBit(i))
				{
					int bit = i - m;
					FlipBit(bit);
					FlipBit(i);
					int l = redPol.Length;
					while (--l >= 0)
					{
						FlipBit(redPol[l] + bit);
					}
				}
			}
			m_ints = resizedInts((m + 31) >> 5);
		}

		public IntArray Square(int m)
		{
			// TODO make the table static readonly
			int[] table = { 0x0, 0x1, 0x4, 0x5, 0x10, 0x11, 0x14, 0x15, 0x40,
									0x41, 0x44, 0x45, 0x50, 0x51, 0x54, 0x55 };

			int t = (m + 31) >> 5;
			if (m_ints.Length < t)
			{
				m_ints = resizedInts(t);
			}

			IntArray c = new IntArray(t + t);

			// TODO twice the same code, put in separate private method
			for (int i = 0; i < t; i++)
			{
				int v0 = 0;
				for (int j = 0; j < 4; j++)
				{
					v0 = (int)((uint) v0 >> 8);
					int u = (int)((uint)m_ints[i] >> (j * 4)) & 0xF;
					int w = table[u] << 24;
					v0 |= w;
				}
				c.m_ints[i + i] = v0;

				v0 = 0;
				int upper = (int)((uint) m_ints[i] >> 16);
				for (int j = 0; j < 4; j++)
				{
					v0 = (int)((uint) v0 >> 8);
					int u = (int)((uint)upper >> (j * 4)) & 0xF;
					int w = table[u] << 24;
					v0 |= w;
				}
				c.m_ints[i + i + 1] = v0;
			}
			return c;
		}

		public override bool Equals(object o)
		{
			if (!(o is IntArray))
			{
				return false;
			}
			IntArray other = (IntArray) o;
			int usedLen = GetUsedLength();
			if (other.GetUsedLength() != usedLen)
			{
				return false;
			}
			for (int i = 0; i < usedLen; i++)
			{
				if (m_ints[i] != other.m_ints[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int i = GetUsedLength();
			int hc = i;
			while (--i >= 0)
			{
				hc *= 17;
				hc ^= m_ints[i];
			}
			return hc;
		}

		public object Clone()
		{
			return new IntArray((int[]) m_ints.Clone());
		}

		public override string ToString()
		{
			int usedLen = GetUsedLength();
			if (usedLen == 0)
			{
				return "0";
			}

			StringBuilder sb = new StringBuilder(Convert.ToString(m_ints[usedLen - 1], 2));
			for (int iarrJ = usedLen - 2; iarrJ >= 0; iarrJ--)
			{
				string hexString = Convert.ToString(m_ints[iarrJ], 2);

				// Add leading zeroes, except for highest significant int
				for (int i = hexString.Length; i < 8; i++)
				{
					hexString = "0" + hexString;
				}
				sb.Append(hexString);
			}
			return sb.ToString();
		}
	}
}
