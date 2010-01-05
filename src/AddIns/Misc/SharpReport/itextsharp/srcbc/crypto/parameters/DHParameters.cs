using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class DHParameters
		: ICipherParameters
    {
		private const int DefaultMinimumLength = 160;

		private readonly BigInteger p, g, q, j;
		private readonly int m, l;
		private readonly DHValidationParameters validation;

		private static int GetDefaultM(
			BigInteger	p,
			int			l)
		{
			int effectiveL = l != 0 ? l : p.BitLength - 1;

			return System.Math.Min(DefaultMinimumLength, effectiveL);

//			return DefaultMinimumLength;
		}

		public DHParameters(
			BigInteger	p,
			BigInteger	g)
			: this(p, g, null, GetDefaultM(p, 0), 0, null, null)
		{
		}

		public DHParameters(
			BigInteger	p,
			BigInteger	g,
			BigInteger	q)
			: this(p, g, q, GetDefaultM(p, 0), 0, null, null)
		{
		}

		public DHParameters(
			BigInteger	p,
			BigInteger	g,
			BigInteger	q,
			int			l)
			: this(p, g, q, GetDefaultM(p, l), l, null, null)
		{
		}   

		public DHParameters(
			BigInteger  p,
			BigInteger  g,
			BigInteger  q,
			int         m,
			int         l)
			: this(p, g, q, m, l, null, null)
		{
		}

		public DHParameters(
			BigInteger				p,
			BigInteger				g,
			BigInteger				q,
			BigInteger				j,
			DHValidationParameters	validation)
			: this(p, g, q,  GetDefaultM(p, 0), 0, j, validation)
		{
		}

		public DHParameters(
			BigInteger				p,
			BigInteger				g,
			BigInteger				q,
			int						m,
			int						l,
			BigInteger				j,
			DHValidationParameters	validation)
		{
			if (p == null)
				throw new ArgumentNullException("p");
			if (g == null)
				throw new ArgumentNullException("g");
			if (!p.TestBit(0))
				throw new ArgumentException("field must be an odd prime", "p");
			if (g.CompareTo(BigInteger.Two) < 0
				|| g.CompareTo(p.Subtract(BigInteger.Two)) > 0)
				throw new ArgumentException("generator must in the range [2, p - 2]", "g");
			if (m >= p.BitLength)
				throw new ArgumentException("m value must be < bitlength of p", "m");
			if (l != 0 && l < m)
				throw new ArgumentException("l value must be >= m, or zero", "l");
			if (j != null && j.CompareTo(BigInteger.Two) < 0)
				throw new ArgumentException("subgroup factor must be >= 2", "j");

			this.p = p;
			this.g = g;
			this.q = q;
			this.m = m;
			this.l = l;
			this.j = j;
			this.validation = validation;
        }

		public BigInteger P
        {
            get { return p; }
        }

		public BigInteger G
        {
            get { return g; }
        }

		public BigInteger Q
        {
            get { return q; }
        }

		public BigInteger J
        {
            get { return j; }
        }

		/// <summary>The minimum bitlength of the private value.</summary>
		public int M
		{
			get { return m; }
		}

		/// <summary>The bitlength of the private value.</summary>
		/// <remarks>If zero, bitLength(p) - 1 will be used.</remarks>
		public int L
		{
			get { return l; }
		}

		public DHValidationParameters ValidationParameters
        {
			get { return validation; }
        }

		public override bool Equals(
			object obj)
        {
			if (obj == this)
				return true;

			DHParameters other = obj as DHParameters;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			DHParameters other)
		{
			return p.Equals(other.p)
				&& g.Equals(other.g)
				&& Platform.Equals(q, other.q);
		}

		public override int GetHashCode()
        {
			int hc = p.GetHashCode() ^ g.GetHashCode();

			if (q != null)
			{
				hc ^= q.GetHashCode();
			}

			return hc;
        }
    }
}
