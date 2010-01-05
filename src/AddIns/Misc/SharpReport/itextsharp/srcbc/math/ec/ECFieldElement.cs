using System;
using System.Diagnostics;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class ECFieldElement
	{
		public abstract BigInteger ToBigInteger();
		public abstract string FieldName { get; }
		public abstract int FieldSize { get; }
		public abstract ECFieldElement Add(ECFieldElement b);
		public abstract ECFieldElement Subtract(ECFieldElement b);
		public abstract ECFieldElement Multiply(ECFieldElement b);
		public abstract ECFieldElement Divide(ECFieldElement b);
		public abstract ECFieldElement Negate();
		public abstract ECFieldElement Square();
		public abstract ECFieldElement Invert();
		public abstract ECFieldElement Sqrt();

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			ECFieldElement other = obj as ECFieldElement;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			ECFieldElement other)
		{
			return ToBigInteger().Equals(other.ToBigInteger());
		}

		public override int GetHashCode()
		{
			return ToBigInteger().GetHashCode();
		}

		public override string ToString()
		{
			return this.ToBigInteger().ToString(2);
		}
	}

	public class FpFieldElement
		: ECFieldElement
	{
		private readonly BigInteger q, x;

		public FpFieldElement(
			BigInteger	q,
			BigInteger	x)
		{
			if (x.CompareTo(q) >= 0)
				throw new ArgumentException("x value too large in field element");

			this.q = q;
			this.x = x;
		}

		public override BigInteger ToBigInteger()
		{
			return x;
		}

		/**
		 * return the field name for this field.
		 *
		 * @return the string "Fp".
		 */
		public override string FieldName
		{
			get { return "Fp"; }
		}

		public override int FieldSize
		{
			get { return q.BitLength; }
		}

		public BigInteger Q
		{
			get { return q; }
		}

		public override ECFieldElement Add(
			ECFieldElement b)
		{
			return new FpFieldElement(q, x.Add(b.ToBigInteger()).Mod(q));
		}

		public override ECFieldElement Subtract(
			ECFieldElement b)
		{
			return new FpFieldElement(q, x.Subtract(b.ToBigInteger()).Mod(q));
		}

		public override ECFieldElement Multiply(
			ECFieldElement b)
		{
			return new FpFieldElement(q, x.Multiply(b.ToBigInteger()).Mod(q));
		}

		public override ECFieldElement Divide(
			ECFieldElement b)
		{
			return new FpFieldElement(q, x.Multiply(b.ToBigInteger().ModInverse(q)).Mod(q));
		}

		public override ECFieldElement Negate()
		{
			return new FpFieldElement(q, x.Negate().Mod(q));
		}

		public override ECFieldElement Square()
		{
			return new FpFieldElement(q, x.Multiply(x).Mod(q));
		}

		public override ECFieldElement Invert()
		{
			return new FpFieldElement(q, x.ModInverse(q));
		}

		// D.1.4 91
		/**
		 * return a sqrt root - the routine verifies that the calculation
		 * returns the right value - if none exists it returns null.
		 */
		public override ECFieldElement Sqrt()
		{
			if (!q.TestBit(0))
				throw Platform.CreateNotImplementedException("even value of q");

			// p mod 4 == 3
			if (q.TestBit(1))
			{
				// TODO Can this be optimised (inline the Square?)
				// z = g^(u+1) + p, p = 4u + 3
				ECFieldElement z = new FpFieldElement(q, x.ModPow(q.ShiftRight(2).Add(BigInteger.One), q));

				return z.Square().Equals(this) ? z : null;
			}

			// p mod 4 == 1
			BigInteger qMinusOne = q.Subtract(BigInteger.One);

			BigInteger legendreExponent = qMinusOne.ShiftRight(1);
			if (!(x.ModPow(legendreExponent, q).Equals(BigInteger.One)))
				return null;

			BigInteger u = qMinusOne.ShiftRight(2);
			BigInteger k = u.ShiftLeft(1).Add(BigInteger.One);

			BigInteger Q = this.x;
			BigInteger fourQ = Q.ShiftLeft(2).Mod(q);

			BigInteger U, V;
			do
			{
				Random rand = new Random();
				BigInteger P;
				do
				{
					P = new BigInteger(q.BitLength, rand);
				}
				while (P.CompareTo(q) >= 0
					|| !(P.Multiply(P).Subtract(fourQ).ModPow(legendreExponent, q).Equals(qMinusOne)));

				BigInteger[] result = fastLucasSequence(q, P, Q, k);
				U = result[0];
				V = result[1];

				if (V.Multiply(V).Mod(q).Equals(fourQ))
				{
					// Integer division by 2, mod q
					if (V.TestBit(0))
					{
						V = V.Add(q);
					}

					V = V.ShiftRight(1);

					Debug.Assert(V.Multiply(V).Mod(q).Equals(x));

					return new FpFieldElement(q, V);
				}
			}
			while (U.Equals(BigInteger.One) || U.Equals(qMinusOne));

			return null;


//			BigInteger qMinusOne = q.Subtract(BigInteger.One);
//
//			BigInteger legendreExponent = qMinusOne.ShiftRight(1);
//			if (!(x.ModPow(legendreExponent, q).Equals(BigInteger.One)))
//				return null;
//
//			Random rand = new Random();
//			BigInteger fourX = x.ShiftLeft(2);
//
//			BigInteger r;
//			do
//			{
//				r = new BigInteger(q.BitLength, rand);
//			}
//			while (r.CompareTo(q) >= 0
//				|| !(r.Multiply(r).Subtract(fourX).ModPow(legendreExponent, q).Equals(qMinusOne)));
//
//			BigInteger n1 = qMinusOne.ShiftRight(2);
//			BigInteger n2 = n1.Add(BigInteger.One);
//
//			BigInteger wOne = WOne(r, x, q);
//			BigInteger wSum = W(n1, wOne, q).Add(W(n2, wOne, q)).Mod(q);
//			BigInteger twoR = r.ShiftLeft(1);
//
//			BigInteger root = twoR.ModPow(q.Subtract(BigInteger.Two), q)
//				.Multiply(x).Mod(q)
//				.Multiply(wSum).Mod(q);
//
//			return new FpFieldElement(q, root);
		}

//		private static BigInteger W(BigInteger n, BigInteger wOne, BigInteger p)
//		{
//			if (n.Equals(BigInteger.One))
//				return wOne;
//
//			bool isEven = !n.TestBit(0);
//			n = n.ShiftRight(1);
//			if (isEven)
//			{
//				BigInteger w = W(n, wOne, p);
//				return w.Multiply(w).Subtract(BigInteger.Two).Mod(p);
//			}
//			BigInteger w1 = W(n.Add(BigInteger.One), wOne, p);
//			BigInteger w2 = W(n, wOne, p);
//			return w1.Multiply(w2).Subtract(wOne).Mod(p);
//		}
//
//		private BigInteger WOne(BigInteger r, BigInteger x, BigInteger p)
//		{
//			return r.Multiply(r).Multiply(x.ModPow(q.Subtract(BigInteger.Two), q)).Subtract(BigInteger.Two).Mod(p);
//		}

		private static BigInteger[] fastLucasSequence(
			BigInteger	p,
			BigInteger	P,
			BigInteger	Q,
			BigInteger	k)
		{
			// TODO Research and apply "common-multiplicand multiplication here"

			int n = k.BitLength;
			int s = k.GetLowestSetBit();

			Debug.Assert(k.TestBit(s));

			BigInteger Uh = BigInteger.One;
			BigInteger Vl = BigInteger.Two;
			BigInteger Vh = P;
			BigInteger Ql = BigInteger.One;
			BigInteger Qh = BigInteger.One;

			for (int j = n - 1; j >= s + 1; --j)
			{
				Ql = Ql.Multiply(Qh).Mod(p);

				if (k.TestBit(j))
				{
					Qh = Ql.Multiply(Q).Mod(p);
					Uh = Uh.Multiply(Vh).Mod(p);
					Vl = Vh.Multiply(Vl).Subtract(P.Multiply(Ql)).Mod(p);
					Vh = Vh.Multiply(Vh).Subtract(Qh.ShiftLeft(1)).Mod(p);
				}
				else
				{
					Qh = Ql;
					Uh = Uh.Multiply(Vl).Subtract(Ql).Mod(p);
					Vh = Vh.Multiply(Vl).Subtract(P.Multiply(Ql)).Mod(p);
					Vl = Vl.Multiply(Vl).Subtract(Ql.ShiftLeft(1)).Mod(p);
				}
			}

			Ql = Ql.Multiply(Qh).Mod(p);
			Qh = Ql.Multiply(Q).Mod(p);
			Uh = Uh.Multiply(Vl).Subtract(Ql).Mod(p);
			Vl = Vh.Multiply(Vl).Subtract(P.Multiply(Ql)).Mod(p);
			Ql = Ql.Multiply(Qh).Mod(p);

			for (int j = 1; j <= s; ++j)
			{
				Uh = Uh.Multiply(Vl).Mod(p);
				Vl = Vl.Multiply(Vl).Subtract(Ql.ShiftLeft(1)).Mod(p);
				Ql = Ql.Multiply(Ql).Mod(p);
			}

			return new BigInteger[]{ Uh, Vl };
		}

//		private static BigInteger[] verifyLucasSequence(
//			BigInteger	p,
//			BigInteger	P,
//			BigInteger	Q,
//			BigInteger	k)
//		{
//			BigInteger[] actual = fastLucasSequence(p, P, Q, k);
//			BigInteger[] plus1 = fastLucasSequence(p, P, Q, k.Add(BigInteger.One));
//			BigInteger[] plus2 = fastLucasSequence(p, P, Q, k.Add(BigInteger.Two));
//
//			BigInteger[] check = stepLucasSequence(p, P, Q, actual, plus1);
//
//			Debug.Assert(check[0].Equals(plus2[0]));
//			Debug.Assert(check[1].Equals(plus2[1]));
//
//			return actual;
//		}
//
//		private static BigInteger[] stepLucasSequence(
//			BigInteger		p,
//			BigInteger		P,
//			BigInteger		Q,
//			BigInteger[]	backTwo,
//			BigInteger[]	backOne)
//		{
//			return new BigInteger[]
//			{
//				P.Multiply(backOne[0]).Subtract(Q.Multiply(backTwo[0])).Mod(p),
//				P.Multiply(backOne[1]).Subtract(Q.Multiply(backTwo[1])).Mod(p)
//			};
//		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			FpFieldElement other = obj as FpFieldElement;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			FpFieldElement other)
		{
			return q.Equals(other.q) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return q.GetHashCode() ^ base.GetHashCode();
		}
	}

