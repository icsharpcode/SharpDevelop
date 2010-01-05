using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tsp
{
	public class TspUtil
	{
		private static readonly IDictionary digestLengths = new Hashtable();
		private static readonly IDictionary digestNames = new Hashtable();

		static TspUtil()
		{
			digestLengths.Add(PkcsObjectIdentifiers.MD5.Id, 16);
			digestLengths.Add(OiwObjectIdentifiers.IdSha1.Id, 20);
			digestLengths.Add(NistObjectIdentifiers.IdSha224.Id, 28);
			digestLengths.Add(NistObjectIdentifiers.IdSha256.Id, 32);
			digestLengths.Add(NistObjectIdentifiers.IdSha384.Id, 48);
			digestLengths.Add(NistObjectIdentifiers.IdSha512.Id, 64);

			digestNames.Add(PkcsObjectIdentifiers.MD5.Id, "MD5");
			digestNames.Add(OiwObjectIdentifiers.IdSha1.Id, "SHA1");
			digestNames.Add(NistObjectIdentifiers.IdSha224.Id, "SHA224");
			digestNames.Add(NistObjectIdentifiers.IdSha256.Id, "SHA256");
			digestNames.Add(NistObjectIdentifiers.IdSha384.Id, "SHA384");
			digestNames.Add(NistObjectIdentifiers.IdSha512.Id, "SHA512");
			digestNames.Add(PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id, "SHA1");
			digestNames.Add(PkcsObjectIdentifiers.Sha224WithRsaEncryption.Id, "SHA224");
			digestNames.Add(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, "SHA256");
			digestNames.Add(PkcsObjectIdentifiers.Sha384WithRsaEncryption.Id, "SHA384");
			digestNames.Add(PkcsObjectIdentifiers.Sha512WithRsaEncryption.Id, "SHA512");
			digestNames.Add(TeleTrusTObjectIdentifiers.RipeMD128.Id, "RIPEMD128");
			digestNames.Add(TeleTrusTObjectIdentifiers.RipeMD160.Id, "RIPEMD160");
			digestNames.Add(TeleTrusTObjectIdentifiers.RipeMD256.Id, "RIPEMD256");
			digestNames.Add(CryptoProObjectIdentifiers.GostR3411.Id, "GOST3411");
		}

		/**
		 * Validate the passed in certificate as being of the correct type to be used
		 * for time stamping. To be valid it must have an ExtendedKeyUsage extension
		 * which has a key purpose identifier of id-kp-timeStamping.
		 *
		 * @param cert the certificate of interest.
		 * @throws TspValidationException if the certicate fails on one of the check points.
		 */
		public static void ValidateCertificate(
			X509Certificate cert)
		{
			if (cert.Version != 3)
				throw new ArgumentException("Certificate must have an ExtendedKeyUsage extension.");

			Asn1OctetString ext = cert.GetExtensionValue(X509Extensions.ExtendedKeyUsage);
			if (ext == null)
				throw new TspValidationException("Certificate must have an ExtendedKeyUsage extension.");

			if (!cert.GetCriticalExtensionOids().Contains(X509Extensions.ExtendedKeyUsage.Id))
				throw new TspValidationException("Certificate must have an ExtendedKeyUsage extension marked as critical.");

			try
			{
				ExtendedKeyUsage extKey = ExtendedKeyUsage.GetInstance(
					Asn1Object.FromByteArray(ext.GetOctets()));

				if (!extKey.HasKeyPurposeId(KeyPurposeID.IdKPTimeStamping) || extKey.Count != 1)
					throw new TspValidationException("ExtendedKeyUsage not solely time stamping.");
			}
			catch (IOException)
			{
				throw new TspValidationException("cannot process ExtendedKeyUsage extension");
			}
		}

		/// <summary>
		/// Return the digest algorithm using one of the standard JCA string
		/// representations rather than the algorithm identifier (if possible).
		/// </summary>
		internal static string GetDigestAlgName(
			string digestAlgOID)
		{
			string digestName = (string) digestNames[digestAlgOID];

			return digestName != null ? digestName : digestAlgOID;
		}

		internal static int GetDigestLength(
			string digestAlgOID)
		{
			string digestName = GetDigestAlgName(digestAlgOID);

			try
			{
				if (digestLengths.Contains(digestAlgOID))
				{
					return (int) digestLengths[digestAlgOID];
				}

				return DigestUtilities.GetDigest(digestName).GetDigestSize();
			}
			catch (SecurityUtilityException e)
			{
				throw new TspException("digest algorithm cannot be found.", e);
			}
		}
	}
}
