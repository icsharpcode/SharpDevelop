using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Generators
{
    /**
     * Generate suitable parameters for DSA, in line with FIPS 186-2.
     */
    public class DsaParametersGenerator
    {
        private int             size;
        private int             certainty;
        private SecureRandom    random;

        /**
         * initialise the key generator.
         *
         * @param size size of the key (range 2^512 -> 2^1024 - 64 bit increments)
         * @param certainty measure of robustness of prime (for FIPS 186-2 compliance this should be at least 80).
         * @param random random byte source.
         */
        public void Init(
            int             size,
            int             certainty,
            SecureRandom    random)
        {
			if (!IsValidDsaStrength(size))
				throw new ArgumentException("size must be from 512 - 1024 and a multiple of 64", "size");

			this.size = size;
            this.certainty = certainty;
            this.random = random;
        }

        /**
         * add value to b, returning the result in a. The a value is treated
         * as a BigInteger of length (a.Length * 8) bits. The result is
         * modulo 2^a.Length in case of overflow.
         */
        private static void Add(
            byte[]  a,
            byte[]  b,
            int     value)
        {
            int     x = (b[b.Length - 1] & 0xff) + value;

            a[b.Length - 1] = (byte)x;
            x = (int) ((uint) x >>8);

            for (int i = b.Length - 2; i >= 0; i--)
            {
                x += (b[i] & 0xff);
                a[i] = (byte)x;
                x = (int) ((uint) x >>8);
            }
        }

        /**
         * which Generates the p and g values from the given parameters,
         * returning the DsaParameters object.
         * <p>
         * Note: can take a while...</p>
         */
        public DsaParameters GenerateParameters()
        {
            byte[]          seed = new byte[20];
            byte[]          part1 = new byte[20];
            byte[]          part2 = new byte[20];
            byte[]          u = new byte[20];
            Sha1Digest      sha1 = new Sha1Digest();
            int             n = (size - 1) / 160;
            byte[]          w = new byte[size / 8];

            BigInteger      q = null, p = null, g = null;
            int             counter = 0;
            bool         primesFound = false;

            while (!primesFound)
            {
                do
                {
                    random.NextBytes(seed);

                    sha1.BlockUpdate(seed, 0, seed.Length);

                    sha1.DoFinal(part1, 0);

                    Array.Copy(seed, 0, part2, 0, seed.Length);

                    Add(part2, seed, 1);

                    sha1.BlockUpdate(part2, 0, part2.Length);

                    sha1.DoFinal(part2, 0);

                    for (int i = 0; i != u.Length; i++)
                    {
                        u[i] = (byte)(part1[i] ^ part2[i]);
                    }

                    u[0] |= (byte)0x80;
                    u[19] |= (byte)0x01;

                    q = new BigInteger(1, u);
                }
                while (!q.IsProbablePrime(certainty));

                counter = 0;

                int offset = 2;

                while (counter < 4096)
                {
                    for (int k = 0; k < n; k++)
                    {
                        Add(part1, seed, offset + k);
                        sha1.BlockUpdate(part1, 0, part1.Length);
                        sha1.DoFinal(part1, 0);
                        Array.Copy(part1, 0, w, w.Length - (k + 1) * part1.Length, part1.Length);
                    }

                    Add(part1, seed, offset + n);
                    sha1.BlockUpdate(part1, 0, part1.Length);
                    sha1.DoFinal(part1, 0);
                    Array.Copy(part1, part1.Length - ((w.Length - (n) * part1.Length)), w, 0, w.Length - n * part1.Length);

                    w[0] |= (byte)0x80;

                    BigInteger  x = new BigInteger(1, w);

                    BigInteger  c = x.Mod(q.ShiftLeft(1));

                    p = x.Subtract(c.Subtract(BigInteger.One));

                    if (p.TestBit(size - 1))
                    {
                        if (p.IsProbablePrime(certainty))
                        {
                            primesFound = true;
                            break;
                        }
                    }

                    counter += 1;
                    offset += n + 1;
                }
            }

            //
            // calculate the generator g
            //
            BigInteger  pMinusOneOverQ = p.Subtract(BigInteger.One).Divide(q);

            for (;;)
            {
                BigInteger h = new BigInteger(size, random);
                if (h.CompareTo(BigInteger.One) <= 0 || h.CompareTo(p.Subtract(BigInteger.One)) >= 0)
                {
                    continue;
                }

                g = h.ModPow(pMinusOneOverQ, p);
                if (g.CompareTo(BigInteger.One) <= 0)
                {
                    continue;
                }

                break;
            }

            return new DsaParameters(p, q, g, new DsaValidationParameters(seed, counter));
        }

		private static bool IsValidDsaStrength(
			int strength)
		{
			return strength >= 512 && strength <= 1024 && strength % 64 == 0;
		}
	}
}
