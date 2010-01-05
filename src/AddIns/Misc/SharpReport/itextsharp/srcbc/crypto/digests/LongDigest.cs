using System;
using Org.BouncyCastle.Crypto;

namespace Org.BouncyCastle.Crypto.Digests
{
    /**
    * Base class for SHA-384 and SHA-512.
    */
    public abstract class LongDigest
		: IDigest
    {
        private int     MyByteLength = 128;

        private byte[]  xBuf;
        private int     xBufOff;

        private long	byteCount1;
        private long	byteCount2;

        internal long	H1, H2, H3, H4, H5, H6, H7, H8;

        private long[]	W = new long[80];
        private int		wOff;

		/**
        * Constructor for variable length word
        */
        internal LongDigest()
        {
            xBuf = new byte[8];

			Reset();
        }

		/**
        * Copy constructor.  We are using copy constructors in place
        * of the object.Clone() interface as this interface is not
        * supported by J2ME.
        */
        internal LongDigest(
			LongDigest t)
        {
            xBuf = new byte[t.xBuf.Length];
            Array.Copy(t.xBuf, 0, xBuf, 0, t.xBuf.Length);

            xBufOff = t.xBufOff;
            byteCount1 = t.byteCount1;
            byteCount2 = t.byteCount2;

            H1 = t.H1;
            H2 = t.H2;
            H3 = t.H3;
            H4 = t.H4;
            H5 = t.H5;
            H6 = t.H6;
            H7 = t.H7;
            H8 = t.H8;

            Array.Copy(t.W, 0, W, 0, t.W.Length);
            wOff = t.wOff;
        }

        public void Update(
            byte input)
        {
            xBuf[xBufOff++] = input;

            if (xBufOff == xBuf.Length)
            {
                ProcessWord(xBuf, 0);
                xBufOff = 0;
            }

            byteCount1++;
        }

        public void BlockUpdate(
            byte[]  input,
            int     inOff,
            int     length)
        {
            //
            // fill the current word
            //
            while ((xBufOff != 0) && (length > 0))
            {
                Update(input[inOff]);

                inOff++;
                length--;
            }

            //
            // process whole words.
            //
            while (length > xBuf.Length)
            {
                ProcessWord(input, inOff);

                inOff += xBuf.Length;
                length -= xBuf.Length;
                byteCount1 += xBuf.Length;
            }

            //
            // load in the remainder.
            //
            while (length > 0)
            {
                Update(input[inOff]);

                inOff++;
                length--;
            }
        }

        public void Finish()
        {
            AdjustByteCounts();

            long    lowBitLength = byteCount1 << 3;
            long    hiBitLength = byteCount2;

            //
            // add the pad bytes.
            //
            Update((byte)128);

            while (xBufOff != 0)
            {
                Update((byte)0);
            }

            ProcessLength(lowBitLength, hiBitLength);

            ProcessBlock();
        }

        public virtual void Reset()
        {
            byteCount1 = 0;
            byteCount2 = 0;

            xBufOff = 0;
            for ( int i = 0; i < xBuf.Length; i++ )
            {
                xBuf[i] = 0;
            }

            wOff = 0;
            for (int i = 0; i != W.Length; i++)
            {
                W[i] = 0;
            }
        }

        internal void ProcessWord(
            byte[]  input,
            int     inOff)
        {
            W[wOff++] = ((long)(input[inOff] & 0xff) << 56)
                      | ((long)(input[inOff + 1] & 0xff) << 48)
                      | ((long)(input[inOff + 2] & 0xff) << 40)
                      | ((long)(input[inOff + 3] & 0xff) << 32)
                      | ((long)(input[inOff + 4] & 0xff) << 24)
                      | ((long)(input[inOff + 5] & 0xff) << 16)
                      | ((long)(input[inOff + 6] & 0xff) << 8)
                      | ((uint)(input[inOff + 7] & 0xff) );

            if (wOff == 16)
            {
                ProcessBlock();
            }
        }

        internal static void UnpackWord(
            long    word,
            byte[]  outBytes,
            int     outOff)
        {
            outBytes[outOff]     = (byte)((ulong) word >> 56);
            outBytes[outOff + 1] = (byte)((ulong) word >> 48);
            outBytes[outOff + 2] = (byte)((ulong) word >> 40);
            outBytes[outOff + 3] = (byte)((ulong) word >> 32);
            outBytes[outOff + 4] = (byte)((ulong) word >> 24);
            outBytes[outOff + 5] = (byte)((ulong) word >> 16);
            outBytes[outOff + 6] = (byte)((ulong) word >> 8);
            outBytes[outOff + 7] = (byte)word;
        }

        /**
        * adjust the byte counts so that byteCount2 represents the
        * upper long (less 3 bits) word of the byte count.
        */
        private void AdjustByteCounts()
        {
            if (byteCount1 > 0x1fffffffffffffffL)
            {
                byteCount2 += (long) ((ulong) byteCount1 >> 61);
                byteCount1 &= 0x1fffffffffffffffL;
            }
        }

        internal void ProcessLength(
            long	lowW,
            long	hiW)
        {
            if (wOff > 14)
            {
                ProcessBlock();
            }

            W[14] = hiW;
            W[15] = lowW;
        }

        internal void ProcessBlock()
        {
            AdjustByteCounts();

            //
            // expand 16 word block into 80 word blocks.
            //
            for (int ti = 16; ti <= 79; ++ti)
            {
                W[ti] = Sigma1(W[ti - 2]) + W[ti - 7] + Sigma0(W[ti - 15]) + W[ti - 16];
            }

            //
            // set up working variables.
            //
            long a = H1;
            long b = H2;
            long c = H3;
            long d = H4;
            long e = H5;
            long f = H6;
            long g = H7;
            long h = H8;

			int t = 0;
			for(int i = 0; i < 10; i ++)
			{
				// t = 8 * i
				h += Sum1(e) + Ch(e, f, g) + K[t] + W[t++];
				d += h;
				h += Sum0(a) + Maj(a, b, c);

				// t = 8 * i + 1
				g += Sum1(d) + Ch(d, e, f) + K[t] + W[t++];
				c += g;
				g += Sum0(h) + Maj(h, a, b);

				// t = 8 * i + 2
				f += Sum1(c) + Ch(c, d, e) + K[t] + W[t++];
				b += f;
				f += Sum0(g) + Maj(g, h, a);

				// t = 8 * i + 3
				e += Sum1(b) + Ch(b, c, d) + K[t] + W[t++];
				a += e;
				e += Sum0(f) + Maj(f, g, h);

				// t = 8 * i + 4
				d += Sum1(a) + Ch(a, b, c) + K[t] + W[t++];
				h += d;
				d += Sum0(e) + Maj(e, f, g);

				// t = 8 * i + 5
				c += Sum1(h) + Ch(h, a, b) + K[t] + W[t++];
				g += c;
				c += Sum0(d) + Maj(d, e, f);

				// t = 8 * i + 6
				b += Sum1(g) + Ch(g, h, a) + K[t] + W[t++];
				f += b;
				b += Sum0(c) + Maj(c, d, e);

				// t = 8 * i + 7
				a += Sum1(f) + Ch(f, g, h) + K[t] + W[t++];
				e += a;
				a += Sum0(b) + Maj(b, c, d);
			}

			H1 += a;
            H2 += b;
            H3 += c;
            H4 += d;
            H5 += e;
            H6 += f;
            H7 += g;
            H8 += h;

			//
            // reset the offset and clean out the word buffer.
            //
            wOff = 0;

			Array.Clear(W, 0, 16);
		}

		/* SHA-384 and SHA-512 functions (as for SHA-256 but for longs) */
        private static long Ch(
            long    x,
            long    y,
            long    z)
        {
            return ((x & y) ^ ((~x) & z));
        }

        private static long Maj(
            long    x,
            long    y,
            long    z)
        {
            return ((x & y) ^ (x & z) ^ (y & z));
        }

        private static long Sum0(
            long x)
        {
	        return ((x << 36)|((long)((ulong)x >> 28))) ^ ((x << 30)|((long)((ulong)x >> 34))) ^ ((x << 25)|((long)((ulong)x >> 39)));
        }

		private static long Sum1(
            long x)
        {
	        return ((x << 50)|((long)((ulong)x >> 14))) ^ ((x << 46)|((long)((ulong)x >> 18))) ^ ((x << 23)|((long)((ulong)x >> 41)));
        }

        private static long Sigma0(
            long x)
        {
	        return ((x << 63)|((long)((ulong)x >> 1))) ^ ((x << 56)|((long)((ulong)x >> 8))) ^ ((long)((ulong)x >> 7));
        }

        private static long Sigma1(
            long x)
        {
	        return ((x << 45)|((long)((ulong)x >> 19))) ^ ((x << 3)|((long)((ulong)x >> 61))) ^ ((long)((ulong)x >> 6));
        }

        /* SHA-384 and SHA-512 Constants
         * (represent the first 64 bits of the fractional parts of the
         * cube roots of the first sixty-four prime numbers)
         */
        internal static readonly long[] K =
        {
			unchecked((long) 0x428a2f98d728ae22L), unchecked((long) 0x7137449123ef65cdL), unchecked((long) 0xb5c0fbcfec4d3b2fL), unchecked((long) 0xe9b5dba58189dbbcL),
			unchecked((long) 0x3956c25bf348b538L), unchecked((long) 0x59f111f1b605d019L), unchecked((long) 0x923f82a4af194f9bL), unchecked((long) 0xab1c5ed5da6d8118L),
			unchecked((long) 0xd807aa98a3030242L), unchecked((long) 0x12835b0145706fbeL), unchecked((long) 0x243185be4ee4b28cL), unchecked((long) 0x550c7dc3d5ffb4e2L),
			unchecked((long) 0x72be5d74f27b896fL), unchecked((long) 0x80deb1fe3b1696b1L), unchecked((long) 0x9bdc06a725c71235L), unchecked((long) 0xc19bf174cf692694L),
			unchecked((long) 0xe49b69c19ef14ad2L), unchecked((long) 0xefbe4786384f25e3L), unchecked((long) 0x0fc19dc68b8cd5b5L), unchecked((long) 0x240ca1cc77ac9c65L),
			unchecked((long) 0x2de92c6f592b0275L), unchecked((long) 0x4a7484aa6ea6e483L), unchecked((long) 0x5cb0a9dcbd41fbd4L), unchecked((long) 0x76f988da831153b5L),
			unchecked((long) 0x983e5152ee66dfabL), unchecked((long) 0xa831c66d2db43210L), unchecked((long) 0xb00327c898fb213fL), unchecked((long) 0xbf597fc7beef0ee4L),
			unchecked((long) 0xc6e00bf33da88fc2L), unchecked((long) 0xd5a79147930aa725L), unchecked((long) 0x06ca6351e003826fL), unchecked((long) 0x142929670a0e6e70L),
			unchecked((long) 0x27b70a8546d22ffcL), unchecked((long) 0x2e1b21385c26c926L), unchecked((long) 0x4d2c6dfc5ac42aedL), unchecked((long) 0x53380d139d95b3dfL),
			unchecked((long) 0x650a73548baf63deL), unchecked((long) 0x766a0abb3c77b2a8L), unchecked((long) 0x81c2c92e47edaee6L), unchecked((long) 0x92722c851482353bL),
			unchecked((long) 0xa2bfe8a14cf10364L), unchecked((long) 0xa81a664bbc423001L), unchecked((long) 0xc24b8b70d0f89791L), unchecked((long) 0xc76c51a30654be30L),
			unchecked((long) 0xd192e819d6ef5218L), unchecked((long) 0xd69906245565a910L), unchecked((long) 0xf40e35855771202aL), unchecked((long) 0x106aa07032bbd1b8L),
			unchecked((long) 0x19a4c116b8d2d0c8L), unchecked((long) 0x1e376c085141ab53L), unchecked((long) 0x2748774cdf8eeb99L), unchecked((long) 0x34b0bcb5e19b48a8L),
			unchecked((long) 0x391c0cb3c5c95a63L), unchecked((long) 0x4ed8aa4ae3418acbL), unchecked((long) 0x5b9cca4f7763e373L), unchecked((long) 0x682e6ff3d6b2b8a3L),
			unchecked((long) 0x748f82ee5defb2fcL), unchecked((long) 0x78a5636f43172f60L), unchecked((long) 0x84c87814a1f0ab72L), unchecked((long) 0x8cc702081a6439ecL),
			unchecked((long) 0x90befffa23631e28L), unchecked((long) 0xa4506cebde82bde9L), unchecked((long) 0xbef9a3f7b2c67915L), unchecked((long) 0xc67178f2e372532bL),
			unchecked((long) 0xca273eceea26619cL), unchecked((long) 0xd186b8c721c0c207L), unchecked((long) 0xeada7dd6cde0eb1eL), unchecked((long) 0xf57d4f7fee6ed178L),
			unchecked((long) 0x06f067aa72176fbaL), unchecked((long) 0x0a637dc5a2c898a6L), unchecked((long) 0x113f9804bef90daeL), unchecked((long) 0x1b710b35131c471bL),
			unchecked((long) 0x28db77f523047d84L), unchecked((long) 0x32caab7b40c72493L), unchecked((long) 0x3c9ebe0a15c9bebcL), unchecked((long) 0x431d67c49c100d4cL),
			unchecked((long) 0x4cc5d4becb3e42b6L), unchecked((long) 0x597f299cfc657e2aL), unchecked((long) 0x5fcb6fab3ad6faecL), unchecked((long) 0x6c44198c4a475817L)
        };

		public int GetByteLength()
		{
			return MyByteLength;
		}

		public abstract string AlgorithmName { get; }
		public abstract int GetDigestSize();
        public abstract int DoFinal(byte[] output, int outOff);
    }
}
