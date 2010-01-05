using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
    /**
     * ASN.1 def for Elliptic-Curve ECParameters structure. See
     * X9.62, for further details.
     */
    public class X9ECParameters
        : Asn1Encodable
    {
        private X9FieldID	fieldID;
        private ECCurve		curve;
        private ECPoint		g;
        private BigInteger	n;
        private BigInteger	h;
        private byte[]		seed;

		public X9ECParameters(
            Asn1Sequence seq)
        {
            if (!(seq[0] is DerInteger)
               || !((DerInteger) seq[0]).Value.Equals(BigInteger.One))
            {
                throw new ArgumentException("bad version in X9ECParameters");
            }

			X9Curve x9c = null;
            if (seq[2] is X9Curve)
            {
                x9c = (X9Curve) seq[2];
            }
            else
            {
                x9c = new X9Curve(
					new X9FieldID(
						(Asn1Sequence) seq[1]),
						(Asn1Sequence) seq[2]);
            }

			this.curve = x9c.Curve;

			if (seq[3] is X9ECPoint)
            {
                this.g = ((X9ECPoint) seq[3]).Point;
            }
            else
            {
                this.g = new X9ECPoint(curve, (Asn1OctetString) seq[3]).Point;
            }

			this.n = ((DerInteger) seq[4]).Value;
            this.seed = x9c.GetSeed();

			if (seq.Count == 6)
            {
                this.h = ((DerInteger) seq[5]).Value;
            }
            else
            {
                this.h = BigInteger.One;
            }
        }

		public X9ECParameters(
            ECCurve		curve,
            ECPoint		g,
            BigInteger	n)
            : this(curve, g, n, BigInteger.One, null)
        {
        }

		public X9ECParameters(
            ECCurve		curve,
            ECPoint		g,
            BigInteger	n,
            BigInteger	h)
            : this(curve, g, n, h, null)
        {
        }

		public X9ECParameters(
            ECCurve		curve,
            ECPoint		g,
            BigInteger	n,
            BigInteger	h,
            byte[]		seed)
        {
            this.curve = curve;
            this.g = g;
            this.n = n;
            this.h = h;
            this.seed = seed;

			if (curve is FpCurve)
			{
				this.fieldID = new X9FieldID(((FpCurve) curve).Q);
			}
			else if (curve is F2mCurve)
			{
				F2mCurve curveF2m = (F2mCurve) curve;
				this.fieldID = new X9FieldID(curveF2m.M, curveF2m.K1,
					curveF2m.K2, curveF2m.K3);
			}
        }

		public ECCurve Curve
        {
			get { return curve; }
        }

		public ECPoint G
        {
            get { return g; }
        }

		public BigInteger N
        {
            get { return n; }
        }

		public BigInteger H
        {
            get { return h; }
        }

		public byte[] GetSeed()
        {
            return seed;
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *  ECParameters ::= Sequence {
         *      version         Integer { ecpVer1(1) } (ecpVer1),
         *      fieldID         FieldID {{FieldTypes}},
         *      curve           X9Curve,
         *      base            X9ECPoint,
         *      order           Integer,
         *      cofactor        Integer OPTIONAL
         *  }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(
				new DerInteger(1),
				fieldID,
				new X9Curve(curve, seed),
				new X9ECPoint(g),
				new DerInteger(n));

			if (!h.Equals(BigInteger.One))
            {
                v.Add(new DerInteger(h));
            }

			return new DerSequence(v);
        }
    }
}
