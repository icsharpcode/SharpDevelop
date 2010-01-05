using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.OpenSsl
{
	/// <remarks>General purpose writer for OpenSSL PEM objects.</remarks>
	public class PemWriter
	{
		private readonly TextWriter writer;

		public TextWriter Writer
		{
			get { return writer; }
		}

		/// <param name="writer">The TextWriter object to write the output to.</param>
		public PemWriter(
			TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			this.writer = writer;
		}

		public void WriteObject(
			object obj) 
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			string type;
			byte[] encoding;
	        
			if (obj is X509Certificate)
			{
				// TODO Should we prefer "X509 CERTIFICATE" here?
				type = "CERTIFICATE";
				try
				{
					encoding = ((X509Certificate)obj).GetEncoded();
				}
				catch (CertificateEncodingException e)
				{
					throw new IOException("Cannot Encode object: " + e.ToString());
				}
			}
			else if (obj is X509Crl)
			{
				type = "X509 CRL";
				try
				{
					encoding = ((X509Crl)obj).GetEncoded();
				}
				catch (CrlException e)
				{
					throw new IOException("Cannot Encode object: " + e.ToString());
				}
			}
			else if (obj is AsymmetricCipherKeyPair)
			{
				WriteObject(((AsymmetricCipherKeyPair)obj).Private);
				return;
			}
			else if (obj is AsymmetricKeyParameter)
			{
				AsymmetricKeyParameter akp = (AsymmetricKeyParameter) obj;
				if (akp.IsPrivate)
				{
					string keyType;
					encoding = EncodePrivateKey(akp, out keyType);

					type = keyType + " PRIVATE KEY";
				}
				else
				{
					type = "PUBLIC KEY";

					encoding = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(akp).GetDerEncoded();
				}
			}
			else if (obj is IX509AttributeCertificate)
			{
				type = "ATTRIBUTE CERTIFICATE";
				encoding = ((X509V2AttributeCertificate)obj).GetEncoded();
			}
			else if (obj is Pkcs10CertificationRequest)
			{
				type = "CERTIFICATE REQUEST";
				encoding = ((Pkcs10CertificationRequest)obj).GetEncoded();
			}
			else if (obj is Asn1.Cms.ContentInfo)
			{
				type = "PKCS7";
				encoding = ((Asn1.Cms.ContentInfo)obj).GetEncoded();
			}
			else
			{
				throw new ArgumentException("Object type not supported: " + obj.GetType().FullName, "obj");
			}

			WritePemBlock(type, encoding);
		}

		public void WriteObject(
			object			obj,
			string			algorithm,
			char[]			password,
			SecureRandom	random)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");
			if (password == null)
				throw new ArgumentNullException("password");
			if (random == null)
				throw new ArgumentNullException("random");

			if (obj is AsymmetricCipherKeyPair)
			{
				WriteObject(((AsymmetricCipherKeyPair) obj).Private, algorithm, password, random);
				return;
			}

			string type = null;
			byte[] keyData = null;

			if (obj is AsymmetricKeyParameter)
			{
				AsymmetricKeyParameter akp = (AsymmetricKeyParameter) obj;
				if (akp.IsPrivate)
				{
					string keyType;
					keyData = EncodePrivateKey(akp, out keyType);

					type = keyType + " PRIVATE KEY";
				}
			}

			if (type == null || keyData == null)
			{
				// TODO Support other types?
				throw new ArgumentException("Object type not supported: " + obj.GetType().FullName, "obj");
			}


			string dekAlgName = algorithm.ToUpper(CultureInfo.InvariantCulture);

			// Note: For backward compatibility
			if (dekAlgName == "DESEDE")
			{
				dekAlgName = "DES-EDE3-CBC";
			}

			int ivLength = dekAlgName.StartsWith("AES-") ? 16 : 8;

			byte[] iv = new byte[ivLength];
			random.NextBytes(iv);

			byte[] encData = PemUtilities.Crypt(true, keyData, password, dekAlgName, iv);
			byte[] hexIV = Hex.Encode(iv);

			WritePemBlock(type, encData,
				"Proc-Type: 4,ENCRYPTED",
				"DEK-Info: " + dekAlgName + "," + Encoding.ASCII.GetString(hexIV, 0, hexIV.Length));
		}

		private byte[] EncodePrivateKey(
			AsymmetricKeyParameter	akp,
			out string				keyType)
		{
			PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(akp);

			DerObjectIdentifier oid = info.AlgorithmID.ObjectID;

			if (oid.Equals(X9ObjectIdentifiers.IdDsa))
			{
				keyType = "DSA";

				DsaParameter p = DsaParameter.GetInstance(info.AlgorithmID.Parameters);

				BigInteger x = ((DsaPrivateKeyParameters) akp).X;
				BigInteger y = p.G.ModPow(x, p.P);

				// TODO Create an ASN1 object somewhere for this?
				return new DerSequence(
					new DerInteger(0),
					new DerInteger(p.P),
					new DerInteger(p.Q),
					new DerInteger(p.G),
					new DerInteger(y),
					new DerInteger(x)).GetEncoded();
			}

			if (oid.Equals(PkcsObjectIdentifiers.RsaEncryption))
			{
				keyType = "RSA";
				return info.PrivateKey.GetEncoded();
			}

			throw new ArgumentException("Cannot handle private key of type: " + akp.GetType().FullName, "akp");
		}

		private void WritePemBlock(
			string			type,
			byte[]			data,
			params string[] fields)
		{
			WriteHeader(type);

			if (fields.Length > 0)
			{
				foreach (string field in fields)
				{
					writer.WriteLine(field);
				}

				writer.WriteLine();
			}

			WriteBytes(Base64.Encode(data));

			WriteFooter(type);
		}

		private void WriteHeader(
			string type)
		{
			writer.WriteLine("-----BEGIN " + type + "-----");
		}

		private void WriteFooter(
			string type)
		{
			writer.WriteLine("-----END " + type + "-----");
		}

		private const int LineLength = 64;

		private void WriteBytes(
			byte[] bytes)
		{
			int pos = 0;
			int remaining = bytes.Length;
			char[] buf = new char[LineLength];

			while (remaining > LineLength)
			{
				Encoding.ASCII.GetChars(bytes, pos, LineLength, buf, 0);
				writer.WriteLine(buf);

				pos += LineLength;
				remaining -= LineLength;
			}

			Encoding.ASCII.GetChars(bytes, pos, remaining, buf, 0);
			writer.WriteLine(buf, 0, remaining);
		}
	}
}
