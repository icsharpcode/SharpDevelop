using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Cms
{
	/**
	 * the RecipientInfo class for a recipient who has been sent a message
	 * encrypted using a password.
	 */
	public class PasswordRecipientInformation
		: RecipientInformation
	{
		private readonly PasswordRecipientInfo	_info;
//		private readonly AlgorithmIdentifier	_encAlg;

		public PasswordRecipientInformation(
			PasswordRecipientInfo	info,
			AlgorithmIdentifier		encAlg,
			Stream					data)
			: base(encAlg, AlgorithmIdentifier.GetInstance(info.KeyEncryptionAlgorithm), data)
		{
			this._info = info;
//			this._encAlg = encAlg;
			this._rid = new RecipientID();
		}

		/**
		 * decrypt the content and return an input stream.
		 */
		public override CmsTypedStream GetContentStream(
			ICipherParameters key)
		{
			try
			{
				AlgorithmIdentifier kekAlg = AlgorithmIdentifier.GetInstance(_info.KeyEncryptionAlgorithm);
				Asn1Sequence        kekAlgParams = (Asn1Sequence)kekAlg.Parameters;
				byte[]              encryptedKey = _info.EncryptedKey.GetOctets();
				string              kekAlgName = DerObjectIdentifier.GetInstance(kekAlgParams[0]).Id;
				string				cName = CmsEnvelopedHelper.Instance.GetRfc3211WrapperName(kekAlgName);
				IWrapper			keyWrapper = WrapperUtilities.GetWrapper(cName);

				byte[] iv = Asn1OctetString.GetInstance(kekAlgParams[1]).GetOctets();

				ICipherParameters parameters = ((CmsPbeKey)key).GetEncoded(kekAlgName);
				parameters = new ParametersWithIV(parameters, iv);

				keyWrapper.Init(false, parameters);

				AlgorithmIdentifier aid = _encAlg;
				string alg = aid.ObjectID.Id;

				KeyParameter sKey = ParameterUtilities.CreateKeyParameter(
					alg, keyWrapper.Unwrap(encryptedKey, 0, encryptedKey.Length));

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
		}
	}
}
