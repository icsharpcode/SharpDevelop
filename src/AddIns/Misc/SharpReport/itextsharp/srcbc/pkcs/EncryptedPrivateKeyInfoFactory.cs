using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Pkcs
{
    public sealed class EncryptedPrivateKeyInfoFactory
    {
        private EncryptedPrivateKeyInfoFactory()
        {
        }

        public static EncryptedPrivateKeyInfo CreateEncryptedPrivateKeyInfo(
            DerObjectIdentifier		algorithm,
            char[]					passPhrase,
            byte[]					salt,
            int						iterationCount,
            AsymmetricKeyParameter	key)
        {
            return CreateEncryptedPrivateKeyInfo(
				algorithm.Id, passPhrase, salt, iterationCount,
				PrivateKeyInfoFactory.CreatePrivateKeyInfo(key));
        }

		public static EncryptedPrivateKeyInfo CreateEncryptedPrivateKeyInfo(
			string					algorithm,
			char[]					passPhrase,
			byte[]					salt,
			int						iterationCount,
			AsymmetricKeyParameter	key)
		{
			return CreateEncryptedPrivateKeyInfo(
				algorithm, passPhrase, salt, iterationCount,
				PrivateKeyInfoFactory.CreatePrivateKeyInfo(key));
		}

		public static EncryptedPrivateKeyInfo CreateEncryptedPrivateKeyInfo(
            string			algorithm,
            char[]			passPhrase,
            byte[]			salt,
            int				iterationCount,
            PrivateKeyInfo	keyInfo)
        {
            if (!PbeUtilities.IsPbeAlgorithm(algorithm))
                throw new ArgumentException("attempt to use non-Pbe algorithm with Pbe EncryptedPrivateKeyInfo generation");

			IBufferedCipher cipher = PbeUtilities.CreateEngine(algorithm) as IBufferedCipher;

			if (cipher == null)
			{
				// TODO Throw exception?
			}

			Asn1Encodable parameters = PbeUtilities.GenerateAlgorithmParameters(
				algorithm, salt, iterationCount);

			ICipherParameters keyParameters = PbeUtilities.GenerateCipherParameters(
				algorithm, passPhrase, parameters);

			cipher.Init(true, keyParameters);

			byte[] keyBytes = keyInfo.GetEncoded();
			byte[] encoding = cipher.DoFinal(keyBytes);

			DerObjectIdentifier oid = PbeUtilities.GetObjectIdentifier(algorithm);
			AlgorithmIdentifier algID = new AlgorithmIdentifier(oid, parameters);

			return new EncryptedPrivateKeyInfo(algID, encoding);
        }
    }
}
