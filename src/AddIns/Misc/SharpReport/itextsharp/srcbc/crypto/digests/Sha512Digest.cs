using System;

namespace Org.BouncyCastle.Crypto.Digests
{
    /**
     * Draft FIPS 180-2 implementation of SHA-512. <b>Note:</b> As this is
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
    public class Sha512Digest
		: LongDigest
    {
        private const int DigestLength = 64;

		public Sha512Digest()
        {
        }

		/**
         * Copy constructor.  This will copy the state of the provided
         * message digest.
         */
        public Sha512Digest(
			Sha512Digest t)
			: base(t)
		{
		}

		public override string AlgorithmName
		{
			get { return "SHA-512"; }
		}

		public override int GetDigestSize()
		{
			return DigestLength;
		}

		public override int DoFinal(
            byte[]  output,
            int     outOff)
        {
            Finish();

            UnpackWord(H1, output, outOff);
            UnpackWord(H2, output, outOff + 8);
            UnpackWord(H3, output, outOff + 16);
            UnpackWord(H4, output, outOff + 24);
            UnpackWord(H5, output, outOff + 32);
            UnpackWord(H6, output, outOff + 40);
            UnpackWord(H7, output, outOff + 48);
            UnpackWord(H8, output, outOff + 56);

            Reset();

            return DigestLength;

        }

        /**
        * reset the chaining variables
        */
        public override void Reset()
        {
            base.Reset();

            /* SHA-512 initial hash value
             * The first 64 bits of the fractional parts of the square roots
             * of the first eight prime numbers
             */
            H1 = unchecked((long) 0x6a09e667f3bcc908L);
            H2 = unchecked((long) 0xbb67ae8584caa73bL);
            H3 = unchecked((long) 0x3c6ef372fe94f82bL);
            H4 = unchecked((long) 0xa54ff53a5f1d36f1L);
            H5 = unchecked((long) 0x510e527fade682d1L);
            H6 = unchecked((long) 0x9b05688c2b3e6c1fL);
            H7 = unchecked((long) 0x1f83d9abfb41bd6bL);
            H8 = unchecked((long) 0x5be0cd19137e2179L);
        }
    }
}
