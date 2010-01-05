using System;
using System.Collections;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.OpenSsl
{
	/**
	* Class for reading OpenSSL PEM encoded streams containing 
	* X509 certificates, PKCS8 encoded keys and PKCS7 objects.
	* <p>
	* In the case of PKCS7 objects the reader will return a CMS ContentInfo object. Keys and
	* Certificates will be returned using the appropriate java.security type.</p>
	*/
	public class PemReader
	{
		private readonly TextReader reader;
		private readonly IPasswordFinder pFinder;

		public TextReader Reader
		{
			get { return reader; }
		}

		/**
		* Create a new PemReader
		*
		* @param reader the Reader
		*/
		public PemReader(
			TextReader reader)
			: this(reader, null)
		{
		}

		/**
		* Create a new PemReader with a password finder
		*
		* @param reader the Reader
		* @param pFinder the password finder
		*/
		public PemReader(
			TextReader		reader,
			IPasswordFinder	pFinder)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.reader = reader;
			this.pFinder = pFinder;
		}

		private const string BeginString = "-----BEGIN ";

		public object ReadObject()
		{
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				int startPos = line.IndexOf(BeginString);
				if (startPos == -1)
					continue;

				startPos += BeginString.Length;

				int endPos = line.IndexOf('-', startPos);
				if (endPos == -1)
					endPos = line.Length;

				string headerName = line.Substring(startPos, endPos - startPos).Trim();
				//Console.WriteLine("[" + headerName + "]");

				string endMarker = "-----END " + headerName;

				switch (headerName)
				{
					case "PUBLIC KEY":
						return ReadPublicKey(endMarker);
					case "RSA PUBLIC KEY":
						return ReadRsaPublicKey(endMarker);
					case "CERTIFICATE REQUEST":
					case "NEW CERTIFICATE REQUEST":
						return ReadCertificateRequest(endMarker);
					case "CERTIFICATE":
					case "X509 CERTIFICATE":
						return ReadCertificate(endMarker);
					case "PKCS7":
						return ReadPkcs7(endMarker);
					case "X509 CRL":
						return ReadCrl(endMarker);
					case "ATTRIBUTE CERTIFICATE":
						return ReadAttributeCertificate(endMarker);
					case "RSA PRIVATE KEY":
						return ReadKeyPair("RSA", endMarker);
					case "DSA PRIVATE KEY":
						return ReadKeyPair("DSA", endMarker);
					// TODO Add back in when tests done, and return type issue resolved
					//case "EC PARAMETERS":
					//	return ReadECParameters(endMarker);
					case "EC PRIVATE KEY":
						return ReadECPrivateKey(endMarker);
					default:
						// TODO Throw an exception for an unknown header?

						// Ignore contents
						ReadBytes(endMarker);
						break;
				}
			}

			return null;
		}

		private byte[] ReadBytes(
			string endMarker)
		{
			return ReadBytesAndFields(endMarker, null);
		}

		private byte[] ReadBytesAndFields(
			string		endMarker,
			IDictionary	fields)
		{
			StringBuilder buf = new StringBuilder();

			string line;
			while ((line = reader.ReadLine()) != null
				&& line.IndexOf(endMarker) == -1)
			{
				int colonPos = line.IndexOf(':');

				if (colonPos == -1)
				{
					buf.Append(line.Trim());
				}
				else if (fields != null)
				{
					// Process field
					string fieldName = line.Substring(0, colonPos).Trim();

					if (fieldName.StartsWith("X-"))
						fieldName = fieldName.Substring(2);

					string fieldValue = line.Substring(colonPos + 1).Trim();

					// TODO Complain if field already specified?
					fields[fieldName] = fieldValue;
				}
			}

			if (line == null)
			{
				throw new IOException(endMarker + " not found");
			}

			if (buf.Length % 4 != 0)
			{
				throw new IOException("base64 data appears to be truncated");
			}

			return Base64.Decode(buf.ToString());
		}

		private AsymmetricKeyParameter ReadRsaPublicKey(
			string endMarker) 
		{
			RsaPublicKeyStructure rsaPubStructure = RsaPublicKeyStructure.GetInstance(
				Asn1Object.FromByteArray(
				ReadBytes(endMarker)));

			return new RsaKeyParameters(
				false, // not private
				rsaPubStructure.Modulus, 
				rsaPubStructure.PublicExponent);
		}

		private AsymmetricKeyParameter ReadPublicKey(
			string endMarker)
		{
			return PublicKeyFactory.CreateKey(
				ReadBytes(endMarker));
		}

		/**
		* Reads in a X509Certificate.
		*
		* @return the X509Certificate
		* @throws IOException if an I/O error occured
		*/
		private X509Certificate ReadCertificate(
			string endMarker)
		{
			byte[] bytes = ReadBytes(endMarker);

			try
			{
				return new X509CertificateParser().ReadCertificate(bytes);
			}
			catch (Exception e)
			{
				throw new IOException("problem parsing cert: " + e.ToString());
			}
		}

		/**
		* Reads in a X509CRL.
		*
		* @return the X509Certificate
		* @throws IOException if an I/O error occured
		*/
		private X509Crl ReadCrl(
			string endMarker)
		{
			byte[] bytes = ReadBytes(endMarker);

			try
			{
				return new X509CrlParser().ReadCrl(bytes);
			}
			catch (Exception e)
			{
				throw new IOException("problem parsing cert: " + e.ToString());
			}
		}

		/**
		* Reads in a PKCS10 certification request.
		*
		* @return the certificate request.
		* @throws IOException if an I/O error occured
		*/
		private Pkcs10CertificationRequest ReadCertificateRequest(
			string endMarker)
		{
			byte[] bytes = ReadBytes(endMarker);

			try
			{
				return new Pkcs10CertificationRequest(bytes);
			}
			catch (Exception e)
			{
				throw new IOException("problem parsing cert: " + e.ToString());
			}
		}

		/**
		* Reads in a X509 Attribute Certificate.
		*
		* @return the X509 Attribute Certificate
		* @throws IOException if an I/O error occured
		*/
		private IX509AttributeCertificate ReadAttributeCertificate(
			string endMarker)
		{
			byte[] bytes = ReadBytes(endMarker);

			return new X509V2AttributeCertificate(bytes);
		}

		/**
		* Reads in a PKCS7 object. This returns a ContentInfo object suitable for use with the CMS
		* API.
		*
		* @return the X509Certificate
		* @throws IOException if an I/O error occured
		*/
		// TODO Consider returning Asn1.Pkcs.ContentInfo
		private Asn1.Cms.ContentInfo ReadPkcs7(
			string endMarker)
		{
			byte[] bytes = ReadBytes(endMarker);

			try
			{
				return Asn1.Cms.ContentInfo.GetInstance(
					Asn1Object.FromByteArray(bytes));
			}
			catch (Exception e)
			{
				throw new IOException("problem parsing PKCS7 object: " + e.ToString());
			}
		}

		/**
		* Read a Key Pair
		*/
		private AsymmetricCipherKeyPair ReadKeyPair(
			string	type,
			string	endMarker)
		{
			//
			// extract the key
			//
			IDictionary fields = new Hashtable();
			byte[] keyBytes = ReadBytesAndFields(endMarker, fields);

			string procType = (string) fields["Proc-Type"];

			if (procType == "4,ENCRYPTED")
			{
				if (pFinder == null)
					throw new InvalidOperationException("No password finder specified, but a password is required");

				char[] password = pFinder.GetPassword();

				if (password == null)
					throw new IOException("Password is null, but a password is required");

				string dekInfo = (string) fields["DEK-Info"];
				string[] tknz = dekInfo.Split(',');

				string dekAlgName = tknz[0].Trim();
				byte[] iv = Hex.Decode(tknz[1].Trim());

				keyBytes = PemUtilities.Crypt(false, keyBytes, password, dekAlgName, iv);
			}

			try
			{
				AsymmetricKeyParameter pubSpec, privSpec;
				Asn1Sequence seq = (Asn1Sequence) Asn1Object.FromByteArray(keyBytes);

				switch (type)
				{
					case "RSA":
					{
						RsaPrivateKeyStructure rsa = new RsaPrivateKeyStructure(seq);

						pubSpec = new RsaKeyParameters(false, rsa.Modulus, rsa.PublicExponent);
						privSpec = new RsaPrivateCrtKeyParameters(
							rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent,
							rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2,
							rsa.Coefficient);

						break;
					}

					case "DSA":
					{
						// TODO Create an ASN1 object somewhere for this?
						//DerInteger v = (DerInteger)seq[0];
						DerInteger p = (DerInteger)seq[1];
						DerInteger q = (DerInteger)seq[2];
						DerInteger g = (DerInteger)seq[3];
						DerInteger y = (DerInteger)seq[4];
						DerInteger x = (DerInteger)seq[5];

						DsaParameters parameters = new DsaParameters(p.Value, q.Value, g.Value);

						privSpec = new DsaPrivateKeyParameters(x.Value, parameters);
						pubSpec = new DsaPublicKeyParameters(y.Value, parameters);

						break;
					}

					default:
						throw new ArgumentException("Unknown key type: " + type, "type");
				}

				return new AsymmetricCipherKeyPair(pubSpec, privSpec);
			}
			catch (Exception e)
			{
				throw new IOException(
					"problem creating " + type + " private key: " + e.ToString());
			}
		}

		// TODO Add an equivalent class for ECNamedCurveParameterSpec?
		//private ECNamedCurveParameterSpec ReadECParameters(
		private X9ECParameters ReadECParameters(
			string endMarker)
		{
			byte[] bytes = ReadBytes(endMarker);
			DerObjectIdentifier oid = (DerObjectIdentifier) Asn1Object.FromByteArray(bytes);

			//return ECNamedCurveTable.getParameterSpec(oid.Id);
			return GetCurveParameters(oid.Id);
		}

		//private static ECDomainParameters GetCurveParameters(
		private static X9ECParameters GetCurveParameters(
			string name)
		{
			// TODO ECGost3410NamedCurves support (returns ECDomainParameters though)
			X9ECParameters ecP = X962NamedCurves.GetByName(name);

			if (ecP == null)
			{
				ecP = SecNamedCurves.GetByName(name);
				if (ecP == null)
				{
					ecP = NistNamedCurves.GetByName(name);
					if (ecP == null)
					{
						ecP = TeleTrusTNamedCurves.GetByName(name);

						if (ecP == null)
							throw new Exception("unknown curve name: " + name);
					}
				}
			}

			//return new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
			return ecP;
		}

		private AsymmetricCipherKeyPair ReadECPrivateKey(
			string endMarker)
		{
			try
			{
				byte[] bytes = ReadBytes(endMarker);
				ECPrivateKeyStructure pKey = new ECPrivateKeyStructure(
					(Asn1Sequence) Asn1Object.FromByteArray(bytes));
				AlgorithmIdentifier algId = new AlgorithmIdentifier(
					X9ObjectIdentifiers.IdECPublicKey, pKey.GetParameters());

				PrivateKeyInfo privInfo = new PrivateKeyInfo(algId, pKey.ToAsn1Object());
				SubjectPublicKeyInfo pubInfo = new SubjectPublicKeyInfo(algId, pKey.GetPublicKey().GetBytes());

				// TODO Are the keys returned here ECDSA, as Java version forces?
				return new AsymmetricCipherKeyPair(
					PublicKeyFactory.CreateKey(pubInfo),
					PrivateKeyFactory.CreateKey(privInfo));
			}
			catch (InvalidCastException e)
			{ 
				throw new IOException("wrong ASN.1 object found in stream.", e);
			}
			catch (Exception e)
			{
				throw new IOException("problem parsing EC private key.", e);
			}
		}
	}
}
