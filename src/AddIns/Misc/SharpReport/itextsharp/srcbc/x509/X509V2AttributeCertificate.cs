using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;

namespace Org.BouncyCastle.X509
{
	/// <summary>An implementation of a version 2 X.509 Attribute Certificate.</summary>
	public class X509V2AttributeCertificate
		: X509ExtensionBase, IX509AttributeCertificate
	{
		private readonly AttributeCertificate cert;
		private readonly DateTime notBefore;
		private readonly DateTime notAfter;

		public X509V2AttributeCertificate(
			Stream encIn)
			: this(new Asn1InputStream(encIn))
		{
		}

		public X509V2AttributeCertificate(
			byte[] encoded)
			: this(new Asn1InputStream(encoded))
		{
		}

		internal X509V2AttributeCertificate(
			Asn1InputStream ais)
			: this(AttributeCertificate.GetInstance(ais.ReadObject()))
		{
		}

		internal X509V2AttributeCertificate(
			AttributeCertificate cert)
		{
			this.cert = cert;

			try
			{
				this.notAfter = cert.ACInfo.AttrCertValidityPeriod.NotAfterTime.ToDateTime();
				this.notBefore = cert.ACInfo.AttrCertValidityPeriod.NotBeforeTime.ToDateTime();
			}
			catch (Exception e)
			{
				throw new IOException("invalid data structure in certificate!", e);
			}
		}

		public virtual int Version
		{
			get { return cert.ACInfo.Version.Value.IntValue; }
		}

		public virtual BigInteger SerialNumber
		{
			get { return cert.ACInfo.SerialNumber.Value; }
		}

		public virtual AttributeCertificateHolder Holder
		{
			get
			{
				return new AttributeCertificateHolder((Asn1Sequence)cert.ACInfo.Holder.ToAsn1Object());
			}
		}

		public virtual AttributeCertificateIssuer Issuer
		{
			get
			{
				return new AttributeCertificateIssuer(cert.ACInfo.Issuer);
			}
		}

		public virtual DateTime NotBefore
		{
			get { return notBefore; }
		}

		public virtual DateTime NotAfter
		{
			get { return notAfter; }
		}

		public virtual bool[] GetIssuerUniqueID()
		{
			DerBitString id = cert.ACInfo.IssuerUniqueID;

			if (id != null)
			{
				byte[] bytes = id.GetBytes();
				bool[] boolId = new bool[bytes.Length * 8 - id.PadBits];

				for (int i = 0; i != boolId.Length; i++)
				{
					//boolId[i] = (bytes[i / 8] & (0x80 >>> (i % 8))) != 0;
					boolId[i] = (bytes[i / 8] & (0x80 >> (i % 8))) != 0;
				}

				return boolId;
			}

			return null;
		}

		public virtual bool IsValidNow
		{
			get { return IsValid(DateTime.UtcNow); }
		}

		public virtual bool IsValid(
			DateTime date)
		{
			return date.CompareTo(NotBefore) >= 0 && date.CompareTo(NotAfter) <= 0;
		}

		public virtual void CheckValidity()
		{
			this.CheckValidity(DateTime.UtcNow);
		}

		public virtual void CheckValidity(
			DateTime date)
		{
			if (date.CompareTo(NotAfter) > 0)
				throw new CertificateExpiredException("certificate expired on " + NotAfter);
			if (date.CompareTo(NotBefore) < 0)
				throw new CertificateNotYetValidException("certificate not valid until " + NotBefore);
		}

		public virtual byte[] GetSignature()
		{
			return cert.SignatureValue.GetBytes();
		}

		public virtual void Verify(
			AsymmetricKeyParameter publicKey)
		{
			if (!cert.SignatureAlgorithm.Equals(cert.ACInfo.Signature))
			{
				throw new CertificateException("Signature algorithm in certificate info not same as outer certificate");
			}

			ISigner signature = SignerUtilities.GetSigner(cert.SignatureAlgorithm.ObjectID.Id);

			signature.Init(false, publicKey);

			try
			{
				byte[] b = cert.ACInfo.GetEncoded();
				signature.BlockUpdate(b, 0, b.Length);
			}
			catch (IOException e)
			{
				throw new SignatureException("Exception encoding certificate info object", e);
			}

			if (!signature.VerifySignature(this.GetSignature()))
			{
				throw new InvalidKeyException("Public key presented not for certificate signature");
			}
		}

		public virtual byte[] GetEncoded()
		{
			return cert.GetEncoded();
		}

		protected override X509Extensions GetX509Extensions()
		{
			return cert.ACInfo.Extensions;
		}

		public virtual X509Attribute[] GetAttributes()
		{
			Asn1Sequence seq = cert.ACInfo.Attributes;
			X509Attribute[] attrs = new X509Attribute[seq.Count];

			for (int i = 0; i != seq.Count; i++)
			{
				attrs[i] = new X509Attribute((Asn1Encodable)seq[i]);
			}

			return attrs;
		}

		public virtual X509Attribute[] GetAttributes(
			string oid)
		{
			Asn1Sequence seq = cert.ACInfo.Attributes;
			ArrayList list = new ArrayList();

			for (int i = 0; i != seq.Count; i++)
			{
				X509Attribute attr = new X509Attribute((Asn1Encodable)seq[i]);
				if (attr.Oid.Equals(oid))
				{
					list.Add(attr);
				}
			}

			if (list.Count < 1)
			{
				return null;
			}

			return (X509Attribute[]) list.ToArray(typeof(X509Attribute));
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			X509V2AttributeCertificate other = obj as X509V2AttributeCertificate;

			if (other == null)
				return false;

			return cert.Equals(other.cert);

			// NB: May prefer this implementation of Equals if more than one certificate implementation in play
			//return Arrays.AreEqual(this.GetEncoded(), other.GetEncoded());
		}

		public override int GetHashCode()
		{
			return cert.GetHashCode();
		}
	}
}
