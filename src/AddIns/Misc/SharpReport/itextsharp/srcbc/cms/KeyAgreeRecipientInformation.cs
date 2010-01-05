using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
	/**
	* the RecipientInfo class for a recipient who has been sent a message
	* encrypted using key agreement.
	*/
	public class KeyAgreeRecipientInformation
		: RecipientInformation
	{
		private KeyAgreeRecipientInfo _info;
//		private AlgorithmIdentifier   _encAlg;
		private Asn1OctetString       _encryptedKey;

		public KeyAgreeRecipientInformation(
			KeyAgreeRecipientInfo	info,
			AlgorithmIdentifier		encAlg,
			Stream					data)
			: base(encAlg, AlgorithmIdentifier.GetInstance(info.KeyEncryptionAlgorithm), data)
		{
			_info = info;
//			_encAlg = encAlg;

			try
			{
				Asn1Sequence s = _info.RecipientEncryptedKeys;
				RecipientEncryptedKey id = RecipientEncryptedKey.GetInstance(s[0]);

				Asn1.Cms.IssuerAndSerialNumber iAnds = id.Identifier.IssuerAndSerialNumber;

//				byte[] issuerBytes = iAnds.Name.GetEncoded();

				_rid = new RecipientID();
//				_rid.SetIssuer(issuerBytes);
				_rid.Issuer = iAnds.Name;
				_rid.SerialNumber = iAnds.SerialNumber.Value;

				_encryptedKey = id.EncryptedKey;
			}
			catch (IOException e)
			{
				throw new ArgumentException("invalid rid in KeyAgreeRecipientInformation", e);
			}
		}

		/**
		* decrypt the content and return an input stream.
		*/
		public override CmsTypedStream GetContentStream(
//			Key key)
			ICipherParameters key)
		{
			if (!(key is AsymmetricKeyParameter))
				throw new ArgumentException("KeyAgreement requires asymmetric key", "key");

			AsymmetricKeyParameter privKey = (AsymmetricKeyParameter) key;

			if (!privKey.IsPrivate)
				throw new ArgumentException("Expected private key", "key");

			try
			{
				OriginatorPublicKey origK = _info.Originator.OriginatorKey;
				PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privKey);
				SubjectPublicKeyInfo pubInfo = new SubjectPublicKeyInfo(privInfo.AlgorithmID, origK.PublicKey.GetBytes());
				AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(pubInfo);

				string wrapAlg = DerObjectIdentifier.GetInstance(
					Asn1Sequence.GetInstance(_keyEncAlg.Parameters)[0]).Id;

				IBasicAgreement agreement = AgreementUtilities.GetBasicAgreementWithKdf(
					_keyEncAlg.ObjectID, wrapAlg);

				agreement.Init(privKey);

				BigInteger wKeyNum = agreement.CalculateAgreement(pubKey);
				// TODO Fix the way bytes are derived from the secret
				byte[] wKeyBytes = wKeyNum.ToByteArrayUnsigned();
				KeyParameter wKey = ParameterUtilities.CreateKeyParameter(wrapAlg, wKeyBytes);

				IWrapper keyCipher = WrapperUtilities.GetWrapper(wrapAlg);

				keyCipher.Init(false, wKey);

				AlgorithmIdentifier aid = _encAlg;
				string alg = aid.ObjectID.Id;

				byte[] encryptedKey = _encryptedKey.GetOctets();
				byte[] sKeyBytes = keyCipher.Unwrap(encryptedKey, 0, encryptedKey.Length);

				KeyParameter sKey = ParameterUtilities.CreateKeyParameter(alg, sKeyBytes);

				return GetContentFromSessionKey(sKey);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e)
			{
				throw new CmsException("key invalid in message.", e);
			}
			catch (Exception e)
			{
				throw new CmsException("originator key invalid.", e);
			}
		}
	}
}
