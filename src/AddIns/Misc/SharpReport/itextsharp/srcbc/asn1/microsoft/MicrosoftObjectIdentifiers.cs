using System;

namespace Org.BouncyCastle.Asn1.Microsoft
{
	public abstract class MicrosoftObjectIdentifiers
	{
		//
		// Microsoft
		//       iso(1) identified-organization(3) dod(6) internet(1) private(4) enterprise(1) Microsoft(311)
		//
		public static readonly DerObjectIdentifier Microsoft               = new DerObjectIdentifier("1.3.6.1.4.1.311");
		public static readonly DerObjectIdentifier MicrosoftCertTemplateV1 = new DerObjectIdentifier(Microsoft + ".20.2");
		public static readonly DerObjectIdentifier MicrosoftCAVersion      = new DerObjectIdentifier(Microsoft + ".21.1");
		public static readonly DerObjectIdentifier MicrosoftPrevCACertHash = new DerObjectIdentifier(Microsoft + ".21.2");
		public static readonly DerObjectIdentifier MicrosoftCertTemplateV2 = new DerObjectIdentifier(Microsoft + ".21.7");
		public static readonly DerObjectIdentifier MicrosoftAppPolicies    = new DerObjectIdentifier(Microsoft + ".21.10");
	}
}