//	/**
//	 * Class representing the Elements of the finite field
//	 * <code>F<sub>2<sup>m</sup></sub></code> in polynomial basis (PB)
//	 * representation. Both trinomial (Tpb) and pentanomial (Ppb) polynomial
//	 * basis representations are supported. Gaussian normal basis (GNB)
//	 * representation is not supported.
//	 */
//	public class F2mFieldElement
//		: ECFieldElement
//	{
//		/**
//		 * Indicates gaussian normal basis representation (GNB). Number chosen
//		 * according to X9.62. GNB is not implemented at present.
//		 */
//		public const int Gnb = 1;
//
//		/**
//		 * Indicates trinomial basis representation (Tpb). Number chosen
//		 * according to X9.62.
//		 */
//		public const int Tpb = 2;
//
//		/**
//		 * Indicates pentanomial basis representation (Ppb). Number chosen
//		 * according to X9.62.
//		 */
//		public const int Ppb = 3;
//
//		/**
//		 * Tpb or Ppb.
//		 */
//		private int representation;
//
//		/**
//		 * The exponent <code>m</code> of <code>F<sub>2<sup>m</sup></sub></code>.
//		 */
//		private int m;
//
//		/**
//		 * Tpb: The integer <code>k</code> where <code>x<sup>m</sup> +
//		 * x<sup>k</sup> + 1</code> represents the reduction polynomial
//		 * <code>f(z)</code>.<br/>
//		 * Ppb: The integer <code>k1</code> where <code>x<sup>m</sup> +
//		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//		 * represents the reduction polynomial <code>f(z)</code>.<br/>
//		 */
//		private int k1;
//
//		/**
//		 * Tpb: Always set to <code>0</code><br/>
//		 * Ppb: The integer <code>k2</code> where <code>x<sup>m</sup> +
//		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//		 * represents the reduction polynomial <code>f(z)</code>.<br/>
//		 */
//		private int k2;
//
//		/**
//			* Tpb: Always set to <code>0</code><br/>
//			* Ppb: The integer <code>k3</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.<br/>
//			*/
//		private int k3;
//
//		/**
//			* Constructor for Ppb.
//			* @param m  The exponent <code>m</code> of
//			* <code>F<sub>2<sup>m</sup></sub></code>.
//			* @param k1 The integer <code>k1</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.
//			* @param k2 The integer <code>k2</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.
//			* @param k3 The integer <code>k3</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.
//			* @param x The BigInteger representing the value of the field element.
//			*/
//		public F2mFieldElement(
//			int			m,
//			int			k1,
//			int			k2,
//			int			k3,
//			BigInteger	x)
//			: base(x)
//		{
//			if ((k2 == 0) && (k3 == 0))
//			{
//				this.representation = Tpb;
//			}
//			else
//			{
//				if (k2 >= k3)
//					throw new ArgumentException("k2 must be smaller than k3");
//				if (k2 <= 0)
//					throw new ArgumentException("k2 must be larger than 0");
//
//				this.representation = Ppb;
//			}
//
//			if (x.SignValue < 0)
//				throw new ArgumentException("x value cannot be negative");
//
//			this.m = m;
//			this.k1 = k1;
//			this.k2 = k2;
//			this.k3 = k3;
//		}
//
//		/**
//			* Constructor for Tpb.
//			* @param m  The exponent <code>m</code> of
//			* <code>F<sub>2<sup>m</sup></sub></code>.
//			* @param k The integer <code>k</code> where <code>x<sup>m</sup> +
//			* x<sup>k</sup> + 1</code> represents the reduction
//			* polynomial <code>f(z)</code>.
//			* @param x The BigInteger representing the value of the field element.
//			*/
//		public F2mFieldElement(
//			int			m,
//			int			k,
//			BigInteger	x)
//			: this(m, k, 0, 0, x)
//		{
//			// Set k1 to k, and set k2 and k3 to 0
//		}
//
//		public override string FieldName
//		{
//			get { return "F2m"; }
//		}
//
//		/**
//		* Checks, if the ECFieldElements <code>a</code> and <code>b</code>
//		* are elements of the same field <code>F<sub>2<sup>m</sup></sub></code>
//		* (having the same representation).
//		* @param a field element.
//		* @param b field element to be compared.
//		* @throws ArgumentException if <code>a</code> and <code>b</code>
//		* are not elements of the same field
//		* <code>F<sub>2<sup>m</sup></sub></code> (having the same
//		* representation).
//		*/
//		public static void CheckFieldElements(
//			ECFieldElement	a,
//			ECFieldElement	b)
//		{
//			if (!(a is F2mFieldElement) || !(b is F2mFieldElement))
//			{
//				throw new ArgumentException("Field elements are not "
//					+ "both instances of F2mFieldElement");
//			}
//
//			if ((a.x.SignValue < 0) || (b.x.SignValue < 0))
//			{
//				throw new ArgumentException(
//					"x value may not be negative");
//			}
//
//			F2mFieldElement aF2m = (F2mFieldElement)a;
//			F2mFieldElement bF2m = (F2mFieldElement)b;
//
//			if ((aF2m.m != bF2m.m) || (aF2m.k1 != bF2m.k1)
//				|| (aF2m.k2 != bF2m.k2) || (aF2m.k3 != bF2m.k3))
//			{
//				throw new ArgumentException("Field elements are not "
//					+ "elements of the same field F2m");
//			}
//
//			if (aF2m.representation != bF2m.representation)
//			{
//				// Should never occur
//				throw new ArgumentException(
//					"One of the field "
//					+ "elements are not elements has incorrect representation");
//			}
//		}
//
//		/**
//			* Computes <code>z * a(z) mod f(z)</code>, where <code>f(z)</code> is
//			* the reduction polynomial of <code>this</code>.
//			* @param a The polynomial <code>a(z)</code> to be multiplied by
//			* <code>z mod f(z)</code>.
//			* @return <code>z * a(z) mod f(z)</code>
//			*/
//		private BigInteger multZModF(
//			BigInteger a)
//		{
//			// Left-shift of a(z)
//			BigInteger az = a.ShiftLeft(1);
//			if (az.TestBit(this.m))
//			{
//				// If the coefficient of z^m in a(z) Equals 1, reduction
//				// modulo f(z) is performed: Add f(z) to to a(z):
//				// Step 1: Unset mth coeffient of a(z)
//				az = az.ClearBit(this.m);
//
//				// Step 2: Add r(z) to a(z), where r(z) is defined as
//				// f(z) = z^m + r(z), and k1, k2, k3 are the positions of
//				// the non-zero coefficients in r(z)
//				az = az.FlipBit(0);
//				az = az.FlipBit(this.k1);
//				if (this.representation == Ppb)
//				{
//					az = az.FlipBit(this.k2);
//					az = az.FlipBit(this.k3);
//				}
//			}
//			return az;
//		}
//
//		public override ECFieldElement Add(
//			ECFieldElement b)
//		{
//			// No check performed here for performance reasons. Instead the
//			// elements involved are checked in ECPoint.F2m
//			// checkFieldElements(this, b);
//			if (b.x.SignValue == 0)
//				return this;
//
//			return new F2mFieldElement(this.m, this.k1, this.k2, this.k3, this.x.Xor(b.x));
//		}
//
//		public override ECFieldElement Subtract(
//			ECFieldElement b)
//		{
//			// Addition and subtraction are the same in F2m
//			return Add(b);
//		}
//
//		public override ECFieldElement Multiply(
//			ECFieldElement b)
//		{
//			// Left-to-right shift-and-add field multiplication in F2m
//			// Input: Binary polynomials a(z) and b(z) of degree at most m-1
//			// Output: c(z) = a(z) * b(z) mod f(z)
//
//			// No check performed here for performance reasons. Instead the
//			// elements involved are checked in ECPoint.F2m
//			// checkFieldElements(this, b);
//			BigInteger az = this.x;
//			BigInteger bz = b.x;
//			BigInteger cz;
//
//			// Compute c(z) = a(z) * b(z) mod f(z)
//			if (az.TestBit(0))
//			{
//				cz = bz;
//			}
//			else
//			{
//				cz = BigInteger.Zero;
//			}
//
//			for (int i = 1; i < this.m; i++)
//			{
//				// b(z) := z * b(z) mod f(z)
//				bz = multZModF(bz);
//
//				if (az.TestBit(i))
//				{
//					// If the coefficient of x^i in a(z) Equals 1, b(z) is added
//					// to c(z)
//					cz = cz.Xor(bz);
//				}
//			}
//			return new F2mFieldElement(m, this.k1, this.k2, this.k3, cz);
//		}
//
//
//		public override ECFieldElement Divide(
//			ECFieldElement b)
//		{
//			// There may be more efficient implementations
//			ECFieldElement bInv = b.Invert();
//			return Multiply(bInv);
//		}
//
//		public override ECFieldElement Negate()
//		{
//			// -x == x holds for all x in F2m
//			return this;
//		}
//
//		public override ECFieldElement Square()
//		{
//			// Naive implementation, can probably be speeded up using modular
//			// reduction
//			return Multiply(this);
//		}
//
//		public override ECFieldElement Invert()
//		{
//			// Inversion in F2m using the extended Euclidean algorithm
//			// Input: A nonzero polynomial a(z) of degree at most m-1
//			// Output: a(z)^(-1) mod f(z)
//
//			// u(z) := a(z)
//			BigInteger uz = this.x;
//			if (uz.SignValue <= 0)
//			{
//				throw new ArithmeticException("x is zero or negative, " +
//					"inversion is impossible");
//			}
//
//			// v(z) := f(z)
//			BigInteger vz = BigInteger.One.ShiftLeft(m);
//			vz = vz.SetBit(0);
//			vz = vz.SetBit(this.k1);
//			if (this.representation == Ppb)
//			{
//				vz = vz.SetBit(this.k2);
//				vz = vz.SetBit(this.k3);
//			}
//
//			// g1(z) := 1, g2(z) := 0
//			BigInteger g1z = BigInteger.One;
//			BigInteger g2z = BigInteger.Zero;
//
//			// while u != 1
//			while (uz.SignValue != 0)
//			{
//				// j := deg(u(z)) - deg(v(z))
//				int j = uz.BitLength - vz.BitLength;
//
//				// If j < 0 then: u(z) <-> v(z), g1(z) <-> g2(z), j := -j
//				if (j < 0)
//				{
//					BigInteger uzCopy = uz;
//					uz = vz;
//					vz = uzCopy;
//
//					BigInteger g1zCopy = g1z;
//					g1z = g2z;
//					g2z = g1zCopy;
//
//					j = -j;
//				}
//
//				// u(z) := u(z) + z^j * v(z)
//				// Note, that no reduction modulo f(z) is required, because
//				// deg(u(z) + z^j * v(z)) <= max(deg(u(z)), j + deg(v(z)))
//				// = max(deg(u(z)), deg(u(z)) - deg(v(z)) + deg(v(z))
//				// = deg(u(z))
//				uz = uz.Xor(vz.ShiftLeft(j));
//
//				// g1(z) := g1(z) + z^j * g2(z)
//				g1z = g1z.Xor(g2z.ShiftLeft(j));
//				//                if (g1z.BitLength() > this.m) {
//				//                    throw new ArithmeticException(
//				//                            "deg(g1z) >= m, g1z = " + g1z.ToString(2));
//				//                }
//			}
//			return new F2mFieldElement(this.m, this.k1, this.k2, this.k3, g2z);
//		}
//
//		public override ECFieldElement Sqrt()
//		{
//			throw new ArithmeticException("Not implemented");
//		}
//
//		/**
//			* @return the representation of the field
//			* <code>F<sub>2<sup>m</sup></sub></code>, either of
//			* {@link F2mFieldElement.Tpb} (trinomial
//			* basis representation) or
//			* {@link F2mFieldElement.Ppb} (pentanomial
//			* basis representation).
//			*/
//		public int Representation
//		{
//			get { return this.representation; }
//		}
//
//		/**
//			* @return the degree <code>m</code> of the reduction polynomial
//			* <code>f(z)</code>.
//			*/
//		public int M
//		{
//			get { return this.m; }
//		}
//
//		/**
//			* @return Tpb: The integer <code>k</code> where <code>x<sup>m</sup> +
//			* x<sup>k</sup> + 1</code> represents the reduction polynomial
//			* <code>f(z)</code>.<br/>
//			* Ppb: The integer <code>k1</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.<br/>
//			*/
//		public int K1
//		{
//			get { return this.k1; }
//		}
//
//		/**
//			* @return Tpb: Always returns <code>0</code><br/>
//			* Ppb: The integer <code>k2</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.<br/>
//			*/
//		public int K2
//		{
//			get { return this.k2; }
//		}
//
//		/**
//			* @return Tpb: Always set to <code>0</code><br/>
//			* Ppb: The integer <code>k3</code> where <code>x<sup>m</sup> +
//			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
//			* represents the reduction polynomial <code>f(z)</code>.<br/>
//			*/
//		public int K3
//		{
//			get { return this.k3; }
//		}
//
//		public override bool Equals(
//			object obj)
//		{
//			if (obj == this)
//				return true;
//
//			F2mFieldElement other = obj as F2mFieldElement;
//
//			if (other == null)
//				return false;
//
//			return Equals(other);
//		}
//
//		protected bool Equals(
//			F2mFieldElement other)
//		{
//			return m == other.m
//				&& k1 == other.k1
//				&& k2 == other.k2
//				&& k3 == other.k3
//				&& representation == other.representation
//				&& base.Equals(other);
//		}
//
//		public override int GetHashCode()
//		{
//			return base.GetHashCode() ^ m ^ k1 ^ k2 ^ k3;
//		}
//	}

	/**
	 * Class representing the Elements of the finite field
	 * <code>F<sub>2<sup>m</sup></sub></code> in polynomial basis (PB)
	 * representation. Both trinomial (Tpb) and pentanomial (Ppb) polynomial
	 * basis representations are supported. Gaussian normal basis (GNB)
	 * representation is not supported.
	 */
	public class F2mFieldElement
		: ECFieldElement
	{
		/**
		 * Indicates gaussian normal basis representation (GNB). Number chosen
		 * according to X9.62. GNB is not implemented at present.
		 */
		public const int Gnb = 1;

		/**
		 * Indicates trinomial basis representation (Tpb). Number chosen
		 * according to X9.62.
		 */
		public const int Tpb = 2;

		/**
		 * Indicates pentanomial basis representation (Ppb). Number chosen
		 * according to X9.62.
		 */
		public const int Ppb = 3;

		/**
		 * Tpb or Ppb.
		 */
		private int representation;

		/**
		 * The exponent <code>m</code> of <code>F<sub>2<sup>m</sup></sub></code>.
		 */
		private int m;

		/**
		 * Tpb: The integer <code>k</code> where <code>x<sup>m</sup> +
		 * x<sup>k</sup> + 1</code> represents the reduction polynomial
		 * <code>f(z)</code>.<br/>
		 * Ppb: The integer <code>k1</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.<br/>
		 */
		private int k1;

		/**
		 * Tpb: Always set to <code>0</code><br/>
		 * Ppb: The integer <code>k2</code> where <code>x<sup>m</sup> +
		 * x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
		 * represents the reduction polynomial <code>f(z)</code>.<br/>
		 */
		private int k2;

		/**
			* Tpb: Always set to <code>0</code><br/>
			* Ppb: The integer <code>k3</code> where <code>x<sup>m</sup> +
			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
			* represents the reduction polynomial <code>f(z)</code>.<br/>
			*/
		private int k3;

		/**
		 * The <code>IntArray</code> holding the bits.
		 */
		private IntArray x;

		/**
		 * The number of <code>int</code>s required to hold <code>m</code> bits.
		 */
		private readonly int t;

		/**
			* Constructor for Ppb.
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
			* @param x The BigInteger representing the value of the field element.
			*/
		public F2mFieldElement(
			int			m,
			int			k1,
			int			k2,
			int			k3,
			BigInteger	x)
		{
			// t = m / 32 rounded up to the next integer
			this.t = (m + 31) >> 5;
			this.x = new IntArray(x, t);

			if ((k2 == 0) && (k3 == 0))
			{
				this.representation = Tpb;
			}
			else
			{
				if (k2 >= k3)
					throw new ArgumentException("k2 must be smaller than k3");
				if (k2 <= 0)
					throw new ArgumentException("k2 must be larger than 0");

				this.representation = Ppb;
			}

			if (x.SignValue < 0)
				throw new ArgumentException("x value cannot be negative");

			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
		}

		/**
			* Constructor for Tpb.
			* @param m  The exponent <code>m</code> of
			* <code>F<sub>2<sup>m</sup></sub></code>.
			* @param k The integer <code>k</code> where <code>x<sup>m</sup> +
			* x<sup>k</sup> + 1</code> represents the reduction
			* polynomial <code>f(z)</code>.
			* @param x The BigInteger representing the value of the field element.
			*/
		public F2mFieldElement(
			int			m,
			int			k,
			BigInteger	x)
			: this(m, k, 0, 0, x)
		{
			// Set k1 to k, and set k2 and k3 to 0
		}

		private F2mFieldElement(int m, int k1, int k2, int k3, IntArray x)
		{
			t = (m + 31) >> 5;
			this.x = x;
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;

			if ((k2 == 0) && (k3 == 0))
			{
				this.representation = Tpb;
			}
			else
			{
				this.representation = Ppb;
			}
		}

		public override BigInteger ToBigInteger()
		{
			return x.ToBigInteger();
		}

		public override string FieldName
		{
			get { return "F2m"; }
		}

		public override int FieldSize
		{
			get { return m; }
		}

		/**
		* Checks, if the ECFieldElements <code>a</code> and <code>b</code>
		* are elements of the same field <code>F<sub>2<sup>m</sup></sub></code>
		* (having the same representation).
		* @param a field element.
		* @param b field element to be compared.
		* @throws ArgumentException if <code>a</code> and <code>b</code>
		* are not elements of the same field
		* <code>F<sub>2<sup>m</sup></sub></code> (having the same
		* representation).
		*/
		public static void CheckFieldElements(
			ECFieldElement	a,
			ECFieldElement	b)
		{
			if (!(a is F2mFieldElement) || !(b is F2mFieldElement))
			{
				throw new ArgumentException("Field elements are not "
					+ "both instances of F2mFieldElement");
			}

			F2mFieldElement aF2m = (F2mFieldElement)a;
			F2mFieldElement bF2m = (F2mFieldElement)b;

			if ((aF2m.m != bF2m.m) || (aF2m.k1 != bF2m.k1)
				|| (aF2m.k2 != bF2m.k2) || (aF2m.k3 != bF2m.k3))
			{
				throw new ArgumentException("Field elements are not "
					+ "elements of the same field F2m");
			}

			if (aF2m.representation != bF2m.representation)
			{
				// Should never occur
				throw new ArgumentException(
					"One of the field "
					+ "elements are not elements has incorrect representation");
			}
		}

		public override ECFieldElement Add(
			ECFieldElement b)
		{
			// No check performed here for performance reasons. Instead the
			// elements involved are checked in ECPoint.F2m
			// checkFieldElements(this, b);
			IntArray iarrClone = (IntArray) this.x.Clone();
			F2mFieldElement bF2m = (F2mFieldElement) b;
			iarrClone.AddShifted(bF2m.x, 0);
			return new F2mFieldElement(m, k1, k2, k3, iarrClone);
		}

		public override ECFieldElement Subtract(
			ECFieldElement b)
		{
			// Addition and subtraction are the same in F2m
			return Add(b);
		}

		public override ECFieldElement Multiply(
			ECFieldElement b)
		{
			// Right-to-left comb multiplication in the IntArray
			// Input: Binary polynomials a(z) and b(z) of degree at most m-1
			// Output: c(z) = a(z) * b(z) mod f(z)

			// No check performed here for performance reasons. Instead the
			// elements involved are checked in ECPoint.F2m
			// checkFieldElements(this, b);
			F2mFieldElement bF2m = (F2mFieldElement) b;
			IntArray mult = x.Multiply(bF2m.x, m);
			mult.Reduce(m, new int[]{k1, k2, k3});
			return new F2mFieldElement(m, k1, k2, k3, mult);
		}

		public override ECFieldElement Divide(
			ECFieldElement b)
		{
			// There may be more efficient implementations
			ECFieldElement bInv = b.Invert();
			return Multiply(bInv);
		}

		public override ECFieldElement Negate()
		{
			// -x == x holds for all x in F2m
			return this;
		}

		public override ECFieldElement Square()
		{
			IntArray squared = x.Square(m);
			squared.Reduce(m, new int[]{k1, k2, k3});
			return new F2mFieldElement(m, k1, k2, k3, squared);
		}

		public override ECFieldElement Invert()
		{
			// Inversion in F2m using the extended Euclidean algorithm
			// Input: A nonzero polynomial a(z) of degree at most m-1
			// Output: a(z)^(-1) mod f(z)

			// u(z) := a(z)
			IntArray uz = (IntArray)this.x.Clone();

			// v(z) := f(z)
			IntArray vz = new IntArray(t);
			vz.SetBit(m);
			vz.SetBit(0);
			vz.SetBit(this.k1);
			if (this.representation == Ppb)
			{
				vz.SetBit(this.k2);
				vz.SetBit(this.k3);
			}

			// g1(z) := 1, g2(z) := 0
			IntArray g1z = new IntArray(t);
			g1z.SetBit(0);
			IntArray g2z = new IntArray(t);

			// while u != 0
			while (uz.GetUsedLength() > 0)
//            while (uz.bitLength() > 1)
			{
				// j := deg(u(z)) - deg(v(z))
				int j = uz.BitLength - vz.BitLength;

				// If j < 0 then: u(z) <-> v(z), g1(z) <-> g2(z), j := -j
				if (j < 0)
				{
                    IntArray uzCopy = uz;
					uz = vz;
					vz = uzCopy;

                    IntArray g1zCopy = g1z;
					g1z = g2z;
					g2z = g1zCopy;

					j = -j;
				}

				// u(z) := u(z) + z^j * v(z)
				// Note, that no reduction modulo f(z) is required, because
				// deg(u(z) + z^j * v(z)) <= max(deg(u(z)), j + deg(v(z)))
				// = max(deg(u(z)), deg(u(z)) - deg(v(z)) + deg(v(z))
				// = deg(u(z))
				// uz = uz.xor(vz.ShiftLeft(j));
				// jInt = n / 32
				int jInt = j >> 5;
				// jInt = n % 32
				int jBit = j & 0x1F;
				IntArray vzShift = vz.ShiftLeft(jBit);
				uz.AddShifted(vzShift, jInt);

				// g1(z) := g1(z) + z^j * g2(z)
//                g1z = g1z.xor(g2z.ShiftLeft(j));
				IntArray g2zShift = g2z.ShiftLeft(jBit);
				g1z.AddShifted(g2zShift, jInt);
			}
			return new F2mFieldElement(this.m, this.k1, this.k2, this.k3, g2z);
		}

		public override ECFieldElement Sqrt()
		{
			throw new ArithmeticException("Not implemented");
		}

		/**
			* @return the representation of the field
			* <code>F<sub>2<sup>m</sup></sub></code>, either of
			* {@link F2mFieldElement.Tpb} (trinomial
			* basis representation) or
			* {@link F2mFieldElement.Ppb} (pentanomial
			* basis representation).
			*/
		public int Representation
		{
			get { return this.representation; }
		}

		/**
			* @return the degree <code>m</code> of the reduction polynomial
			* <code>f(z)</code>.
			*/
		public int M
		{
			get { return this.m; }
		}

		/**
			* @return Tpb: The integer <code>k</code> where <code>x<sup>m</sup> +
			* x<sup>k</sup> + 1</code> represents the reduction polynomial
			* <code>f(z)</code>.<br/>
			* Ppb: The integer <code>k1</code> where <code>x<sup>m</sup> +
			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
			* represents the reduction polynomial <code>f(z)</code>.<br/>
			*/
		public int K1
		{
			get { return this.k1; }
		}

		/**
			* @return Tpb: Always returns <code>0</code><br/>
			* Ppb: The integer <code>k2</code> where <code>x<sup>m</sup> +
			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
			* represents the reduction polynomial <code>f(z)</code>.<br/>
			*/
		public int K2
		{
			get { return this.k2; }
		}

		/**
			* @return Tpb: Always set to <code>0</code><br/>
			* Ppb: The integer <code>k3</code> where <code>x<sup>m</sup> +
			* x<sup>k3</sup> + x<sup>k2</sup> + x<sup>k1</sup> + 1</code>
			* represents the reduction polynomial <code>f(z)</code>.<br/>
			*/
		public int K3
		{
			get { return this.k3; }
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			F2mFieldElement other = obj as F2mFieldElement;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			F2mFieldElement other)
		{
			return m == other.m
				&& k1 == other.k1
				&& k2 == other.k2
				&& k3 == other.k3
				&& representation == other.representation
				&& base.Equals(other);
		}

		public override int GetHashCode()
		{
			return m.GetHashCode()
				^	k1.GetHashCode()
				^	k2.GetHashCode()
				^	k3.GetHashCode()
				^	representation.GetHashCode()
				^	base.GetHashCode();
		}
	}
}
