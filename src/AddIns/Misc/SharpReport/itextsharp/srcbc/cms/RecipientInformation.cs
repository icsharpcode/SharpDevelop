using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Cms
{
    public abstract class RecipientInformation
    {
		internal RecipientID _rid = new RecipientID();
        internal AlgorithmIdentifier _encAlg;
        internal AlgorithmIdentifier _keyEncAlg;
        internal Stream _data;

		internal RecipientInformation(
            AlgorithmIdentifier	encAlg,
            AlgorithmIdentifier	keyEncAlg,
            Stream				data)
        {
			if (!data.CanRead)
				throw new ArgumentException("Expected input stream", "data");

			this._encAlg = encAlg;
            this._keyEncAlg = keyEncAlg;
            this._data = data;
        }

		public RecipientID RecipientID
        {
			get { return _rid; }
        }

		public AlgorithmIdentifier KeyEncryptionAlgorithmID
		{
			get { return _keyEncAlg; }
		}

		/**
        * return the object identifier for the key encryption algorithm.
		* @return OID for key encryption algorithm.
        */
        public string KeyEncryptionAlgOid
        {
			get { return _keyEncAlg.ObjectID.Id; }
        }

		/**
        * return the ASN.1 encoded key encryption algorithm parameters, or null if
        * there aren't any.
		* @return ASN.1 encoding of key encryption algorithm parameters.
        */
		public Asn1Object KeyEncryptionAlgParams
		{
			get
			{
				Asn1Encodable ae = _keyEncAlg.Parameters;

				return ae == null ? null : ae.ToAsn1Object();
			}
		}

		internal CmsTypedStream GetContentFromSessionKey(
            KeyParameter sKey)
        {
			try
            {
				IBufferedCipher cipher =  CipherUtilities.GetCipher(_encAlg.ObjectID);

				Asn1Encodable asn1Enc = _encAlg.Parameters;
				Asn1Object asn1Params = asn1Enc == null ? null : asn1Enc.ToAsn1Object();

				ICipherParameters cipherParameters = sKey;

				if (asn1Params != null && !(asn1Params is Asn1Null))
                {
					cipherParameters = ParameterUtilities.GetCipherParameters(
						_encAlg.ObjectID, cipherParameters, asn1Params);
                }
                else
                {
					string alg = _encAlg.ObjectID.Id;
					if (alg.Equals(CmsEnvelopedDataGenerator.DesEde3Cbc)
                        || alg.Equals(CmsEnvelopedDataGenerator.IdeaCbc)
                        || alg.Equals(CmsEnvelopedDataGenerator.Cast5Cbc))
                    {
                        cipherParameters = new ParametersWithIV(cipherParameters, new byte[8]);
                    }
                }

				cipher.Init(false, cipherParameters);

				return new CmsTypedStream(new CipherStream(_data, cipher, null));
            }
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e)
            {
                throw new CmsException("key invalid in message.", e);
            }
            catch (IOException e)
            {
                throw new CmsException("error decoding algorithm parameters.", e);
            }
        }

		public byte[] GetContent(
            ICipherParameters key)
        {
            try
            {
                if (_data is MemoryStream)
                {
//					_data.Reset();
					_data.Seek(0L, SeekOrigin.Begin);
                }

				return CmsUtilities.StreamToByteArray(GetContentStream(key).ContentStream);
            }
            catch (IOException e)
            {
                throw new Exception("unable to parse internal stream: " + e);
            }
        }

		public abstract CmsTypedStream GetContentStream(ICipherParameters key);
    }
}
