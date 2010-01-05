using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Pkcs
{
    public sealed class PrivateKeyInfoFactory
    {
        private PrivateKeyInfoFactory()
        {
        }

        public static PrivateKeyInfo CreatePrivateKeyInfo(
			AsymmetricKeyParameter key)
        {
			if (key == null)
				throw new ArgumentNullException("key");
			if (!key.IsPrivate)
				throw new ArgumentException("Public key passed - private key expected", "key");

			if (key is ElGamalPrivateKeyParameters)
            {
                ElGamalPrivateKeyParameters _key = (ElGamalPrivateKeyParameters)key;
                return new PrivateKeyInfo(
                    new AlgorithmIdentifier(
                        OiwObjectIdentifiers.ElGamalAlgorithm,
                        new ElGamalParameter(
                            _key.Parameters.P,
                            _key.Parameters.G).ToAsn1Object()),
					new DerInteger(_key.X));
			}

			if (key is DsaPrivateKeyParameters)
            {
                DsaPrivateKeyParameters _key = (DsaPrivateKeyParameters)key;
                return new PrivateKeyInfo(
                    new AlgorithmIdentifier(
                        X9ObjectIdentifiers.IdDsa,
                        new DsaParameter(
                            _key.Parameters.P,
                            _key.Parameters.Q,
                            _key.Parameters.G).ToAsn1Object()),
					new DerInteger(_key.X));
            }

			if (key is DHPrivateKeyParameters)
            {
				/*
					Process DH private key.
					The value for L was set to zero implicitly.
					This is the same action as found in JCEDHPrivateKey GetEncoded method.
				*/

				DHPrivateKeyParameters _key = (DHPrivateKeyParameters)key;

				DHParameter withNewL = new DHParameter(
					_key.Parameters.P, _key.Parameters.G, 0);

				return new PrivateKeyInfo(
                    new AlgorithmIdentifier(
						PkcsObjectIdentifiers.DhKeyAgreement,
						withNewL.ToAsn1Object()),
					new DerInteger(_key.X));
            }

			if (key is RsaKeyParameters)
			{
				AlgorithmIdentifier algID = new AlgorithmIdentifier(
					PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance);

				RsaPrivateKeyStructure keyStruct;
				if (key is RsaPrivateCrtKeyParameters)
				{
					RsaPrivateCrtKeyParameters _key = (RsaPrivateCrtKeyParameters)key;

					keyStruct = new RsaPrivateKeyStructure(
						_key.Modulus,
						_key.PublicExponent,
						_key.Exponent,
						_key.P,
						_key.Q,
						_key.DP,
						_key.DQ,
						_key.QInv);
				}
				else
				{
					RsaKeyParameters _key = (RsaKeyParameters) key;

					keyStruct = new RsaPrivateKeyStructure(
						_key.Modulus,
						BigInteger.Zero,
						_key.Exponent,
						BigInteger.Zero,
						BigInteger.Zero,
						BigInteger.Zero,
						BigInteger.Zero,
						BigInteger.Zero);
				}

				return new PrivateKeyInfo(algID, keyStruct.ToAsn1Object());
			}

			if (key is ECPrivateKeyParameters)
            {
                ECPrivateKeyParameters _key = (ECPrivateKeyParameters)key;
				AlgorithmIdentifier algID;

				if (_key.AlgorithmName == "ECGOST3410")
				{
					if (_key.PublicKeyParamSet == null)
						throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");

					Gost3410PublicKeyAlgParameters gostParams = new Gost3410PublicKeyAlgParameters(
						_key.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet);

					algID = new AlgorithmIdentifier(
						CryptoProObjectIdentifiers.GostR3410x2001,
						gostParams.ToAsn1Object());
				}
				else
				{
					X9ECParameters ecP = new X9ECParameters(
						_key.Parameters.Curve,
						_key.Parameters.G,
						_key.Parameters.N,
						_key.Parameters.H,
						_key.Parameters.GetSeed());

					X962Parameters x962 = new X962Parameters(ecP);

					algID = new AlgorithmIdentifier(
						X9ObjectIdentifiers.IdECPublicKey,
						x962.ToAsn1Object());
				}

				return new PrivateKeyInfo(algID, new ECPrivateKeyStructure(_key.D).ToAsn1Object());
			}

			if (key is Gost3410PrivateKeyParameters)
			{
				Gost3410PrivateKeyParameters _key = (Gost3410PrivateKeyParameters)key;

				if (_key.PublicKeyParamSet == null)
					throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");

				byte[] keyEnc = _key.X.ToByteArrayUnsigned();
				byte[] keyBytes = new byte[keyEnc.Length];

				for (int i = 0; i != keyBytes.Length; i++)
				{
					keyBytes[i] = keyEnc[keyEnc.Length - 1 - i]; // must be little endian
				}

				Gost3410PublicKeyAlgParameters algParams = new Gost3410PublicKeyAlgParameters(
					_key.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet, null);

				AlgorithmIdentifier algID = new AlgorithmIdentifier(
					CryptoProObjectIdentifiers.GostR3410x94,
					algParams.ToAsn1Object());

				return new PrivateKeyInfo(algID, new DerOctetString(keyBytes));
			}

			throw new ArgumentException("Class provided is not convertible: " + key.GetType().FullName);
        }

		public static PrivateKeyInfo CreatePrivateKeyInfo(
			char[]					passPhrase,
			EncryptedPrivateKeyInfo	encInfo)
		{
			return CreatePrivateKeyInfo(passPhrase, false, encInfo);
		}

		public static PrivateKeyInfo CreatePrivateKeyInfo(
			char[]					passPhrase,
			bool					wrongPkcs12Zero,
			EncryptedPrivateKeyInfo	encInfo)
        {
			AlgorithmIdentifier algID = encInfo.EncryptionAlgorithm;
			IBufferedCipher cipher = PbeUtilities.CreateEngine(algID.ObjectID) as IBufferedCipher;

			if (cipher == null)
			{
				// TODO Throw exception?
			}

			ICipherParameters keyParameters = PbeUtilities.GenerateCipherParameters(
				algID.ObjectID, passPhrase, wrongPkcs12Zero, algID.Parameters);

			cipher.Init(false, keyParameters);

			byte[] keyBytes = encInfo.GetEncryptedData();
			byte[] encoding = cipher.DoFinal(keyBytes);
			Asn1Object asn1Data = Asn1Object.FromByteArray(encoding);

			return PrivateKeyInfo.GetInstance(asn1Data);
        }
    }
}
