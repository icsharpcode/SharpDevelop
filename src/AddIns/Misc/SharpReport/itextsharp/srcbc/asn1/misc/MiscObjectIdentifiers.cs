using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Misc
{
    public abstract class MiscObjectIdentifiers
    {
        //
        // Netscape
        //       iso/itu(2) joint-assign(16) us(840) uscompany(1) Netscape(113730) cert-extensions(1) }
        //
        public static readonly DerObjectIdentifier Netscape                = new DerObjectIdentifier("2.16.840.1.113730.1");
        public static readonly DerObjectIdentifier NetscapeCertType        = new DerObjectIdentifier(Netscape + ".1");
        public static readonly DerObjectIdentifier NetscapeBaseUrl         = new DerObjectIdentifier(Netscape + ".2");
        public static readonly DerObjectIdentifier NetscapeRevocationUrl   = new DerObjectIdentifier(Netscape + ".3");
        public static readonly DerObjectIdentifier NetscapeCARevocationUrl = new DerObjectIdentifier(Netscape + ".4");
        public static readonly DerObjectIdentifier NetscapeRenewalUrl      = new DerObjectIdentifier(Netscape + ".7");
        public static readonly DerObjectIdentifier NetscapeCAPolicyUrl     = new DerObjectIdentifier(Netscape + ".8");
        public static readonly DerObjectIdentifier NetscapeSslServerName   = new DerObjectIdentifier(Netscape + ".12");
        public static readonly DerObjectIdentifier NetscapeCertComment     = new DerObjectIdentifier(Netscape + ".13");
        //
        // Verisign
        //       iso/itu(2) joint-assign(16) us(840) uscompany(1) verisign(113733) cert-extensions(1) }
        //
        internal const string Verisign = "2.16.840.1.113733.1";

		//
        // CZAG - country, zip, age, and gender
        //
        public static readonly DerObjectIdentifier VerisignCzagExtension = new DerObjectIdentifier(Verisign + ".6.3");

		// D&B D-U-N-S number
		public static readonly DerObjectIdentifier VerisignDnbDunsNumber = new DerObjectIdentifier(Verisign + ".6.15");

		//
		// Novell
		//       iso/itu(2) country(16) us(840) organization(1) novell(113719)
		//
		public static readonly string				Novell					= "2.16.840.1.113719";
		public static readonly DerObjectIdentifier	NovellSecurityAttribs	= new DerObjectIdentifier(Novell + ".1.9.4.1");

		//
		// Entrust
		//       iso(1) member-body(16) us(840) nortelnetworks(113533) entrust(7)
		//
		public static readonly string				Entrust					= "1.2.840.113533.7";
		public static readonly DerObjectIdentifier	EntrustVersionExtension = new DerObjectIdentifier(Entrust + ".65.0");
	}
}
