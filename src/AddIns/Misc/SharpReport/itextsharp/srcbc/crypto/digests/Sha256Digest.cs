using System;

namespace Org.BouncyCastle.Crypto.Digests
{
    /**
    * Draft FIPS 180-2 implementation of SHA-256. <b>Note:</b> As this is
    * based on a draft this implementation is subject to change.
    *
    * <pre>
    *         block  word  digest
    * SHA-1   512    32    160
    * SHA-256 512    32    256
    * SHA-384 1024   64    384
    * SHA-512 1024   64    512
    * </pre>
    */
    public class Sha256Digest
		: GeneralDigest
    {
        private const int    DigestLength = 32;

        private int     H1, H2, H3, H4, H5, H6, H7, H8;

        private int[]   X = new int[64];
        private int     xOff;

        public Sha256Digest()
        {
            Reset();
        }

        /**
        * Copy constructor.  This will copy the state of the provided
        * message digest.
        */
        public Sha256Digest(Sha256Digest t) : base(t)
        {
            H1 = t.H1;
            H2 = t.H2;
            H3 = t.H3;
            H4 = t.H4;
            H5 = t.H5;
            H6 = t.H6;
            H7 = t.H7;
            H8 = t.H8;

            Array.Copy(t.X, 0, X, 0, t.X.Length);
            xOff = t.xOff;
        }

        public override string AlgorithmName
		{
			get { return "SHA-256"; }
		}

		public override int GetDigestSize()
		{
			return DigestLength;
		}

		internal override void ProcessWord(
            byte[]  input,
            int     inOff)
        {
            X[xOff++] = ((input[inOff] & 0xff) << 24) | ((input[inOff + 1] & 0xff) << 16)
                | ((input[inOff + 2] & 0xff) << 8) | ((input[inOff + 3] & 0xff));

            if (xOff == 16)
            {
                ProcessBlock();
            }
        }

        private void UnpackWord(
            int     word,
            byte[]  outBytes,
            int     outOff)
        {
            outBytes[outOff]     = (byte)((uint) word >> 24);
            outBytes[outOff + 1] = (byte)((uint) word >> 16);
            outBytes[outOff + 2] = (byte)((uint) word >> 8);
            outBytes[outOff + 3] = (byte)word;
        }

        internal override void ProcessLength(
            long    bitLength)
        {
            if (xOff > 14)
            {
                ProcessBlock();
            }

            X[14] = (int)((ulong) bitLength >> 32);
            X[15] = (int)(bitLength & 0xffffffff);
        }

        public override int DoFinal(
            byte[]  output,
            int     outOff)
        {
            Finish();

            UnpackWord(H1, output, outOff);
            UnpackWord(H2, output, outOff + 4);
            UnpackWord(H3, output, outOff + 8);
            UnpackWord(H4, output, outOff + 12);
            UnpackWord(H5, output, outOff + 16);
            UnpackWord(H6, output, outOff + 20);
            UnpackWord(H7, output, outOff + 24);
            UnpackWord(H8, output, outOff + 28);

            Reset();

            return DigestLength;
        }

        /**
        * reset the chaining variables
        */
        public override void Reset()
        {
            base.Reset();

            /* SHA-256 initial hash value
            * The first 32 bits of the fractional parts of the square roots
            * of the first eight prime numbers
            */
            unchecked
            {
                H1 = (int) 0x6a09e667;
                H2 = (int) 0xbb67ae85;
                H3 = (int) 0x3c6ef372;
                H4 = (int) 0xa54ff53a;
                H5 = (int) 0x510e527f;
                H6 = (int) 0x9b05688c;
                H7 = (int) 0x1f83d9ab;
                H8 = (int) 0x5be0cd19;
            }

            xOff = 0;
            for (int i = 0; i != X.Length; i++)
            {
                X[i] = 0;
            }
        }

        internal override void ProcessBlock()
        {
            //
            // expand 16 word block into 64 word blocks.
            //
            for (int ti = 16; ti <= 63; ti++)
            {
                X[ti] = Theta1(X[ti - 2]) + X[ti - 7] + Theta0(X[ti - 15]) + X[ti - 16];
            }

            //
            // set up working variables.
            //
            int a = H1;
            int b = H2;
            int c = H3;
            int d = H4;
            int e = H5;
            int f = H6;
            int g = H7;
            int h = H8;

			int t = 0;
			for(int i = 0; i < 8; i ++)
			{
				// t = 8 * i
				h += Sum1(e) + Ch(e, f, g) + K[t] + X[t++];
				d += h;
				h += Sum0(a) + Maj(a, b, c);

				// t = 8 * i + 1
				g += Sum1(d) + Ch(d, e, f) + K[t] + X[t++];
				c += g;
				g += Sum0(h) + Maj(h, a, b);

				// t = 8 * i + 2
				f += Sum1(c) + Ch(c, d, e) + K[t] + X[t++];
				b += f;
				f += Sum0(g) + Maj(g, h, a);

				// t = 8 * i + 3
				e += Sum1(b) + Ch(b, c, d) + K[t] + X[t++];
				a += e;
				e += Sum0(f) + Maj(f, g, h);

				// t = 8 * i + 4
				d += Sum1(a) + Ch(a, b, c) + K[t] + X[t++];
				h += d;
				d += Sum0(e) + Maj(e, f, g);

				// t = 8 * i + 5
				c += Sum1(h) + Ch(h, a, b) + K[t] + X[t++];
				g += c;
				c += Sum0(d) + Maj(d, e, f);

				// t = 8 * i + 6
				b += Sum1(g) + Ch(g, h, a) + K[t] + X[t++];
				f += b;
				b += Sum0(c) + Maj(c, d, e);

				// t = 8 * i + 7
				a += Sum1(f) + Ch(f, g, h) + K[t] + X[t++];
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
            xOff = 0;

			Array.Clear(X, 0, 16);
        }

		/* SHA-256 functions */
        private static int Ch(
            int    x,
            int    y,
            int    z)
        {
            return ((x & y) ^ ((~x) & z));
        }

        private static int Maj(
            int    x,
            int    y,
            int    z)
        {
            return ((x & y) ^ (x & z) ^ (y & z));
        }

        private static int Sum0(
            int    x)
        {
	        return (((int)((uint)x >> 2)) | (x << 30)) ^ (((int)((uint)x >> 13)) | (x << 19)) ^ (((int)((uint)x >> 22)) | (x << 10));
        }

        private static int Sum1(
            int    x)
        {
	        return (((int)((uint)x >> 6)) | (x << 26)) ^ (((int)((uint)x >> 11)) | (x << 21)) ^ (((int)((uint)x >> 25)) | (x << 7));
        }

        private static int Theta0(
            int    x)
        {
	        return (((int)((uint)x >> 7)) | (x << 25)) ^ (((int)((uint)x >> 18)) | (x << 14)) ^ ((int)((uint)x >> 3));
        }

        private static int Theta1(
            int    x)
        {
	        return (((int)((uint)x >> 17)) | (x << 15)) ^ (((int)((uint)x >> 19)) | (x << 13)) ^ ((int)((uint)x >> 10));
        }

        /* SHA-256 Constants
        * (represent the first 32 bits of the fractional parts of the
        * cube roots of the first sixty-four prime numbers)
        */
        internal static readonly int[] K = {
            unchecked ((int) 0x428a2f98), unchecked ((int) 0x71374491),
            unchecked ((int) 0xb5c0fbcf), unchecked ((int) 0xe9b5dba5),
            unchecked ((int) 0x3956c25b), unchecked ((int) 0x59f111f1),
            unchecked ((int) 0x923f82a4), unchecked ((int) 0xab1c5ed5),
            unchecked ((int) 0xd807aa98), unchecked ((int) 0x12835b01),
            unchecked ((int) 0x243185be), unchecked ((int) 0x550c7dc3),
            unchecked ((int) 0x72be5d74), unchecked ((int) 0x80deb1fe),
            unchecked ((int) 0x9bdc06a7), unchecked ((int) 0xc19bf174),
            unchecked ((int) 0xe49b69c1), unchecked ((int) 0xefbe4786),
            unchecked ((int) 0x0fc19dc6), unchecked ((int) 0x240ca1cc),
            unchecked ((int) 0x2de92c6f), unchecked ((int) 0x4a7484aa),
            unchecked ((int) 0x5cb0a9dc), unchecked ((int) 0x76f988da),
            unchecked ((int) 0x983e5152), unchecked ((int) 0xa831c66d),
            unchecked ((int) 0xb00327c8), unchecked ((int) 0xbf597fc7),
            unchecked ((int) 0xc6e00bf3), unchecked ((int) 0xd5a79147),
            unchecked ((int) 0x06ca6351), unchecked ((int) 0x14292967),
            unchecked ((int) 0x27b70a85), unchecked ((int) 0x2e1b2138),
            unchecked ((int) 0x4d2c6dfc), unchecked ((int) 0x53380d13),
            unchecked ((int) 0x650a7354), unchecked ((int) 0x766a0abb),
            unchecked ((int) 0x81c2c92e), unchecked ((int) 0x92722c85),
            unchecked ((int) 0xa2bfe8a1), unchecked ((int) 0xa81a664b),
            unchecked ((int) 0xc24b8b70), unchecked ((int) 0xc76c51a3),
            unchecked ((int) 0xd192e819), unchecked ((int) 0xd6990624),
            unchecked ((int) 0xf40e3585), unchecked ((int) 0x106aa070),
            unchecked ((int) 0x19a4c116), unchecked ((int) 0x1e376c08),
            unchecked ((int) 0x2748774c), unchecked ((int) 0x34b0bcb5),
            unchecked ((int) 0x391c0cb3), unchecked ((int) 0x4ed8aa4a),
            unchecked ((int) 0x5b9cca4f), unchecked ((int) 0x682e6ff3),
            unchecked ((int) 0x748f82ee), unchecked ((int) 0x78a5636f),
            unchecked ((int) 0x84c87814), unchecked ((int) 0x8cc70208),
            unchecked ((int) 0x90befffa), unchecked ((int) 0xa4506ceb),
            unchecked ((int) 0xbef9a3f7), unchecked ((int) 0xc67178f2)
        };
    }
}
