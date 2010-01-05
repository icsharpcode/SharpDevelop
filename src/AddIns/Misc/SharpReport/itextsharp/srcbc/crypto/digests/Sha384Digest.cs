using System;

namespace Org.BouncyCastle.Crypto.Digests
{
    /**
     * Draft FIPS 180-2 implementation of SHA-384. <b>Note:</b> As this is
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
    public class Sha384Digest
		: LongDigest
    {
        private const int DigestLength = 48;

		public Sha384Digest()
        {
        }

        /**
         * Copy constructor.  This will copy the state of the provided
         * message digest.
         */
        public Sha384Digest(
			Sha384Digest t)
			: base(t)
		{
		}

		public override string AlgorithmName
		{
			get { return "SHA-384"; }
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

            Reset();

            return DigestLength;
        }

        /**
        * reset the chaining variables
        */
        public override void Reset()
        {
            base.Reset();

            /* SHA-384 initial hash value
                * The first 64 bits of the fractional parts of the square roots
                * of the 9th through 16th prime numbers
                */
            H1 = unchecked((long) 0xcbbb9d5dc1059ed8L);
            H2 = unchecked((long) 0x629a292a367cd507L);
            H3 = unchecked((long) 0x9159015a3070dd17L);
            H4 = unchecked((long) 0x152fecd8f70e5939L);
            H5 = unchecked((long) 0x67332667ffc00b31L);
            H6 = unchecked((long) 0x8eb44a8768581511L);
            H7 = unchecked((long) 0xdb0c2e0d64f98fa7L);
            H8 = unchecked((long) 0x47b5481dbefa4fa4L);
        }
    }
}
