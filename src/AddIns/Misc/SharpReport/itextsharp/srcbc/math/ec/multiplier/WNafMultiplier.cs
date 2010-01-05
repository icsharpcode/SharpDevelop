using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	/**
	* Class implementing the WNAF (Window Non-Adjacent Form) multiplication
	* algorithm.
	*/
	internal class WNafMultiplier
		: ECMultiplier 
	{
		/**
		* Computes the Window NAF (non-adjacent Form) of an integer.
		* @param width The width <code>w</code> of the Window NAF. The width is
		* defined as the minimal number <code>w</code>, such that for any
		* <code>w</code> consecutive digits in the resulting representation, at
		* most one is non-zero.
		* @param k The integer of which the Window NAF is computed.
		* @return The Window NAF of the given width, such that the following holds:
		* <code>k = &#8722;<sub>i=0</sub><sup>l-1</sup> k<sub>i</sub>2<sup>i</sup>
		* </code>, where the <code>k<sub>i</sub></code> denote the elements of the
		* returned <code>sbyte[]</code>.
		*/
		public sbyte[] WindowNaf(sbyte width, BigInteger k)
		{
			// The window NAF is at most 1 element longer than the binary
			// representation of the integer k. sbyte can be used instead of short or
			// int unless the window width is larger than 8. For larger width use
			// short or int. However, a width of more than 8 is not efficient for
			// m = log2(q) smaller than 2305 Bits. Note: Values for m larger than
			// 1000 Bits are currently not used in practice.
			sbyte[] wnaf = new sbyte[k.BitLength + 1];

			// 2^width as short and BigInteger
			short pow2wB = (short)(1 << width);
			BigInteger pow2wBI = BigInteger.ValueOf(pow2wB);

			int i = 0;

			// The actual length of the WNAF
			int length = 0;

			// while k >= 1
			while (k.SignValue > 0)
			{
				// if k is odd
				if (k.TestBit(0))
				{
					// k Mod 2^width
					BigInteger remainder = k.Mod(pow2wBI);

					// if remainder > 2^(width - 1) - 1
					if (remainder.TestBit(width - 1))
					{
						wnaf[i] = (sbyte)(remainder.IntValue - pow2wB);
					}
					else
					{
						wnaf[i] = (sbyte)remainder.IntValue;
					}
					// wnaf[i] is now in [-2^(width-1), 2^(width-1)-1]

					k = k.Subtract(BigInteger.ValueOf(wnaf[i]));
					length = i;
				}
				else
				{
					wnaf[i] = 0;
				}

				// k = k/2
				k = k.ShiftRight(1);
				i++;
			}

			length++;

			// Reduce the WNAF array to its actual length
			sbyte[] wnafShort = new sbyte[length];
			Array.Copy(wnaf, 0, wnafShort, 0, length);
			return wnafShort;
		}

		/**
		* Multiplies <code>this</code> by an integer <code>k</code> using the
		* Window NAF method.
		* @param k The integer by which <code>this</code> is multiplied.
		* @return A new <code>ECPoint</code> which equals <code>this</code>
		* multiplied by <code>k</code>.
		*/
		public ECPoint Multiply(ECPoint p, BigInteger k, PreCompInfo preCompInfo)
		{
			WNafPreCompInfo wnafPreCompInfo;

			if ((preCompInfo != null) && (preCompInfo is WNafPreCompInfo))
			{
				wnafPreCompInfo = (WNafPreCompInfo)preCompInfo;
			}
			else
			{
				// Ignore empty PreCompInfo or PreCompInfo of incorrect type
				wnafPreCompInfo = new WNafPreCompInfo();
			}

			// floor(log2(k))
			int m = k.BitLength;

			// width of the Window NAF
			sbyte width;

			// Required length of precomputation array
			int reqPreCompLen;

			// Determine optimal width and corresponding length of precomputation
			// array based on literature values
			if (m < 13)
			{
				width = 2;
				reqPreCompLen = 1;
			}
			else
			{
				if (m < 41)
				{
					width = 3;
					reqPreCompLen = 2;
				}
				else
				{
					if (m < 121)
					{
						width = 4;
						reqPreCompLen = 4;
					}
					else
					{
						if (m < 337)
						{
							width = 5;
							reqPreCompLen = 8;
						}
						else
						{
							if (m < 897)
							{
								width = 6;
								reqPreCompLen = 16;
							}
							else
							{
								if (m < 2305)
								{
									width = 7;
									reqPreCompLen = 32;
								}
								else 
								{
									width = 8;
									reqPreCompLen = 127;
								}
							}
						}
					}
				}
			}

			// The length of the precomputation array
			int preCompLen = 1;

			ECPoint[] preComp = wnafPreCompInfo.GetPreComp();
			ECPoint twiceP = wnafPreCompInfo.GetTwiceP();

			// Check if the precomputed ECPoints already exist
			if (preComp == null)
			{
				// Precomputation must be performed from scratch, create an empty
				// precomputation array of desired length
				preComp = new ECPoint[]{ p };
			}
			else
			{
				// Take the already precomputed ECPoints to start with
				preCompLen = preComp.Length;
			}

			if (twiceP == null)
			{
				// Compute twice(p)
				twiceP = p.Twice();
			}

			if (preCompLen < reqPreCompLen)
			{
				// Precomputation array must be made bigger, copy existing preComp
				// array into the larger new preComp array
				ECPoint[] oldPreComp = preComp;
				preComp = new ECPoint[reqPreCompLen];
				Array.Copy(oldPreComp, 0, preComp, 0, preCompLen);

				for (int i = preCompLen; i < reqPreCompLen; i++)
				{
					// Compute the new ECPoints for the precomputation array.
					// The values 1, 3, 5, ..., 2^(width-1)-1 times p are
					// computed
					preComp[i] = twiceP.Add(preComp[i - 1]);
				}            
			}

			// Compute the Window NAF of the desired width
			sbyte[] wnaf = WindowNaf(width, k);
			int l = wnaf.Length;

			// Apply the Window NAF to p using the precomputed ECPoint values.
			ECPoint q = p.Curve.Infinity;
			for (int i = l - 1; i >= 0; i--)
			{
				q = q.Twice();

				if (wnaf[i] != 0)
				{
					if (wnaf[i] > 0)
					{
						q = q.Add(preComp[(wnaf[i] - 1)/2]);
					}
					else
					{
						// wnaf[i] < 0
						q = q.Subtract(preComp[(-wnaf[i] - 1)/2]);
					}
				}
			}

			// Set PreCompInfo in ECPoint, such that it is available for next
			// multiplication.
			wnafPreCompInfo.SetPreComp(preComp);
			wnafPreCompInfo.SetTwiceP(twiceP);
			p.SetPreCompInfo(wnafPreCompInfo);
			return q;
		}
	}
}
