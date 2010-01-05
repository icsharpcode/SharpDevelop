using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public abstract class PkcsObjectIdentifiers
    {
        //
        // pkcs-1 OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) 1 }
        //
        public const string Pkcs1 = "1.2.840.113549.1.1";

		public static readonly DerObjectIdentifier RsaEncryption			= new DerObjectIdentifier(Pkcs1 + ".1");
        public static readonly DerObjectIdentifier MD2WithRsaEncryption		= new DerObjectIdentifier(Pkcs1 + ".2");
        public static readonly DerObjectIdentifier MD4WithRsaEncryption		= new DerObjectIdentifier(Pkcs1 + ".3");
        public static readonly DerObjectIdentifier MD5WithRsaEncryption		= new DerObjectIdentifier(Pkcs1 + ".4");
        public static readonly DerObjectIdentifier Sha1WithRsaEncryption	= new DerObjectIdentifier(Pkcs1 + ".5");
        public static readonly DerObjectIdentifier SrsaOaepEncryptionSet	= new DerObjectIdentifier(Pkcs1 + ".6");
        public static readonly DerObjectIdentifier IdRsaesOaep				= new DerObjectIdentifier(Pkcs1 + ".7");
        public static readonly DerObjectIdentifier IdMgf1					= new DerObjectIdentifier(Pkcs1 + ".8");
        public static readonly DerObjectIdentifier IdPSpecified				= new DerObjectIdentifier(Pkcs1 + ".9");
        public static readonly DerObjectIdentifier IdRsassaPss				= new DerObjectIdentifier(Pkcs1 + ".10");
        public static readonly DerObjectIdentifier Sha256WithRsaEncryption	= new DerObjectIdentifier(Pkcs1 + ".11");
        public static readonly DerObjectIdentifier Sha384WithRsaEncryption	= new DerObjectIdentifier(Pkcs1 + ".12");
        public static readonly DerObjectIdentifier Sha512WithRsaEncryption	= new DerObjectIdentifier(Pkcs1 + ".13");
        public static readonly DerObjectIdentifier Sha224WithRsaEncryption	= new DerObjectIdentifier(Pkcs1 + ".14");

		//
        // pkcs-3 OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) 3 }
        //
        public const string Pkcs3 = "1.2.840.113549.1.3";

		public static readonly DerObjectIdentifier    DhKeyAgreement          = new DerObjectIdentifier(Pkcs3 + ".1");

		//
        // pkcs-5 OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) 5 }
        //
        public const string Pkcs5 = "1.2.840.113549.1.5";

		public static readonly DerObjectIdentifier PbeWithMD2AndDesCbc    = new DerObjectIdentifier(Pkcs5 + ".1");
        public static readonly DerObjectIdentifier PbeWithMD2AndRC2Cbc    = new DerObjectIdentifier(Pkcs5 + ".4");
        public static readonly DerObjectIdentifier PbeWithMD5AndDesCbc    = new DerObjectIdentifier(Pkcs5 + ".3");
        public static readonly DerObjectIdentifier PbeWithMD5AndRC2Cbc    = new DerObjectIdentifier(Pkcs5 + ".6");
        public static readonly DerObjectIdentifier PbeWithSha1AndDesCbc   = new DerObjectIdentifier(Pkcs5 + ".10");
        public static readonly DerObjectIdentifier PbeWithSha1AndRC2Cbc   = new DerObjectIdentifier(Pkcs5 + ".11");

        public static readonly DerObjectIdentifier IdPbeS2 = new DerObjectIdentifier(Pkcs5 + ".13");
        public static readonly DerObjectIdentifier IdPbkdf2	= new DerObjectIdentifier(Pkcs5 + ".12");

		//
        // encryptionAlgorithm OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) 3 }
        //
        public const string EncryptionAlgorithm = "1.2.840.113549.3";

		public static readonly DerObjectIdentifier DesEde3Cbc	= new DerObjectIdentifier(EncryptionAlgorithm + ".7");
        public static readonly DerObjectIdentifier RC2Cbc		= new DerObjectIdentifier(EncryptionAlgorithm + ".2");

		//
        // object identifiers for digests
        //
        public const string DigestAlgorithm = "1.2.840.113549.2";

		//
        // md2 OBJECT IDENTIFIER ::=
        //      {iso(1) member-body(2) US(840) rsadsi(113549) DigestAlgorithm(2) 2}
        //
        public static readonly DerObjectIdentifier MD2 = new DerObjectIdentifier(DigestAlgorithm + ".2");

        //
        // md4 OBJECT IDENTIFIER ::=
        //      {iso(1) member-body(2) US(840) rsadsi(113549) DigestAlgorithm(2) 4}
        //
        public static readonly DerObjectIdentifier MD4 = new DerObjectIdentifier(DigestAlgorithm + ".4");

        //
        // md5 OBJECT IDENTIFIER ::=
        //      {iso(1) member-body(2) US(840) rsadsi(113549) DigestAlgorithm(2) 5}
        //
        public static readonly DerObjectIdentifier MD5 = new DerObjectIdentifier(DigestAlgorithm + ".5");

		public static readonly DerObjectIdentifier IdHmacWithSha1	= new DerObjectIdentifier(DigestAlgorithm + ".7");
        public static readonly DerObjectIdentifier IdHmacWithSha224	= new DerObjectIdentifier(DigestAlgorithm + ".8");
        public static readonly DerObjectIdentifier IdHmacWithSha256	= new DerObjectIdentifier(DigestAlgorithm + ".9");
        public static readonly DerObjectIdentifier IdHmacWithSha384	= new DerObjectIdentifier(DigestAlgorithm + ".10");
        public static readonly DerObjectIdentifier IdHmacWithSha512	= new DerObjectIdentifier(DigestAlgorithm + ".11");

		//
        // pkcs-7 OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) 7 }
        //
        public const string Pkcs7 = "1.2.840.113549.1.7";

		public static readonly DerObjectIdentifier Data                    = new DerObjectIdentifier(Pkcs7 + ".1");
        public static readonly DerObjectIdentifier SignedData              = new DerObjectIdentifier(Pkcs7 + ".2");
        public static readonly DerObjectIdentifier EnvelopedData           = new DerObjectIdentifier(Pkcs7 + ".3");
        public static readonly DerObjectIdentifier SignedAndEnvelopedData  = new DerObjectIdentifier(Pkcs7 + ".4");
        public static readonly DerObjectIdentifier DigestedData            = new DerObjectIdentifier(Pkcs7 + ".5");
        public static readonly DerObjectIdentifier EncryptedData           = new DerObjectIdentifier(Pkcs7 + ".6");

        //
        // pkcs-9 OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) 9 }
        //
        public const string Pkcs9 = "1.2.840.113549.1.9";

		public static readonly DerObjectIdentifier Pkcs9AtEmailAddress					= new DerObjectIdentifier(Pkcs9 + ".1");
        public static readonly DerObjectIdentifier Pkcs9AtUnstructuredName				= new DerObjectIdentifier(Pkcs9 + ".2");
        public static readonly DerObjectIdentifier Pkcs9AtContentType					= new DerObjectIdentifier(Pkcs9 + ".3");
        public static readonly DerObjectIdentifier Pkcs9AtMessageDigest					= new DerObjectIdentifier(Pkcs9 + ".4");
        public static readonly DerObjectIdentifier Pkcs9AtSigningTime					= new DerObjectIdentifier(Pkcs9 + ".5");
        public static readonly DerObjectIdentifier Pkcs9AtCounterSignature				= new DerObjectIdentifier(Pkcs9 + ".6");
        public static readonly DerObjectIdentifier Pkcs9AtChallengePassword				= new DerObjectIdentifier(Pkcs9 + ".7");
        public static readonly DerObjectIdentifier Pkcs9AtUnstructuredAddress			= new DerObjectIdentifier(Pkcs9 + ".8");
        public static readonly DerObjectIdentifier Pkcs9AtExtendedCertificateAttributes	= new DerObjectIdentifier(Pkcs9 + ".9");
        public static readonly DerObjectIdentifier Pkcs9AtSigningDescription			= new DerObjectIdentifier(Pkcs9 + ".13");
        public static readonly DerObjectIdentifier Pkcs9AtExtensionRequest				= new DerObjectIdentifier(Pkcs9 + ".14");
        public static readonly DerObjectIdentifier Pkcs9AtSmimeCapabilities				= new DerObjectIdentifier(Pkcs9 + ".15");
        public static readonly DerObjectIdentifier Pkcs9AtFriendlyName					= new DerObjectIdentifier(Pkcs9 + ".20");
        public static readonly DerObjectIdentifier Pkcs9AtLocalKeyID					= new DerObjectIdentifier(Pkcs9 + ".21");

		[Obsolete("Use X509Certificate instead")]
        public static readonly DerObjectIdentifier X509CertType = new DerObjectIdentifier(Pkcs9 + ".22.1");

		public const string CertTypes = Pkcs9 + ".22";
		public static readonly DerObjectIdentifier X509Certificate = new DerObjectIdentifier(CertTypes + ".1");
		public static readonly DerObjectIdentifier SdsiCertificate = new DerObjectIdentifier(CertTypes + ".2");

		public const string CrlTypes = Pkcs9 + ".23";
		public static readonly DerObjectIdentifier X509Crl = new DerObjectIdentifier(CrlTypes + ".1");

		public static readonly DerObjectIdentifier IdAlgPwriKek = new DerObjectIdentifier(Pkcs9 + ".16.3.9");

        //
        // SMIME capability sub oids.
        //
        public static readonly DerObjectIdentifier PreferSignedData				= new DerObjectIdentifier(Pkcs9 + ".15.1");
        public static readonly DerObjectIdentifier CannotDecryptAny				= new DerObjectIdentifier(Pkcs9 + ".15.2");
        public static readonly DerObjectIdentifier SmimeCapabilitiesVersions	= new DerObjectIdentifier(Pkcs9 + ".15.3");

        //
        // other SMIME attributes
        //
		public static readonly DerObjectIdentifier IdAAReceiptRequest = new DerObjectIdentifier(Pkcs9 + ".16.2.1");

        //
        // id-ct OBJECT IDENTIFIER ::= {iso(1) member-body(2) usa(840)
        // rsadsi(113549) pkcs(1) pkcs-9(9) smime(16) ct(1)}
        //
        public const string IdCT = "1.2.840.113549.1.9.16.1";

        public static readonly DerObjectIdentifier IdCTTstInfo           = new DerObjectIdentifier(IdCT + ".4");
        public static readonly DerObjectIdentifier IdCTCompressedData    = new DerObjectIdentifier(IdCT + ".9");

        //
        // id-cti OBJECT IDENTIFIER ::= {iso(1) member-body(2) usa(840)
        // rsadsi(113549) pkcs(1) pkcs-9(9) smime(16) cti(6)}
        //
        public const string IdCti = "1.2.840.113549.1.9.16.6";

        public static readonly DerObjectIdentifier IdCtiEtsProofOfOrigin	= new DerObjectIdentifier(IdCti + ".1");
        public static readonly DerObjectIdentifier IdCtiEtsProofOfReceipt	= new DerObjectIdentifier(IdCti + ".2");
        public static readonly DerObjectIdentifier IdCtiEtsProofOfDelivery	= new DerObjectIdentifier(IdCti + ".3");
        public static readonly DerObjectIdentifier IdCtiEtsProofOfSender	= new DerObjectIdentifier(IdCti + ".4");
        public static readonly DerObjectIdentifier IdCtiEtsProofOfApproval	= new DerObjectIdentifier(IdCti + ".5");
        public static readonly DerObjectIdentifier IdCtiEtsProofOfCreation	= new DerObjectIdentifier(IdCti + ".6");

        //
        // id-aa OBJECT IDENTIFIER ::= {iso(1) member-body(2) usa(840)
        // rsadsi(113549) pkcs(1) pkcs-9(9) smime(16) attributes(2)}
        //
        public const string IdAA = "1.2.840.113549.1.9.16.2";

		public static readonly DerObjectIdentifier IdAAContentHint = new DerObjectIdentifier(IdAA + ".4"); // See RFC 2634

		/*
        * id-aa-encrypKeyPref OBJECT IDENTIFIER ::= {id-aa 11}
        *
        */
        public static readonly DerObjectIdentifier IdAAEncrypKeyPref = new DerObjectIdentifier(IdAA + ".11");
        public static readonly DerObjectIdentifier IdAASigningCertificate = new DerObjectIdentifier(IdAA + ".12");
		public static readonly DerObjectIdentifier IdAASigningCertificateV2 = new DerObjectIdentifier(IdAA + ".47");

		public static readonly DerObjectIdentifier IdAAContentIdentifier = new DerObjectIdentifier(IdAA + ".7"); // See RFC 2634

		/*
		 * RFC 3126
		 */
		public static readonly DerObjectIdentifier IdAASignatureTimeStampToken = new DerObjectIdentifier(IdAA + ".14");

		public static readonly DerObjectIdentifier IdAAEtsSigPolicyID = new DerObjectIdentifier(IdAA + ".15");
		public static readonly DerObjectIdentifier IdAAEtsCommitmentType = new DerObjectIdentifier(IdAA + ".16");
		public static readonly DerObjectIdentifier IdAAEtsSignerLocation = new DerObjectIdentifier(IdAA + ".17");
		public static readonly DerObjectIdentifier IdAAEtsSignerAttr = new DerObjectIdentifier(IdAA + ".18");
		public static readonly DerObjectIdentifier IdAAEtsOtherSigCert = new DerObjectIdentifier(IdAA + ".19");
		public static readonly DerObjectIdentifier IdAAEtsContentTimestamp = new DerObjectIdentifier(IdAA + ".20");
		public static readonly DerObjectIdentifier IdAAEtsCertificateRefs = new DerObjectIdentifier(IdAA + ".21");
		public static readonly DerObjectIdentifier IdAAEtsRevocationRefs = new DerObjectIdentifier(IdAA + ".22");
		public static readonly DerObjectIdentifier IdAAEtsCertValues = new DerObjectIdentifier(IdAA + ".23");
		public static readonly DerObjectIdentifier IdAAEtsRevocationValues = new DerObjectIdentifier(IdAA + ".24");
		public static readonly DerObjectIdentifier IdAAEtsEscTimeStamp = new DerObjectIdentifier(IdAA + ".25");
		public static readonly DerObjectIdentifier IdAAEtsCertCrlTimestamp = new DerObjectIdentifier(IdAA + ".26");
		public static readonly DerObjectIdentifier IdAAEtsArchiveTimestamp = new DerObjectIdentifier(IdAA + ".27");

		[Obsolete("Use 'IdAAEtsSigPolicyID' instead")]
		public static readonly DerObjectIdentifier IdAASigPolicyID = IdAAEtsSigPolicyID;
		[Obsolete("Use 'IdAAEtsCommitmentType' instead")]
		public static readonly DerObjectIdentifier IdAACommitmentType = IdAAEtsCommitmentType;
		[Obsolete("Use 'IdAAEtsSignerLocation' instead")]
		public static readonly DerObjectIdentifier IdAASignerLocation = IdAAEtsSignerLocation;
		[Obsolete("Use 'IdAAEtsOtherSigCert' instead")]
		public static readonly DerObjectIdentifier IdAAOtherSigCert = IdAAEtsOtherSigCert;

		//
		// id-spq OBJECT IDENTIFIER ::= {iso(1) member-body(2) usa(840)
		// rsadsi(113549) pkcs(1) pkcs-9(9) smime(16) id-spq(5)}
		//
		public const string IdSpq = "1.2.840.113549.1.9.16.5";

		public static readonly DerObjectIdentifier IdSpqEtsUri = new DerObjectIdentifier(IdSpq + ".1");
		public static readonly DerObjectIdentifier IdSpqEtsUNotice = new DerObjectIdentifier(IdSpq + ".2");

		//
        // pkcs-12 OBJECT IDENTIFIER ::= {
        //       iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) 12 }
        //
        public const string Pkcs12 = "1.2.840.113549.1.12";
        public const string BagTypes = Pkcs12 + ".10.1";

        public static readonly DerObjectIdentifier KeyBag				= new DerObjectIdentifier(BagTypes + ".1");
        public static readonly DerObjectIdentifier Pkcs8ShroudedKeyBag	= new DerObjectIdentifier(BagTypes + ".2");
        public static readonly DerObjectIdentifier CertBag				= new DerObjectIdentifier(BagTypes + ".3");
        public static readonly DerObjectIdentifier CrlBag				= new DerObjectIdentifier(BagTypes + ".4");
        public static readonly DerObjectIdentifier SecretBag			= new DerObjectIdentifier(BagTypes + ".5");
        public static readonly DerObjectIdentifier SafeContentsBag		= new DerObjectIdentifier(BagTypes + ".6");

        public const string Pkcs12PbeIds = Pkcs12 + ".1";

        public static readonly DerObjectIdentifier PbeWithShaAnd128BitRC4			= new DerObjectIdentifier(Pkcs12PbeIds + ".1");
        public static readonly DerObjectIdentifier PbeWithShaAnd40BitRC4			= new DerObjectIdentifier(Pkcs12PbeIds + ".2");
        public static readonly DerObjectIdentifier PbeWithShaAnd3KeyTripleDesCbc	= new DerObjectIdentifier(Pkcs12PbeIds + ".3");
        public static readonly DerObjectIdentifier PbeWithShaAnd2KeyTripleDesCbc	= new DerObjectIdentifier(Pkcs12PbeIds + ".4");
        public static readonly DerObjectIdentifier PbeWithShaAnd128BitRC2Cbc		= new DerObjectIdentifier(Pkcs12PbeIds + ".5");
        public static readonly DerObjectIdentifier PbewithShaAnd40BitRC2Cbc			= new DerObjectIdentifier(Pkcs12PbeIds + ".6");

		public static readonly DerObjectIdentifier IdAlgCms3DesWrap = new DerObjectIdentifier("1.2.840.113549.1.9.16.3.6");
		public static readonly DerObjectIdentifier IdAlgCmsRC2Wrap = new DerObjectIdentifier("1.2.840.113549.1.9.16.3.7");
	}
}
