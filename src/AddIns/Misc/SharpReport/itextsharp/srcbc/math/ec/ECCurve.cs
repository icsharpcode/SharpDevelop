using System;
using System.Collections;

using Org.BouncyCastle.Math.EC.Abc;

namespace Org.BouncyCastle.Math.EC
{
	/// <remarks>Base class for an elliptic curve.</remarks>
	public abstract class ECCurve
	{
		internal ECFieldElement a, b;

		public abstract int FieldSize { get; }
		public abstract ECFieldElement FromBigInteger(BigInteger x);
		public abstract ECPoint CreatePoint(BigInteger x, BigInteger y, bool withCompression);
		public abstract ECPoint DecodePoint(byte[] encoded);
		public abstract ECPoint Infinity { get; }

		public ECFieldElement A
		{
			get { return a; }
		}

		public ECFieldElement B
		{
			get { return b; }
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			ECCurve other = obj as ECCurve;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			ECCurve other)
		{
			return a.Equals(other.a) && b.Equals(other.b);
		}

		public override int GetHashCode()
		{
			return a.GetHashCode() ^ b.GetHashCode();
		}
	}

	public abstract class ECCurveBase : ECCurve
	{
		protected internal ECCurveBase()
		{
		}

		protected internal abstract ECPoint DecompressPoint(int yTilde, BigInteger X1);

		/**
		 * Decode a point on this curve from its ASN.1 encoding. The different
		 * encodings are taken account of, including point compression for
		 * <code>F<sub>p</sub></code> (X9.62 s 4.2.1 pg 17).
		 * @return The decoded point.
		 */
		public override ECPoint DecodePoint(
			byte[] encoded)
		{
			ECPoint p = null;
			int expectedLength = (FieldSize + 7) / 8;

			switch (encoded[0])
			{
				case 0x00: // infinity
				{
					if (encoded.Length != 1)
						throw new ArgumentException("Incorrect length for infinity encoding", "encoded");

					p = Infinity;
					break;
				}

				case 0x02: // compressed
				case 0x03: // compressed
				{
					if (encoded.Length != (expectedLength + 1))
						throw new ArgumentException("Incorrect length for compressed encoding", "encoded");

					int yTilde = encoded[0] & 1;
					BigInteger X1 = new BigInteger(1, encoded, 1, encoded.Length - 1);

					p = DecompressPoint(yTilde, X1);
					break;
				}

				case 0x04: // uncompressed
				case 0x06: // hybrid
				case 0x07: // hybrid
				{
					if (encoded.Length != (2 * expectedLength + 1))
						throw new ArgumentException("Incorrect length for uncompressed/hybrid encoding", "encoded");

					BigInteger X1 = new BigInteger(1, encoded, 1, expectedLength);
					BigInteger Y1 = new BigInteger(1, encoded, 1 + expectedLength, expectedLength);

					p = CreatePoint(X1, Y1, false);
					break;
				}

				default:
					throw new FormatException("Invalid point encoding " + encoded[0]);
			}

			return p;
		}
	}

	/**
     * Elliptic curve over Fp
     */
    public class FpCurve : ECCurveBase
    {
        private readonly BigInteger q;
		private readonly FpPoint infinity;

		public FpCurve(BigInteger q, BigInteger a, BigInteger b)
        {
            this.q = q;
            this.a = FromBigInteger(a);
            this.b = FromBigInteger(b);
			this.infinity = new FpPoint(this, null, null);
        }

		public BigInteger Q
        {
			get { return q; }
        }

		public override ECPoint Infinity
		{
			get { return infinity; }
		}

		public override int FieldSize
		{
			get { return q.BitLength; }
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new FpFieldElement(this.q, x);
        }

		public override ECPoint CreatePoint(
			BigInteger	X1,
			BigInteger	Y1,
			bool		withCompression)
		{
			// TODO Validation of X1, Y1?
			return new FpPoint(
				this,
				FromBigInteger(X1),
				FromBigInteger(Y1),
				withCompression);
		}

