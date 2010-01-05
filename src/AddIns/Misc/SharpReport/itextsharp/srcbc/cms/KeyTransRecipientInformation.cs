using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Asn1Pkcs = Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
    /**
    * the KeyTransRecipientInformation class for a recipient who has been sent a secret
    * key encrypted using their public key that needs to be used to
    * extract the message.
    */
    public class KeyTransRecipientInformation
        : RecipientInformation
    {
        private KeyTransRecipientInfo _info;
//        private new AlgorithmIdentifier   _encAlg;

		public KeyTransRecipientInformation(
            KeyTransRecipientInfo	info,
            AlgorithmIdentifier		encAlg,
            Stream					data)
            : base(encAlg, AlgorithmIdentifier.GetInstance(info.KeyEncryptionAlgorithm), data)
        {
            this._info = info;
//            this._encAlg = encAlg;
            this._rid = new RecipientID();

			RecipientIdentifier r = info.RecipientIdentifier;

			try
            {
                if (r.IsTagged)
                {
                    Asn1OctetString octs = Asn1OctetString.GetInstance(r.ID);

					_rid.SubjectKeyIdentifier = octs.GetOctets();
                }
                else
                {
                    IssuerAndSerialNumber iAnds = IssuerAndSerialNumber.GetInstance(r.ID);

					_rid.Issuer = iAnds.Name;
                    _rid.SerialNumber = iAnds.SerialNumber.Value;
                }
            }
            catch (IOException)
            {
                throw new ArgumentException("invalid rid in KeyTransRecipientInformation");
            }
        }

		private string GetExchangeEncryptionAlgorithmName(
			DerObjectIdentifier oid)
		{
			if (Asn1Pkcs.PkcsObjectIdentifiers.RsaEncryption.Equals(oid))
			{
				return "RSA//PKCS1Padding";
			}

			return oid.Id;
		}

		/**
        * decrypt the content and return it as a byte array.
        */
        public override CmsTypedStream GetContentStream(
            ICipherParameters key)
        {
			byte[] encryptedKey = _info.EncryptedKey.GetOctets();
			string keyExchangeAlgorithm = GetExchangeEncryptionAlgorithmName(_keyEncAlg.ObjectID);

			try
			{
				IWrapper keyWrapper = WrapperUtilities.GetWrapper(keyExchangeAlgorithm);

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
//			catch (IllegalBlockSizeException e)
			catch (DataLengthException e)
			{
				throw new CmsException("illegal blocksize in message.", e);
			}
//			catch (BadPaddingException e)
			catch (InvalidCipherTextException e)
			{
				throw new CmsException("bad padding in message.", e);
			}
		}
    }
}
