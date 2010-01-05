using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
    /**
     * class for describing an ECPoint as a Der object.
     */
    public class X9ECPoint
        : Asn1Encodable
    {
        private readonly ECPoint p;

		public X9ECPoint(
            ECPoint p)
        {
            this.p = p;
        }

		public X9ECPoint(
            ECCurve			c,
            Asn1OctetString	s)
        {
            this.p = c.DecodePoint(s.GetOctets());
        }

		public ECPoint Point
        {
			get { return p; }
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *  ECPoint ::= OCTET STRING
         * </pre>
         * <p>
         * Octet string produced using ECPoint.GetEncoded().</p>
         */
        public override Asn1Object ToAsn1Object()
        {
            return new DerOctetString(p.GetEncoded());
        }
    }
}
