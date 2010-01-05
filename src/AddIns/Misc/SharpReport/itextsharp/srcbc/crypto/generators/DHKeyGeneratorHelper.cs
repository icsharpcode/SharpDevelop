using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Generators
{
	class DHKeyGeneratorHelper
	{
		private const int MAX_ITERATIONS = 1000;

		internal static readonly DHKeyGeneratorHelper Instance = new DHKeyGeneratorHelper();

		private DHKeyGeneratorHelper()
		{
		}

		internal BigInteger CalculatePrivate(
			BigInteger		p,
			SecureRandom	random,
			int				limit)
		{
			//
			// calculate the private key
			//
			BigInteger pSub2 = p.Subtract(BigInteger.Two);
			BigInteger x;

			if (limit == 0)
			{
				x = createInRange(pSub2, random);
			}
			else
			{
				do
				{
					// TODO Check this (should the generated numbers always be odd,
					// and length 'limit'?)
					x = new BigInteger(limit, 0, random);
				}
				while (x.SignValue == 0);
			}

			return x;
		}

		private BigInteger createInRange(
			BigInteger		max,
			SecureRandom	random)
		{
			BigInteger x;
			int maxLength = max.BitLength;
			int count = 0;

			do
			{
				x = new BigInteger(maxLength, random);
				count++;
			}
			while ((x.SignValue == 0 || x.CompareTo(max) > 0) && count != MAX_ITERATIONS);

			if (count == MAX_ITERATIONS)  // fall back to a faster (restricted) method
			{
				return new BigInteger(maxLength - 1, random).SetBit(0);
			}

			return x;
		}

		internal BigInteger CalculatePublic(
			BigInteger	p,
			BigInteger	g,
			BigInteger	x)
		{
			return g.ModPow(x, p);
		}
	}
}
