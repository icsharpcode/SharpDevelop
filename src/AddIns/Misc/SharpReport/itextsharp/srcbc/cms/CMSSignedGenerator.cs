using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms
{
    public class CmsSignedGenerator
    {
        /**
        * Default type for the signed data.
        */
        public static readonly string Data = PkcsObjectIdentifiers.Data.Id;

		public static readonly string DigestSha1 = OiwObjectIdentifiers.IdSha1.Id;
        public static readonly string DigestSha224 = NistObjectIdentifiers.IdSha224.Id;
        public static readonly string DigestSha256 = NistObjectIdentifiers.IdSha256.Id;
        public static readonly string DigestSha384 = NistObjectIdentifiers.IdSha384.Id;
        public static readonly string DigestSha512 = NistObjectIdentifiers.IdSha512.Id;
        public static readonly string DigestMD5 = PkcsObjectIdentifiers.MD5.Id;
        public static readonly string DigestGost3411 = CryptoProObjectIdentifiers.GostR3411.Id;
		public static readonly string DigestRipeMD128 = TeleTrusTObjectIdentifiers.RipeMD128.Id;
		public static readonly string DigestRipeMD160 = TeleTrusTObjectIdentifiers.RipeMD160.Id;
		public static readonly string DigestRipeMD256 = TeleTrusTObjectIdentifiers.RipeMD256.Id;

		public static readonly string EncryptionRsa = PkcsObjectIdentifiers.RsaEncryption.Id;
        public static readonly string EncryptionDsa = X9ObjectIdentifiers.IdDsaWithSha1.Id;
        public static readonly string EncryptionECDsa = X9ObjectIdentifiers.ECDsaWithSha1.Id;
        public static readonly string EncryptionRsaPss = PkcsObjectIdentifiers.IdRsassaPss.Id;
        public static readonly string EncryptionGost3410 = CryptoProObjectIdentifiers.GostR3410x94.Id;
        public static readonly string EncryptionECGost3410 = CryptoProObjectIdentifiers.GostR3410x2001.Id;

		private static readonly string EncryptionECDsaWithSha1 = X9ObjectIdentifiers.ECDsaWithSha1.Id;
		private static readonly string EncryptionECDsaWithSha224 = X9ObjectIdentifiers.ECDsaWithSha224.Id;
		private static readonly string EncryptionECDsaWithSha256 = X9ObjectIdentifiers.ECDsaWithSha256.Id;
		private static readonly string EncryptionECDsaWithSha384 = X9ObjectIdentifiers.ECDsaWithSha384.Id;
		private static readonly string EncryptionECDsaWithSha512 = X9ObjectIdentifiers.ECDsaWithSha512.Id;

		private static readonly ISet noParams = new HashSet();
		private static readonly Hashtable ecAlgorithms = new Hashtable();

		static CmsSignedGenerator()
		{
			noParams.Add(EncryptionDsa);
//			noParams.Add(EncryptionECDsa);
			noParams.Add(EncryptionECDsaWithSha1);
			noParams.Add(EncryptionECDsaWithSha224);
			noParams.Add(EncryptionECDsaWithSha256);
			noParams.Add(EncryptionECDsaWithSha384);
			noParams.Add(EncryptionECDsaWithSha512);

			ecAlgorithms.Add(DigestSha1, EncryptionECDsaWithSha1);
			ecAlgorithms.Add(DigestSha224, EncryptionECDsaWithSha224);
			ecAlgorithms.Add(DigestSha256, EncryptionECDsaWithSha256);
			ecAlgorithms.Add(DigestSha384, EncryptionECDsaWithSha384);
			ecAlgorithms.Add(DigestSha512, EncryptionECDsaWithSha512);
		}

		internal ArrayList _certs = new ArrayList();
		internal ArrayList _crls = new ArrayList();
		internal ArrayList _signers = new ArrayList();
		internal IDictionary _digests = new Hashtable();

		protected readonly SecureRandom rand;

		protected CmsSignedGenerator()
			: this(new SecureRandom())
		{
		}

		/// <summary>Constructor allowing specific source of randomness</summary>
		/// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
		protected CmsSignedGenerator(
			SecureRandom rand)
		{
			this.rand = rand;
		}

		protected string GetEncOid(
            AsymmetricKeyParameter	key,
            string					digestOID)
        {
            string encOID = null;

			if (key is RsaKeyParameters)
			{
				if (!((RsaKeyParameters) key).IsPrivate)
					throw new ArgumentException("Expected RSA private key");

				encOID = EncryptionRsa;
			}
			else if (key is DsaPrivateKeyParameters)
			{
				if (!digestOID.Equals(DigestSha1))
					throw new ArgumentException("can't mix DSA with anything but SHA1");

				encOID = EncryptionDsa;
			}
			else if (key is ECPrivateKeyParameters)
			{
				ECPrivateKeyParameters ecPrivKey = (ECPrivateKeyParameters) key;
				string algName = ecPrivKey.AlgorithmName;

				if (algName == "ECGOST3410")
				{
					encOID = EncryptionECGost3410;
				}
				else
				{
					// TODO Should we insist on algName being one of "EC" or "ECDSA", as Java does?
					encOID = (string) ecAlgorithms[digestOID];

					if (encOID == null)
						throw new ArgumentException("can't mix ECDSA with anything but SHA family digests");
				}
			}
			else if (key is Gost3410PrivateKeyParameters)
			{
				encOID = EncryptionGost3410;
			}
			else
			{
				throw new ArgumentException("Unknown algorithm in CmsSignedGenerator.GetEncOid");
			}

			return encOID;
        }

		internal static AlgorithmIdentifier GetEncAlgorithmIdentifier(
			string encOid)
		{
			if (noParams.Contains(encOid))
			{
				return new AlgorithmIdentifier(new DerObjectIdentifier(encOid));
			}

			return new AlgorithmIdentifier(new DerObjectIdentifier(encOid), DerNull.Instance);
		}

		internal protected virtual IDictionary GetBaseParameters(
			DerObjectIdentifier	contentType,
			AlgorithmIdentifier	digAlgId,
			byte[]				hash)
		{
			IDictionary param = new Hashtable();

			param[CmsAttributeTableParameter.ContentType] = contentType;
			param[CmsAttributeTableParameter.DigestAlgorithmIdentifier] = digAlgId;

			if (hash != null)
			{
				param[CmsAttributeTableParameter.Digest] = hash.Clone();
			}

			return param;
		}

		internal protected virtual Asn1Set GetAttributeSet(
            Asn1.Cms.AttributeTable attr)
        {
			return attr == null
				?	null
				:	new DerSet(attr.ToAsn1EncodableVector());
        }

		public void AddCertificates(
			IX509Store certStore)
		{
			_certs.AddRange(CmsUtilities.GetCertificatesFromStore(certStore));
		}

		public void AddCrls(
			IX509Store crlStore)
		{
			_crls.AddRange(CmsUtilities.GetCrlsFromStore(crlStore));
		}

		/**
		* Add the attribute certificates contained in the passed in store to the
		* generator.
		*
		* @param store a store of Version 2 attribute certificates
		* @throws CmsException if an error occurse processing the store.
		*/
		public void AddAttributeCertificates(
			IX509Store store)
		{
			try
			{
				foreach (IX509AttributeCertificate attrCert in store.GetMatches(null))
				{
					_certs.Add(new DerTaggedObject(false, 2,
						AttributeCertificate.GetInstance(Asn1Object.FromByteArray(attrCert.GetEncoded()))));
				}
			}
			catch (Exception e)
			{
				throw new CmsException("error processing attribute certs", e);
			}
		}

		/**
		 * Add a store of precalculated signers to the generator.
		 *
		 * @param signerStore store of signers
		 */
		public void AddSigners(
			SignerInformationStore signerStore)
		{
			foreach (object o in signerStore.GetSigners())
			{
				_signers.Add(o);
			}
		}

		/**
		 * Return a map of oids and byte arrays representing the digests calculated on the content during
		 * the last generate.
		 *
		 * @return a map of oids (as String objects) and byte[] representing digests.
		 */
		public IDictionary GetGeneratedDigests()
		{
			return new Hashtable(_digests);
		}
	}
}
