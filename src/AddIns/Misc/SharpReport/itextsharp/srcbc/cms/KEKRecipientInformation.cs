using System;
using System.IO;

using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Cms
{
    /**
    * the RecipientInfo class for a recipient who has been sent a message
    * encrypted using a secret key known to the other side.
    */
    public class KekRecipientInformation
        : RecipientInformation
    {
        private KekRecipientInfo      _info;
//        private AlgorithmIdentifier   _encAlg;

        public KekRecipientInformation(
            KekRecipientInfo        info,
            AlgorithmIdentifier     encAlg,
            Stream             data)
            : base(encAlg, AlgorithmIdentifier.GetInstance(info.KeyEncryptionAlgorithm), data)
        {
            this._info = info;
            this._encAlg = encAlg;
            this._rid = new RecipientID();

			KekIdentifier kekId = info.KekID;

			_rid.KeyIdentifier = kekId.KeyIdentifier.GetOctets();
        }

		/**
        * decrypt the content and return an input stream.
        */
        public override CmsTypedStream GetContentStream(
            ICipherParameters key)
        {
			try
			{
				byte[] encryptedKey = _info.EncryptedKey.GetOctets();
				IWrapper keyWrapper = WrapperUtilities.GetWrapper(_keyEncAlg.ObjectID.Id);

				keyWrapper.Init(false, key);

				KeyParameter sKey = ParameterUtilities.CreateKeyParameter(
					_encAlg.ObjectID, keyWrapper.Unwrap(encryptedKey, 0, encryptedKey.Length));

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