		protected internal override ECPoint DecompressPoint(
			int			yTilde,
			BigInteger	X1)
		{
			ECFieldElement x = FromBigInteger(X1);
			ECFieldElement alpha = x.Multiply(x.Square().Add(a)).Add(b);
			ECFieldElement beta = alpha.Sqrt();

			//
			// if we can't find a sqrt we haven't got a point on the
			// curve - run!
			//
			if (beta == null)
				throw new ArithmeticException("Invalid point compression");

			BigInteger betaValue = beta.ToBigInteger();
			int bit0 = betaValue.TestBit(0) ? 1 : 0;

			if (bit0 != yTilde)
			{
				// Use the other root
				beta = FromBigInteger(q.Subtract(betaValue));
			}

			return new FpPoint(this, x, beta, true);
		}

		public override bool Equals(
            object obj)
        {
            if (obj == this)
                return true;

			FpCurve other = obj as FpCurve;

			if (other == null)
                return false;

			return Equals(other);
        }

		protected bool Equals(
			FpCurve other)
		{
			return base.Equals(other) && q.Equals(other.q);
		}

		public override int GetHashCode()
        {
            return base.GetHashCode() ^ q.GetHashCode();
        }
    }

	/**
     * Elliptic curves over F2m. The Weierstrass equation is given by
     * <code>y<sup>2</sup> + xy = x<sup>3</sup> + ax<sup>2</sup> + b</code>.
     */
    public class F2mCurve : ECCurveBase
    {
        /**
         * The exponent <code>m</code> of <code>F<sub>2<sup>m</sup></sub></code>.
         */
        private readonly int m;

        /**
         * TPB: The integer <code>k</code> where <code>x<sup>m</sup> +
         * x<sup>k</sup> + 1</code> represents the reduction polynomial
         * <code>f(z)</code>.<br/>
         * PPB: The integer <code>k1</code> where <code>x<sup>m</sup> +
         * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
         * represents the reduction polynomial <code>f(z)</code>.<br/>
         */
        private readonly int k1;

        /**
         * TPB: Always set to <code>0</code><br/>
         * PPB: The integer <code>k2</code> where <code>x<sup>m</sup> +
         * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
         * represents the reduction polynomial <code>f(z)</code>.<br/>
         */
        private readonly int k2;

        /**
         * TPB: Always set to <code>0</code><br/>
         * PPB: The integer <code>k3</code> where <code>x<sup>m</sup> +
         * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
         * represents the reduction polynomial <code>f(z)</code>.<br/>
         */
        private readonly int k3;

		/**
		 * The order of the base point of the curve.
		 */
		private readonly BigInteger n;

		/**
		 * The cofactor of the curve.
		 */
		private readonly BigInteger h;

		/**
		 * The point at infinity on this curve.
		 */
		private readonly F2mPoint infinity;

		/**
		 * The parameter <code>&#956;</code> of the elliptic curve if this is
		 * a Koblitz curve.
		 */
		private sbyte mu = 0;

		/**
		 * The auxiliary values <code>s<sub>0</sub></code> and
		 * <code>s<sub>1</sub></code> used for partial modular reduction for
		 * Koblitz curves.
		 */
		private BigInteger[] si = null;

		/**
		 * Constructor for Trinomial Polynomial Basis (TPB).
		 * @param m  The exponent <code>m</code> of
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param k The integer <code>k</code> where <code>x<sup>m</sup> +
		 * x<sup>k</sup> + 1</code> represents the reduction
		 * polynomial <code>f(z)</code>.
		 * @param a The coefficient <code>a</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param b The coefficient <code>b</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 */
		public F2mCurve(
			int			m,
			int			k,
			BigInteger	a,
			BigInteger	b)
			: this(m, k, 0, 0, a, b, null, null)
		{
		}

