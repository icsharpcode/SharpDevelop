using System;

using Org.BouncyCastle.Math.EC.Abc;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	/**
	* Class implementing the WTNAF (Window
	* <code>&#964;</code>-adic Non-Adjacent Form) algorithm.
	*/
	internal class WTauNafMultiplier
		: ECMultiplier
	{
		/**
		* Multiplies a {@link org.bouncycastle.math.ec.F2mPoint F2mPoint}
		* by <code>k</code> using the reduced <code>&#964;</code>-adic NAF (RTNAF)
		* method.
		* @param p The F2mPoint to multiply.
		* @param k The integer by which to multiply <code>k</code>.
		* @return <code>p</code> multiplied by <code>k</code>.
		*/
		public ECPoint Multiply(ECPoint point, BigInteger k, PreCompInfo preCompInfo)
		{
			if (!(point is F2mPoint))
				throw new ArgumentException("Only F2mPoint can be used in WTauNafMultiplier");

			F2mPoint p = (F2mPoint)point;

			F2mCurve curve = (F2mCurve) p.Curve;
			int m = curve.M;
			sbyte a = (sbyte) curve.A.ToBigInteger().IntValue;
			sbyte mu = curve.GetMu();
			BigInteger[] s = curve.GetSi();

			ZTauElement rho = Tnaf.PartModReduction(k, m, a, s, mu, (sbyte)10);

			return MultiplyWTnaf(p, rho, preCompInfo, a, mu);
		}

		/**
		* Multiplies a {@link org.bouncycastle.math.ec.F2mPoint F2mPoint}
		* by an element <code>&#955;</code> of <code><b>Z</b>[&#964;]</code> using
		* the <code>&#964;</code>-adic NAF (TNAF) method.
		* @param p The F2mPoint to multiply.
		* @param lambda The element <code>&#955;</code> of
		* <code><b>Z</b>[&#964;]</code> of which to compute the
		* <code>[&#964;]</code>-adic NAF.
		* @return <code>p</code> multiplied by <code>&#955;</code>.
		*/
		private F2mPoint MultiplyWTnaf(F2mPoint p, ZTauElement lambda,
			PreCompInfo preCompInfo, sbyte a, sbyte mu)
		{
			ZTauElement[] alpha;
			if (a == 0)
			{
				alpha = Tnaf.Alpha0;
			}
			else
			{
				// a == 1
				alpha = Tnaf.Alpha1;
			}

			BigInteger tw = Tnaf.GetTw(mu, Tnaf.Width);

			sbyte[]u = Tnaf.TauAdicWNaf(mu, lambda, Tnaf.Width,
				BigInteger.ValueOf(Tnaf.Pow2Width), tw, alpha);

			return MultiplyFromWTnaf(p, u, preCompInfo);
		}
	    
		/**
		* Multiplies a {@link org.bouncycastle.math.ec.F2mPoint F2mPoint}
		* by an element <code>&#955;</code> of <code><b>Z</b>[&#964;]</code>
		* using the window <code>&#964;</code>-adic NAF (TNAF) method, given the
		* WTNAF of <code>&#955;</code>.
		* @param p The F2mPoint to multiply.
		* @param u The the WTNAF of <code>&#955;</code>..
		* @return <code>&#955; * p</code>
		*/
		private static F2mPoint MultiplyFromWTnaf(F2mPoint p, sbyte[] u,
			PreCompInfo preCompInfo)
		{
			F2mCurve curve = (F2mCurve)p.Curve;
			sbyte a = (sbyte) curve.A.ToBigInteger().IntValue;

			F2mPoint[] pu;
			if ((preCompInfo == null) || !(preCompInfo is WTauNafPreCompInfo))
			{
				pu = Tnaf.GetPreComp(p, a);
				p.SetPreCompInfo(new WTauNafPreCompInfo(pu));
			}
			else
			{
				pu = ((WTauNafPreCompInfo)preCompInfo).GetPreComp();
			}

			// q = infinity
			F2mPoint q = (F2mPoint) p.Curve.Infinity;
			for (int i = u.Length - 1; i >= 0; i--)
			{
				q = Tnaf.Tau(q);
				if (u[i] != 0)
				{
					if (u[i] > 0)
					{
						q = q.AddSimple(pu[u[i]]);
					}
					else
					{
						// u[i] < 0
						q = q.SubtractSimple(pu[-u[i]]);
					}
				}
			}

			return q;
		}
	}
}
