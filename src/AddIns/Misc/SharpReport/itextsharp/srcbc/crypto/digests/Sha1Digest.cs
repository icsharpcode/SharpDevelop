using System;

namespace Org.BouncyCastle.Crypto.Digests
{

    /**
     * implementation of SHA-1 as outlined in "Handbook of Applied Cryptography", pages 346 - 349.
     *
     * It is interesting to ponder why the, apart from the extra IV, the other difference here from MD5
     * is the "endienness" of the word processing!
     */
    public class Sha1Digest
		: GeneralDigest
    {
        private const int DigestLength = 20;

        private int		H1, H2, H3, H4, H5;

        private int[]	X = new int[80];
        private int		xOff;

		public Sha1Digest()
        {
            Reset();
        }

        /**
         * Copy constructor.  This will copy the state of the provided
         * message digest.
         */
        public Sha1Digest(Sha1Digest t)
			: base(t)
        {
            H1 = t.H1;
            H2 = t.H2;
            H3 = t.H3;
            H4 = t.H4;
            H5 = t.H5;

            Array.Copy(t.X, 0, X, 0, t.X.Length);
            xOff = t.xOff;
        }

		public override string AlgorithmName
		{
			get { return "SHA-1"; }
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

        private static void UnpackWord(
            int     word,
            byte[]  outBytes,
            int     outOff)
        {
            outBytes[outOff++] = (byte)((uint)word >> 24);
            outBytes[outOff++] = (byte)((uint)word >> 16);
            outBytes[outOff++] = (byte)((uint)word >> 8);
            outBytes[outOff++] = (byte)word;
        }

        internal override void ProcessLength(long    bitLength)
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

            Reset();

            return DigestLength;
        }

        /**
         * reset the chaining variables
         */
        public override void Reset()
        {
            base.Reset();

            H1 = unchecked( (int) 0x67452301 );
            H2 = unchecked( (int) 0xefcdab89 );
            H3 = unchecked( (int) 0x98badcfe );
            H4 = unchecked( (int) 0x10325476 );
            H5 = unchecked( (int) 0xc3d2e1f0 );

            xOff = 0;
            for (int i = 0; i != X.Length; i++) X[i] = 0;
        }

        //
        // Additive constants
        //
        private const int Y1 = unchecked( (int) 0x5a827999);
        private const int Y2 = unchecked( (int) 0x6ed9eba1);
        private const int Y3 = unchecked( (int) 0x8f1bbcdc);
        private const int Y4 = unchecked( (int) 0xca62c1d6);

		private static int F(
			int    u,
			int    v,
			int    w)
		{
			return ((u & v) | ((~u) & w));
		}

		private static int H(
			int    u,
			int    v,
			int    w)
		{
			return (u ^ v ^ w);
		}

		private static int G(
			int    u,
			int    v,
			int    w)
		{
			return ((u & v) | (u & w) | (v & w));
		}

		internal override void ProcessBlock()
        {
            //
            // expand 16 word block into 80 word block.
            //
			for (int i = 16; i < 80; i++)
			{
				int t = X[i - 3] ^ X[i - 8] ^ X[i - 14] ^ X[i - 16];
				X[i] = t << 1 | (int)((uint)t >> 31);
			}

            //
            // set up working variables.
            //
            int     A = H1;
            int     B = H2;
            int     C = H3;
            int     D = H4;
            int     E = H5;

            //
            // round 1
            //
			int idx = 0;

			for (int j = 0; j < 4; j++)
			{
				// E = rotateLeft(A, 5) + F(B, C, D) + E + X[idx++] + Y1
				// B = rotateLeft(B, 30)
				E += (A << 5 | (int)((uint)A >> 27)) + F(B, C, D) + X[idx++] + Y1;
				B = B << 30 | (int)((uint)B >> 2);

				D += (E << 5 | (int)((uint)E >> 27)) + F(A, B, C) + X[idx++] + Y1;
				A = A << 30 | (int)((uint)A >> 2);

				C += (D << 5 | (int)((uint)D >> 27)) + F(E, A, B) + X[idx++] + Y1;
				E = E << 30 | (int)((uint)E >> 2);

				B += (C << 5 | (int)((uint)C >> 27)) + F(D, E, A) + X[idx++] + Y1;
				D = D << 30 | (int)((uint)D >> 2);

				A += (B << 5 | (int)((uint)B >> 27)) + F(C, D, E) + X[idx++] + Y1;
				C = C << 30 | (int)((uint)C >> 2);
			}

			//
            // round 2
            //
			for (int j = 0; j < 4; j++)
			{
				// E = rotateLeft(A, 5) + H(B, C, D) + E + X[idx++] + Y2
				// B = rotateLeft(B, 30)
				E += (A << 5 | (int)((uint)A >> 27)) + H(B, C, D) + X[idx++] + Y2;
				B = B << 30 | (int)((uint)B >> 2);

				D += (E << 5 | (int)((uint)E >> 27)) + H(A, B, C) + X[idx++] + Y2;
				A = A << 30 | (int)((uint)A >> 2);

				C += (D << 5 | (int)((uint)D >> 27)) + H(E, A, B) + X[idx++] + Y2;
				E = E << 30 | (int)((uint)E >> 2);

				B += (C << 5 | (int)((uint)C >> 27)) + H(D, E, A) + X[idx++] + Y2;
				D = D << 30 | (int)((uint)D >> 2);

				A += (B << 5 | (int)((uint)B >> 27)) + H(C, D, E) + X[idx++] + Y2;
				C = C << 30 | (int)((uint)C >> 2);
			}

			//
            // round 3
            //
			for (int j = 0; j < 4; j++)
			{
				// E = rotateLeft(A, 5) + G(B, C, D) + E + X[idx++] + Y3
				// B = rotateLeft(B, 30)
				E += (A << 5 | (int)((uint)A >> 27)) + G(B, C, D) + X[idx++] + Y3;
				B = B << 30 | (int)((uint)B >> 2);

				D += (E << 5 | (int)((uint)E >> 27)) + G(A, B, C) + X[idx++] + Y3;
				A = A << 30 | (int)((uint)A >> 2);

				C += (D << 5 | (int)((uint)D >> 27)) + G(E, A, B) + X[idx++] + Y3;
				E = E << 30 | (int)((uint)E >> 2);

				B += (C << 5 | (int)((uint)C >> 27)) + G(D, E, A) + X[idx++] + Y3;
				D = D << 30 | (int)((uint)D >> 2);

				A += (B << 5 | (int)((uint)B >> 27)) + G(C, D, E) + X[idx++] + Y3;
				C = C << 30 | (int)((uint)C >> 2);
			}

			//
            // round 4
            //
			for (int j = 0; j <= 3; j++)
			{
				// E = rotateLeft(A, 5) + H(B, C, D) + E + X[idx++] + Y4
				// B = rotateLeft(B, 30)
				E += (A << 5 | (int)((uint)A >> 27)) + H(B, C, D) + X[idx++] + Y4;
				B = B << 30 | (int)((uint)B >> 2);

				D += (E << 5 | (int)((uint)E >> 27)) + H(A, B, C) + X[idx++] + Y4;
				A = A << 30 | (int)((uint)A >> 2);

				C += (D << 5 | (int)((uint)D >> 27)) + H(E, A, B) + X[idx++] + Y4;
				E = E << 30 | (int)((uint)E >> 2);

				B += (C << 5 | (int)((uint)C >> 27)) + H(D, E, A) + X[idx++] + Y4;
				D = D << 30 | (int)((uint)D >> 2);

				A += (B << 5 | (int)((uint)B >> 27)) + H(C, D, E) + X[idx++] + Y4;
				C = C << 30 | (int)((uint)C >> 2);
			}

			H1 += A;
			H2 += B;
			H3 += C;
			H4 += D;
			H5 += E;

			//
			// reset start of the buffer.
			//
			xOff = 0;
			for (int i = 0; i < 16; i++)
			{
				X[i] = 0;
			}
		}
    }
}