		/**
		 * Constructor for Trinomial Polynomial Basis (TPB).
		 * @param m  The exponent <code>m</code> of
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param k The integer <code>k</code> where <code>x<sup>m</sup> +
		 * x<sup>k</sup> + 1</code> represents the reduction
		 * polynomial <code>f(z)</code>.
		 * @param a The coefficient <code>a</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param b The coefficient <code>b</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param n The order of the main subgroup of the elliptic curve.
		 * @param h The cofactor of the elliptic curve, i.e.
		 * <code>#E<sub>a</sub>(F<sub>2<sup>m</sup></sub>) = h * n</code>.
		 */
		public F2mCurve(
			int			m, 
			int			k, 
			BigInteger	a, 
			BigInteger	b,
			BigInteger	n,
			BigInteger	h)
			: this(m, k, 0, 0, a, b, n, h)
		{
		}

		/**
		 * Constructor for Pentanomial Polynomial Basis (PPB).
		 * @param m  The exponent <code>m</code> of
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param k1 The integer <code>k1</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.
		 * @param k2 The integer <code>k2</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.
		 * @param k3 The integer <code>k3</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.
		 * @param a The coefficient <code>a</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param b The coefficient <code>b</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 */
		public F2mCurve(
			int			m,
			int			k1,
			int			k2,
			int			k3,
			BigInteger	a,
			BigInteger	b)
			: this(m, k1, k2, k3, a, b, null, null)
		{
		}

		/**
		 * Constructor for Pentanomial Polynomial Basis (PPB).
		 * @param m  The exponent <code>m</code> of
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param k1 The integer <code>k1</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.
		 * @param k2 The integer <code>k2</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.
		 * @param k3 The integer <code>k3</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.
		 * @param a The coefficient <code>a</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param b The coefficient <code>b</code> in the Weierstrass equation
		 * for non-supersingular elliptic curves over
		 * <code>F<sub>2<sup>m</sup></sub></code>.
		 * @param n The order of the main subgroup of the elliptic curve.
		 * @param h The cofactor of the elliptic curve, i.e.
		 * <code>#E<sub>a</sub>(F<sub>2<sup>m</sup></sub>) = h * n</code>.
		 */
		public F2mCurve(
			int			m, 
			int			k1, 
			int			k2, 
			int			k3,
			BigInteger	a, 
			BigInteger	b,
			BigInteger	n,
			BigInteger	h)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			this.n = n;
			this.h = h;
			this.infinity = new F2mPoint(this, null, null);

			if (k1 == 0)
                throw new ArgumentException("k1 must be > 0");

			if (k2 == 0)
            {
                if (k3 != 0)
                    throw new ArgumentException("k3 must be 0 if k2 == 0");
            }
            else
            {
                if (k2 <= k1)
                    throw new ArgumentException("k2 must be > k1");

				if (k3 <= k2)
                    throw new ArgumentException("k3 must be > k2");
            }

			this.a = FromBigInteger(a);
            this.b = FromBigInteger(b);
        }

		public override ECPoint Infinity
		{
			get { return infinity; }
		}

		public override int FieldSize
		{
			get { return m; }
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new F2mFieldElement(this.m, this.k1, this.k2, this.k3, x);
        }

		/**
		 * Returns true if this is a Koblitz curve (ABC curve).
		 * @return true if this is a Koblitz curve (ABC curve), false otherwise
		 */
		public bool IsKoblitz
		{
			get
			{
				return n != null && h != null
					&& (a.ToBigInteger().Equals(BigInteger.Zero)
						|| a.ToBigInteger().Equals(BigInteger.One))
					&& b.ToBigInteger().Equals(BigInteger.One);
			}
		}

		/**
		 * Returns the parameter <code>&#956;</code> of the elliptic curve.
		 * @return <code>&#956;</code> of the elliptic curve.
		 * @throws ArgumentException if the given ECCurve is not a
		 * Koblitz curve.
		 */
		internal sbyte GetMu()
		{
			if (mu == 0)
			{
				lock (this)
				{
					if (mu == 0)
					{
						mu = Tnaf.GetMu(this);
					}
				}
			}

			return mu;
		}

