using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.X9
{
    public class X962Parameters
        : Asn1Encodable
    {
        private readonly Asn1Object _params;

		public X962Parameters(
            X9ECParameters ecParameters)
        {
            this._params = ecParameters.ToAsn1Object();
        }

		public X962Parameters(
            DerObjectIdentifier namedCurve)
        {
            this._params = namedCurve;
        }

		public X962Parameters(
            Asn1Object obj)
        {
            this._params = obj;
        }

		public bool IsNamedCurve
        {
			get { return (_params is DerObjectIdentifier); }
        }

		public Asn1Object Parameters
        {
            get { return _params; }
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * Parameters ::= CHOICE {
         *    ecParameters ECParameters,
         *    namedCurve   CURVES.&amp;id({CurveNames}),
         *    implicitlyCA Null
         * }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            return _params;
        }
    }
}
