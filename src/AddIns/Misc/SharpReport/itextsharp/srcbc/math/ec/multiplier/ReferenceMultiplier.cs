namespace Org.BouncyCastle.Math.EC.Multiplier
{
	internal class ReferenceMultiplier
		: ECMultiplier
	{
		/**
		* Simple shift-and-add multiplication. Serves as reference implementation
		* to verify (possibly faster) implementations in
		* {@link org.bouncycastle.math.ec.ECPoint ECPoint}.
		* 
		* @param p The point to multiply.
		* @param k The factor by which to multiply.
		* @return The result of the point multiplication <code>k * p</code>.
		*/
		public ECPoint Multiply(ECPoint p, BigInteger k, PreCompInfo preCompInfo)
		{
			ECPoint q = p.Curve.Infinity;
			int t = k.BitLength;
			for (int i = 0; i < t; i++)
			{
				if (k.TestBit(i))
				{
					q = q.Add(p);
				}
				p = p.Twice();
			}
			return q;
		}
	}
}