		/**
		 * @return the auxiliary values <code>s<sub>0</sub></code> and
		 * <code>s<sub>1</sub></code> used for partial modular reduction for
		 * Koblitz curves.
		 */
		internal BigInteger[] GetSi()
		{
			if (si == null)
			{
				lock (this)
				{
					if (si == null)
					{
						si = Tnaf.GetSi(this);
					}
				}
			}
			return si;
		}

		public override ECPoint CreatePoint(
			BigInteger	X1,
			BigInteger	Y1,
			bool		withCompression)
		{
			// TODO Validation of X1, Y1?
			return new F2mPoint(
				this,
				FromBigInteger(X1),
				FromBigInteger(Y1),
				withCompression);
		}

		protected internal override ECPoint DecompressPoint(
			int			yTilde,
			BigInteger	X1)
		{
			ECFieldElement xp = FromBigInteger(X1);
			ECFieldElement yp = null;
			if (xp.ToBigInteger().SignValue == 0)
			{
				yp = (F2mFieldElement)b;
				for (int i = 0; i < m - 1; i++)
				{
					yp = yp.Square();
				}
			}
			else
			{
				ECFieldElement beta = xp.Add(a).Add(
					b.Multiply(xp.Square().Invert()));
				ECFieldElement z = solveQuadradicEquation(beta);

				if (z == null)
					throw new ArithmeticException("Invalid point compression");

				int zBit = z.ToBigInteger().TestBit(0) ? 1 : 0;
				if (zBit != yTilde)
				{
					z = z.Add(FromBigInteger(BigInteger.One));
				}

				yp = xp.Multiply(z);
			}

			return new F2mPoint(this, xp, yp, true);
		}

		/**
         * Solves a quadratic equation <code>z<sup>2</sup> + z = beta</code>(X9.62
         * D.1.6) The other solution is <code>z + 1</code>.
         *
         * @param beta
         *            The value to solve the qradratic equation for.
         * @return the solution for <code>z<sup>2</sup> + z = beta</code> or
         *         <code>null</code> if no solution exists.
         */
        private ECFieldElement solveQuadradicEquation(ECFieldElement beta)
        {
            if (beta.ToBigInteger().SignValue == 0)
            {
                return FromBigInteger(BigInteger.Zero);
            }

			ECFieldElement z = null;
            ECFieldElement gamma = FromBigInteger(BigInteger.Zero);

			while (gamma.ToBigInteger().SignValue == 0)
            {
                ECFieldElement t = FromBigInteger(new BigInteger(m, new Random()));
				z = FromBigInteger(BigInteger.Zero);

				ECFieldElement w = beta;
                for (int i = 1; i <= m - 1; i++)
                {
					ECFieldElement w2 = w.Square();
                    z = z.Square().Add(w2.Multiply(t));
                    w = w2.Add(beta);
                }
                if (w.ToBigInteger().SignValue != 0)
                {
                    return null;
                }
                gamma = z.Square().Add(z);
            }
            return z;
        }

		public override bool Equals(
            object obj)
        {
            if (obj == this)
                return true;

			F2mCurve other = obj as F2mCurve;

			if (other == null)
                return false;

			return Equals(other);
        }

		protected bool Equals(
			F2mCurve other)
		{
			return m == other.m
				&& k1 == other.k1
				&& k2 == other.k2
				&& k3 == other.k3
				&& base.Equals(other);
		}

		public override int GetHashCode()
        {
            return base.GetHashCode() ^ m ^ k1 ^ k2 ^ k3;
        }

		public int M
        {
			get { return m; }
        }

		/**
         * Return true if curve uses a Trinomial basis.
         *
         * @return true if curve Trinomial, false otherwise.
         */
        public bool IsTrinomial()
        {
            return k2 == 0 && k3 == 0;
        }

		public int K1
        {
			get { return k1; }
        }

		public int K2
        {
			get { return k2; }
        }

		public int K3
        {
			get { return k3; }
        }

		public BigInteger N
		{
			get { return n; }
		}

		public BigInteger H
		{
			get { return h; }
		}
	}
}
