#if !NETCF_1_0

using System;
using System.Security.Cryptography;
using SystemX509 = System.Security.Cryptography.X509Certificates;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Security
{
	/// <summary>
	/// A class containing methods to interface the BouncyCastle world to the .NET Crypto world.
	/// </summary>
	public sealed class DotNetUtilities
	{
		private DotNetUtilities()
		{
		}

		/// <summary>
		/// Create an System.Security.Cryptography.X509Certificate from an X509Certificate Structure.
		/// </summary>
		/// <param name="x509Struct"></param>
		/// <returns>A System.Security.Cryptography.X509Certificate.</returns>
		public static SystemX509.X509Certificate ToX509Certificate(
			X509CertificateStructure x509Struct)
		{
			return new SystemX509.X509Certificate(x509Struct.GetDerEncoded());
		}

		public static SystemX509.X509Certificate ToX509Certificate(
			X509Certificate x509Cert)
		{
			return new SystemX509.X509Certificate(x509Cert.GetEncoded());
		}

		public static X509Certificate FromX509Certificate(
			SystemX509.X509Certificate x509Cert)
		{
			return new X509CertificateParser().ReadCertificate(x509Cert.GetRawCertData());
		}

		public static AsymmetricCipherKeyPair GetDsaKeyPair(
			DSACryptoServiceProvider dsaCsp)
		{
			return GetDsaKeyPair(dsaCsp.ExportParameters(true));
		}

		public static AsymmetricCipherKeyPair GetDsaKeyPair(
			DSAParameters dp)
		{
			DsaValidationParameters validationParameters = (dp.Seed != null)
				?	new DsaValidationParameters(dp.Seed, dp.Counter)
				:	null;

			DsaParameters parameters = new DsaParameters(
				new BigInteger(1, dp.P),
				new BigInteger(1, dp.Q),
				new BigInteger(1, dp.G),
				validationParameters);

			DsaPublicKeyParameters pubKey = new DsaPublicKeyParameters(
				new BigInteger(1, dp.Y),
				parameters);

			DsaPrivateKeyParameters privKey = new DsaPrivateKeyParameters(
				new BigInteger(1, dp.X),
				parameters);

			return new AsymmetricCipherKeyPair(pubKey, privKey);
		}

		public static DsaPublicKeyParameters GetDsaPublicKey(
			DSACryptoServiceProvider dsaCsp)
		{
			return GetDsaPublicKey(dsaCsp.ExportParameters(false));
		}

		public static DsaPublicKeyParameters GetDsaPublicKey(
			DSAParameters dp)
		{
			DsaValidationParameters validationParameters = (dp.Seed != null)
				?	new DsaValidationParameters(dp.Seed, dp.Counter)
				:	null;

			DsaParameters parameters = new DsaParameters(
				new BigInteger(1, dp.P),
				new BigInteger(1, dp.Q),
				new BigInteger(1, dp.G),
				validationParameters);

			return new DsaPublicKeyParameters(
				new BigInteger(1, dp.Y),
				parameters);
		}

		public static AsymmetricCipherKeyPair GetRsaKeyPair(
			RSACryptoServiceProvider rsaCsp)
		{
			return GetRsaKeyPair(rsaCsp.ExportParameters(true));
		}

		public static AsymmetricCipherKeyPair GetRsaKeyPair(
			RSAParameters rp)
		{
			BigInteger modulus = new BigInteger(1, rp.Modulus);
			BigInteger pubExp = new BigInteger(1, rp.Exponent);

			RsaKeyParameters pubKey = new RsaKeyParameters(
				false,
				modulus,
				pubExp);

			RsaPrivateCrtKeyParameters privKey = new RsaPrivateCrtKeyParameters(
				modulus,
				pubExp,
				new BigInteger(1, rp.D),
				new BigInteger(1, rp.P),
				new BigInteger(1, rp.Q),
				new BigInteger(1, rp.DP),
				new BigInteger(1, rp.DQ),
				new BigInteger(1, rp.InverseQ));

			return new AsymmetricCipherKeyPair(pubKey, privKey);
		}

		public static RsaKeyParameters GetRsaPublicKey(
			RSACryptoServiceProvider rsaCsp)
		{
			return GetRsaPublicKey(rsaCsp.ExportParameters(false));
		}

		public static RsaKeyParameters GetRsaPublicKey(
			RSAParameters rp)
		{
			return new RsaKeyParameters(
				false,
				new BigInteger(1, rp.Modulus),
				new BigInteger(1, rp.Exponent));
		}

		public static AsymmetricCipherKeyPair GetKeyPair(AsymmetricAlgorithm privateKey)
		{
			if (privateKey is DSACryptoServiceProvider)
			{
				return GetDsaKeyPair((DSACryptoServiceProvider) privateKey);
			}

			if (privateKey is RSACryptoServiceProvider)
			{
				return GetRsaKeyPair((RSACryptoServiceProvider) privateKey);
			}

			throw new ArgumentException("Unsupported algorithm specified", "privateKey");
		}
	}
}

#endif
