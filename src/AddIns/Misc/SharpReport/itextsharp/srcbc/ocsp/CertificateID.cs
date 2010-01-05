using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Ocsp
{
	public class CertificateID
	{
		public const string HashSha1 = "1.3.14.3.2.26";

		private readonly CertID id;

		public CertificateID(
			CertID id)
		{
			this.id = id;
		}

		/**
		 * create from an issuer certificate and the serial number of the
		 * certificate it signed.
		 * @exception OcspException if any problems occur creating the id fields.
		 */
		public CertificateID(
			string			hashAlgorithm,
			X509Certificate	issuerCert,
			BigInteger		number)
		{
			try
			{
				IDigest digest = DigestUtilities.GetDigest(hashAlgorithm);
				AlgorithmIdentifier hashAlg = new AlgorithmIdentifier(
					new DerObjectIdentifier(hashAlgorithm), DerNull.Instance);

				X509Name issuerName = PrincipalUtilities.GetSubjectX509Principal(issuerCert);

				byte[] encodedIssuerName = issuerName.GetEncoded();
				digest.BlockUpdate(encodedIssuerName, 0, encodedIssuerName.Length);

				byte[] hash = DigestUtilities.DoFinal(digest);

				Asn1OctetString issuerNameHash = new DerOctetString(hash);
				AsymmetricKeyParameter issuerKey = issuerCert.GetPublicKey();

				SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKey);

				byte[] encodedPublicKey = info.PublicKeyData.GetBytes();
				digest.BlockUpdate(encodedPublicKey, 0, encodedPublicKey.Length);

				hash = DigestUtilities.DoFinal(digest);

				Asn1OctetString issuerKeyHash = new DerOctetString(hash);

				DerInteger serialNumber = new DerInteger(number);

				this.id = new CertID(hashAlg, issuerNameHash, issuerKeyHash, serialNumber);
			}
			catch (Exception e)
			{
				throw new OcspException("problem creating ID: " + e, e);
			}
		}

		public string HashAlgOid
		{
			get { return id.HashAlgorithm.ObjectID.Id; }
		}

		public byte[] GetIssuerNameHash()
		{
			return id.IssuerNameHash.GetOctets();
		}

		public byte[] GetIssuerKeyHash()
		{
			return id.IssuerKeyHash.GetOctets();
		}

		/**
		 * return the serial number for the certificate associated
		 * with this request.
		 */
		public BigInteger SerialNumber
		{
			get { return id.SerialNumber.Value; }
		}

		public CertID ToAsn1Object()
		{
			return id;
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			CertificateID other = obj as CertificateID;

			if (other == null)
				return false;

			return id.ToAsn1Object().Equals(other.id.ToAsn1Object());
		}

		public override int GetHashCode()
		{
			return id.ToAsn1Object().GetHashCode();
		}
	}
}
