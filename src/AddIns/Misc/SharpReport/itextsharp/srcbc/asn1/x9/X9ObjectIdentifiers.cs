namespace Org.BouncyCastle.Asn1.X9
{
    public abstract class X9ObjectIdentifiers
    {
        //
        // X9.62
        //
        // ansi-X9-62 OBJECT IDENTIFIER ::= { iso(1) member-body(2)
        //            us(840) ansi-x962(10045) }
        //
        internal const string AnsiX962 = "1.2.840.10045";
        internal const string IdFieldType = AnsiX962 + ".1";

		public static readonly DerObjectIdentifier PrimeField
                        = new DerObjectIdentifier(IdFieldType + ".1");

		public static readonly DerObjectIdentifier CharacteristicTwoField
                        = new DerObjectIdentifier(IdFieldType + ".2");

		public static readonly DerObjectIdentifier GNBasis
                        = new DerObjectIdentifier(IdFieldType + ".2.3.1");

		public static readonly DerObjectIdentifier TPBasis
                        = new DerObjectIdentifier(IdFieldType + ".2.3.2");

		public static readonly DerObjectIdentifier PPBasis
                        = new DerObjectIdentifier(IdFieldType + ".2.3.3");

		public const string IdECSigType = AnsiX962 + ".4";

		public static readonly DerObjectIdentifier ECDsaWithSha1
                        = new DerObjectIdentifier(IdECSigType + ".1");

		public const string IdPublicKeyType = AnsiX962 + ".2";

		public static readonly DerObjectIdentifier IdECPublicKey
                        = new DerObjectIdentifier(IdPublicKeyType + ".1");

		public static readonly DerObjectIdentifier ECDsaWithSha2 = new DerObjectIdentifier(IdECSigType + ".3");
		public static readonly DerObjectIdentifier ECDsaWithSha224 = new DerObjectIdentifier(ECDsaWithSha2 + ".1");
		public static readonly DerObjectIdentifier ECDsaWithSha256 = new DerObjectIdentifier(ECDsaWithSha2 + ".2");
		public static readonly DerObjectIdentifier ECDsaWithSha384 = new DerObjectIdentifier(ECDsaWithSha2 + ".3");
		public static readonly DerObjectIdentifier ECDsaWithSha512 = new DerObjectIdentifier(ECDsaWithSha2 + ".4");


		//
        // named curves
        //
        public static readonly DerObjectIdentifier EllipticCurve = new DerObjectIdentifier(AnsiX962 + ".3");

		//
        // Two Curves
        //
        public static readonly DerObjectIdentifier CTwoCurve = new DerObjectIdentifier(EllipticCurve + ".0");

		public static readonly DerObjectIdentifier C2Pnb163v1 = new DerObjectIdentifier(CTwoCurve + ".1");
        public static readonly DerObjectIdentifier C2Pnb163v2 = new DerObjectIdentifier(CTwoCurve + ".2");
        public static readonly DerObjectIdentifier C2Pnb163v3 = new DerObjectIdentifier(CTwoCurve + ".3");
        public static readonly DerObjectIdentifier C2Pnb176w1 = new DerObjectIdentifier(CTwoCurve + ".4");
        public static readonly DerObjectIdentifier C2Tnb191v1 = new DerObjectIdentifier(CTwoCurve + ".5");
        public static readonly DerObjectIdentifier C2Tnb191v2 = new DerObjectIdentifier(CTwoCurve + ".6");
        public static readonly DerObjectIdentifier C2Tnb191v3 = new DerObjectIdentifier(CTwoCurve + ".7");
        public static readonly DerObjectIdentifier C2Onb191v4 = new DerObjectIdentifier(CTwoCurve + ".8");
        public static readonly DerObjectIdentifier C2Onb191v5 = new DerObjectIdentifier(CTwoCurve + ".9");
        public static readonly DerObjectIdentifier C2Pnb208w1 = new DerObjectIdentifier(CTwoCurve + ".10");
        public static readonly DerObjectIdentifier C2Tnb239v1 = new DerObjectIdentifier(CTwoCurve + ".11");
        public static readonly DerObjectIdentifier C2Tnb239v2 = new DerObjectIdentifier(CTwoCurve + ".12");
        public static readonly DerObjectIdentifier C2Tnb239v3 = new DerObjectIdentifier(CTwoCurve + ".13");
        public static readonly DerObjectIdentifier C2Onb239v4 = new DerObjectIdentifier(CTwoCurve + ".14");
        public static readonly DerObjectIdentifier C2Onb239v5 = new DerObjectIdentifier(CTwoCurve + ".15");
        public static readonly DerObjectIdentifier C2Pnb272w1 = new DerObjectIdentifier(CTwoCurve + ".16");
		public static readonly DerObjectIdentifier C2Pnb304w1 = new DerObjectIdentifier(CTwoCurve + ".17");
		public static readonly DerObjectIdentifier C2Tnb359v1 = new DerObjectIdentifier(CTwoCurve + ".18");
        public static readonly DerObjectIdentifier C2Pnb368w1 = new DerObjectIdentifier(CTwoCurve + ".19");
        public static readonly DerObjectIdentifier C2Tnb431r1 = new DerObjectIdentifier(CTwoCurve + ".20");

		//
        // Prime
        //
        public static readonly DerObjectIdentifier PrimeCurve = new DerObjectIdentifier(EllipticCurve + ".1");

		public static readonly DerObjectIdentifier Prime192v1 = new DerObjectIdentifier(PrimeCurve + ".1");
        public static readonly DerObjectIdentifier Prime192v2 = new DerObjectIdentifier(PrimeCurve + ".2");
        public static readonly DerObjectIdentifier Prime192v3 = new DerObjectIdentifier(PrimeCurve + ".3");
        public static readonly DerObjectIdentifier Prime239v1 = new DerObjectIdentifier(PrimeCurve + ".4");
        public static readonly DerObjectIdentifier Prime239v2 = new DerObjectIdentifier(PrimeCurve + ".5");
        public static readonly DerObjectIdentifier Prime239v3 = new DerObjectIdentifier(PrimeCurve + ".6");
        public static readonly DerObjectIdentifier Prime256v1 = new DerObjectIdentifier(PrimeCurve + ".7");

		//
        // Diffie-Hellman
        //
        // dhpublicnumber OBJECT IDENTIFIER ::= { iso(1) member-body(2)
        //            us(840) ansi-x942(10046) number-type(2) 1 }
        //
        public static readonly DerObjectIdentifier DHPublicNumber = new DerObjectIdentifier("1.2.840.10046.2.1");

		//
        // DSA
        //
        // dsapublicnumber OBJECT IDENTIFIER ::= { iso(1) member-body(2)
        //            us(840) ansi-x957(10040) number-type(4) 1 }
        public static readonly DerObjectIdentifier IdDsa = new DerObjectIdentifier("1.2.840.10040.4.1");

		/**
         *   id-dsa-with-sha1 OBJECT IDENTIFIER ::=  { iso(1) member-body(2)
         *         us(840) x9-57 (10040) x9cm(4) 3 }
         */
        public static readonly DerObjectIdentifier IdDsaWithSha1 = new DerObjectIdentifier("1.2.840.10040.4.3");

		/**
		 * X9.63
		 */
		public static readonly DerObjectIdentifier X9x63Scheme = new DerObjectIdentifier("1.3.133.16.840.63.0");
		public static readonly DerObjectIdentifier DHSinglePassStdDHSha1KdfScheme = new DerObjectIdentifier(X9x63Scheme + ".2");
		public static readonly DerObjectIdentifier DHSinglePassCofactorDHSha1KdfScheme = new DerObjectIdentifier(X9x63Scheme + ".3");
		public static readonly DerObjectIdentifier MqvSinglePassSha1KdfScheme = new DerObjectIdentifier(X9x63Scheme + ".16");

		/**
		 * X9.42
		 */
		public static readonly DerObjectIdentifier X9x42Schemes = new DerObjectIdentifier("1.2.840.10046.3");
		public static readonly DerObjectIdentifier DHStatic = new DerObjectIdentifier(X9x42Schemes + ".1");
		public static readonly DerObjectIdentifier DHEphem = new DerObjectIdentifier(X9x42Schemes + ".2");
		public static readonly DerObjectIdentifier DHOneFlow = new DerObjectIdentifier(X9x42Schemes + ".3");
		public static readonly DerObjectIdentifier DHHybrid1 = new DerObjectIdentifier(X9x42Schemes + ".4");
		public static readonly DerObjectIdentifier DHHybrid2 = new DerObjectIdentifier(X9x42Schemes + ".5");
		public static readonly DerObjectIdentifier DHHybridOneFlow = new DerObjectIdentifier(X9x42Schemes + ".6");
		public static readonly DerObjectIdentifier Mqv2 = new DerObjectIdentifier(X9x42Schemes + ".7");
		public static readonly DerObjectIdentifier Mqv1 = new DerObjectIdentifier(X9x42Schemes + ".8");
	}
}
