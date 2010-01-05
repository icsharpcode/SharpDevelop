using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Generators
{
	internal class DHParametersHelper
	{
		// The primes b/w 2 and ~2^10
		/*
				3   5   7   11  13  17  19  23  29
			31  37  41  43  47  53  59  61  67  71
			73  79  83  89  97  101 103 107 109 113
			127 131 137 139 149 151 157 163 167 173
			179 181 191 193 197 199 211 223 227 229
			233 239 241 251 257 263 269 271 277 281
			283 293 307 311 313 317 331 337 347 349
			353 359 367 373 379 383 389 397 401 409
			419 421 431 433 439 443 449 457 461 463
			467 479 487 491 499 503 509 521 523 541
			547 557 563 569 571 577 587 593 599 601
			607 613 617 619 631 641 643 647 653 659
			661 673 677 683 691 701 709 719 727 733
			739 743 751 757 761 769 773 787 797 809
			811 821 823 827 829 839 853 857 859 863
			877 881 883 887 907 911 919 929 937 941
			947 953 967 971 977 983 991 997
			1009 1013 1019 1021 1031
		*/

		// Each list has a product < 2^31
		private static readonly int[][] primeLists = new int[][]
		{
			new int[]{ 3, 5, 7, 11, 13, 17, 19, 23 },
			new int[]{ 29, 31, 37, 41, 43 },
			new int[]{ 47, 53, 59, 61, 67 },
			new int[]{ 71, 73, 79, 83 },
			new int[]{ 89, 97, 101, 103 },

			new int[]{ 107, 109, 113, 127 },
			new int[]{ 131, 137, 139, 149 },
			new int[]{ 151, 157, 163, 167 },
			new int[]{ 173, 179, 181, 191 },
			new int[]{ 193, 197, 199, 211 },

			new int[]{ 223, 227, 229 },
			new int[]{ 233, 239, 241 },
			new int[]{ 251, 257, 263 },
			new int[]{ 269, 271, 277 },
			new int[]{ 281, 283, 293 },

			new int[]{ 307, 311, 313 },
			new int[]{ 317, 331, 337 },
			new int[]{ 347, 349, 353 },
			new int[]{ 359, 367, 373 },
			new int[]{ 379, 383, 389 },

			new int[]{ 397, 401, 409 },
			new int[]{ 419, 421, 431 },
			new int[]{ 433, 439, 443 },
			new int[]{ 449, 457, 461 },
			new int[]{ 463, 467, 479 },

			new int[]{ 487, 491, 499 },
			new int[]{ 503, 509, 521 },
			new int[]{ 523, 541, 547 },
			new int[]{ 557, 563, 569 },
			new int[]{ 571, 577, 587 },

			new int[]{ 593, 599, 601 },
			new int[]{ 607, 613, 617 },
			new int[]{ 619, 631, 641 },
			new int[]{ 643, 647, 653 },
			new int[]{ 659, 661, 673 },

			new int[]{ 677, 683, 691 },
			new int[]{ 701, 709, 719 },
			new int[]{ 727, 733, 739 },
			new int[]{ 743, 751, 757 },
			new int[]{ 761, 769, 773 },

			new int[]{ 787, 797, 809 },
			new int[]{ 811, 821, 823 },
			new int[]{ 827, 829, 839 },
			new int[]{ 853, 857, 859 },
			new int[]{ 863, 877, 881 },

			new int[]{ 883, 887, 907 },
			new int[]{ 911, 919, 929 },
			new int[]{ 937, 941, 947 },
			new int[]{ 953, 967, 971 },
			new int[]{ 977, 983, 991 },

			new int[]{ 997, 1009, 1013 },
			new int[]{ 1019, 1021, 1031 },
		};

		private static readonly BigInteger Six = BigInteger.ValueOf(6);

		private static readonly int[] primeProducts;
		private static readonly BigInteger[] PrimeProducts;

		static DHParametersHelper()
		{
			primeProducts = new int[primeLists.Length];
			PrimeProducts = new BigInteger[primeLists.Length];

			for (int i = 0; i < primeLists.Length; ++i)
			{
				int[] primeList = primeLists[i];
				int product = 1;
				for (int j = 0; j < primeList.Length; ++j)
				{
					product *= primeList[j];
				}
				primeProducts[i] = product;
				PrimeProducts[i] = BigInteger.ValueOf(product);
			}
		}

		// Finds a pair of prime BigInteger's {p, q: p = 2q + 1}
		internal static BigInteger[] GenerateSafePrimes(
			int             size,
			int             certainty,
			SecureRandom    random)
		{
			BigInteger p, q;
			int qLength = size - 1;

			if (size <= 32)
			{
				for (;;)
				{
					q = new BigInteger(qLength, 2, random);

					p = q.ShiftLeft(1).Add(BigInteger.One);

					if (p.IsProbablePrime(certainty)
						&& (certainty <= 2 || q.IsProbablePrime(certainty)))
							break;
				}
			}
			else
			{
				// Note: Modified from Java version for speed
				for (;;)
				{
					q = new BigInteger(qLength, 0, random);

				retry:
					for (int i = 0; i < primeLists.Length; ++i)
					{
						int test = q.Remainder(PrimeProducts[i]).IntValue;

						if (i == 0)
						{
							int rem3 = test % 3;
							if (rem3 != 2)
							{
								int diff = 2 * rem3 + 2;
								q = q.Add(BigInteger.ValueOf(diff));
								test = (test + diff) % primeProducts[i];
							}
						}

						int[] primeList = primeLists[i];
						for (int j = 0; j < primeList.Length; ++j)
						{
							int prime = primeList[j];
							int qRem = test % prime;
							if (qRem == 0 || qRem == (prime >> 1))
							{
								q = q.Add(Six);
								goto retry;
							}
						}
					}


					if (q.BitLength != qLength)
						continue;

					if (!q.RabinMillerTest(2, random))
						continue;

					p = q.ShiftLeft(1).Add(BigInteger.One);

					if (p.RabinMillerTest(certainty, random)
						&& (certainty <= 2 || q.RabinMillerTest(certainty - 2, random)))
						break;
				}
			}

			return new BigInteger[] { p, q };
		}

		// Select a high order element of the multiplicative group Zp*
		// p and q must be s.t. p = 2*q + 1, where p and q are prime
		internal static BigInteger SelectGenerator(
			BigInteger      p,
			BigInteger      q,
			SecureRandom    random)
		{
			BigInteger pMinusTwo = p.Subtract(BigInteger.Two);
			BigInteger g;

			// Handbook of Applied Cryptography 4.86
			do
			{
				g = CreateInRange(BigInteger.Two, pMinusTwo, random);
			}
			while (g.ModPow(BigInteger.Two, p).Equals(BigInteger.One)
				|| g.ModPow(q, p).Equals(BigInteger.One));

/*
			// RFC 2631 2.1.1 (and see Handbook of Applied Cryptography 4.81)
			do
			{
				BigInteger h = CreateInRange(BigInteger.Two, pMinusTwo, random);

				g = h.ModPow(BigInteger.Two, p);
			}
			while (g.Equals(BigInteger.One));
*/

			return g;
		}

		private static BigInteger CreateInRange(
			BigInteger		min,
			BigInteger		max,
			SecureRandom	random)
		{
			BigInteger x;
			do
			{
				x = new BigInteger(max.BitLength, random);
			}
			while (x.CompareTo(min) < 0 || x.CompareTo(max) > 0);
			return x;
		}
	}
}
