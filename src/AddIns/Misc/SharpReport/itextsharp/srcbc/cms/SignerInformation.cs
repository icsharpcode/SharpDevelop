using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
	/**
	* an expanded SignerInfo block from a CMS Signed message
	*/
	public class SignerInformation
	{
		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private SignerID			sid;
		private SignerInfo			info;
		private AlgorithmIdentifier	digestAlgorithm;
		private AlgorithmIdentifier	encryptionAlgorithm;
		private Asn1Set				signedAttributes;
		private Asn1Set				unsignedAttributes;
		private CmsProcessable		content;
		private byte[]				signature;
		private DerObjectIdentifier	contentType;
		private IDigestCalculator	digestCalculator;
		private byte[]				resultDigest;

		internal SignerInformation(
			SignerInfo			info,
			DerObjectIdentifier	contentType,
			CmsProcessable		content,
			IDigestCalculator	digestCalculator)
		{
			this.info = info;
			this.sid = new SignerID();
			this.contentType = contentType;

			try
			{
				SignerIdentifier s = info.SignerID;

				if (s.IsTagged)
				{
					Asn1OctetString octs = Asn1OctetString.GetInstance(s.ID);

					sid.SubjectKeyIdentifier = octs.GetOctets();
				}
				else
				{
					Asn1.Cms.IssuerAndSerialNumber iAnds =
						Asn1.Cms.IssuerAndSerialNumber.GetInstance(s.ID);

					sid.Issuer = iAnds.Name;
					sid.SerialNumber = iAnds.SerialNumber.Value;
				}
			}
			catch (IOException)
			{
				throw new ArgumentException("invalid sid in SignerInfo");
			}

			this.digestAlgorithm = info.DigestAlgorithm;
			this.signedAttributes = info.AuthenticatedAttributes;
			this.unsignedAttributes = info.UnauthenticatedAttributes;
			this.encryptionAlgorithm = info.DigestEncryptionAlgorithm;
			this.signature = info.EncryptedDigest.GetOctets();

			this.content = content;
			this.digestCalculator = digestCalculator;
		}

		public SignerID SignerID
		{
			get { return sid; }
		}

		/**
		* return the version number for this objects underlying SignerInfo structure.
		*/
		public int Version
		{
			get { return info.Version.Value.IntValue; }
		}

		public AlgorithmIdentifier DigestAlgorithmID
		{
			get { return digestAlgorithm; }
		}

		/**
		* return the object identifier for the signature.
		*/
		public string DigestAlgOid
		{
			get { return digestAlgorithm.ObjectID.Id; }
		}

		/**
		* return the signature parameters, or null if there aren't any.
		*/
		public Asn1Object DigestAlgParams
		{
			get
			{
				Asn1Encodable ae = digestAlgorithm.Parameters;

				return ae == null ? null : ae.ToAsn1Object();
			}
		}

		/**
		 * return the content digest that was calculated during verification.
		 */
		public byte[] GetContentDigest()
		{
			if (resultDigest == null)
			{
				throw new InvalidOperationException("method can only be called after verify.");
			}

			return (byte[])resultDigest.Clone();
		}

		public AlgorithmIdentifier EncryptionAlgorithmID
		{
			get { return encryptionAlgorithm; }
		}

		/**
		* return the object identifier for the signature.
		*/
		public string EncryptionAlgOid
		{
			get { return encryptionAlgorithm.ObjectID.Id; }
		}

		/**
		* return the signature/encryption algorithm parameters, or null if
		* there aren't any.
		*/
		public Asn1Object EncryptionAlgParams
		{
			get
			{
				Asn1Encodable ae = encryptionAlgorithm.Parameters;

				return ae == null ? null : ae.ToAsn1Object();
			}
		}

		/**
		* return a table of the signed attributes - indexed by
		* the OID of the attribute.
		*/
		public Asn1.Cms.AttributeTable SignedAttributes
		{
			get
			{
				return signedAttributes == null
					?	null
					:	new Asn1.Cms.AttributeTable(signedAttributes);
			}
		}

		/**
		* return a table of the unsigned attributes indexed by
		* the OID of the attribute.
		*/
		public Asn1.Cms.AttributeTable UnsignedAttributes
		{
			get
			{
				return unsignedAttributes == null
					?	null
					:	new Asn1.Cms.AttributeTable(unsignedAttributes);
			}
		}

		/**
		* return the encoded signature
		*/
		public byte[] GetSignature()
		{
			return (byte[]) signature.Clone();
		}

		/**
		* Return a SignerInformationStore containing the counter signatures attached to this
		* signer. If no counter signatures are present an empty store is returned.
		*/
		public SignerInformationStore GetCounterSignatures()
		{
			Asn1.Cms.AttributeTable unsignedAttributeTable = UnsignedAttributes;
			if (unsignedAttributeTable == null)
			{
				return new SignerInformationStore(new ArrayList(0));
			}

			IList counterSignatures = new ArrayList();

			Asn1.Cms.Attribute counterSignatureAttribute = unsignedAttributeTable[CmsAttributes.CounterSignature];
			if (counterSignatureAttribute != null)
			{
				Asn1Set values = counterSignatureAttribute.AttrValues;
				counterSignatures = new ArrayList(values.Count);

				foreach (Asn1Encodable asn1Obj in values)
				{
					SignerInfo si = SignerInfo.GetInstance(asn1Obj.ToAsn1Object());

					string digestName = CmsSignedHelper.Instance.GetDigestAlgName(si.DigestAlgorithm.ObjectID.Id);

					counterSignatures.Add(new SignerInformation(si, CmsAttributes.CounterSignature, null, new CounterSignatureDigestCalculator(digestName, GetSignature())));
				}
			}

			return new SignerInformationStore(counterSignatures);
		}

		/**
		* return the DER encoding of the signed attributes.
		* @throws IOException if an encoding error occurs.
		*/
		public byte[] GetEncodedSignedAttributes()
		{
			return signedAttributes == null
				?	null
				:	signedAttributes.GetEncoded(Asn1Encodable.Der);
		}

		private bool DoVerify(
			AsymmetricKeyParameter	key,
			Asn1.Cms.AttributeTable	signedAttrTable)
		{
			string digestName = Helper.GetDigestAlgName(this.DigestAlgOid);
			IDigest digest = Helper.GetDigestInstance(digestName);

			DerObjectIdentifier sigAlgOid = this.encryptionAlgorithm.ObjectID;
			Asn1Encodable sigParams = this.encryptionAlgorithm.Parameters;
			ISigner sig;

			if (sigAlgOid.Equals(Asn1.Pkcs.PkcsObjectIdentifiers.IdRsassaPss))
			{
				// RFC 4056
				// When the id-RSASSA-PSS algorithm identifier is used for a signature,
				// the AlgorithmIdentifier parameters field MUST contain RSASSA-PSS-params.
				if (sigParams == null)
					throw new CmsException("RSASSA-PSS signature must specify algorithm parameters");

				try
				{
					// TODO Provide abstract configuration mechanism

					Asn1.Pkcs.RsassaPssParameters pss = Asn1.Pkcs.RsassaPssParameters.GetInstance(
						sigParams.ToAsn1Object());

					if (!pss.HashAlgorithm.ObjectID.Equals(this.digestAlgorithm.ObjectID))
						throw new CmsException("RSASSA-PSS signature parameters specified incorrect hash algorithm");
					if (!pss.MaskGenAlgorithm.ObjectID.Equals(Asn1.Pkcs.PkcsObjectIdentifiers.IdMgf1))
						throw new CmsException("RSASSA-PSS signature parameters specified unknown MGF");

					IDigest pssDigest = DigestUtilities.GetDigest(pss.HashAlgorithm.ObjectID);
					int saltLen = pss.SaltLength.Value.IntValue;
					byte trailer = (byte) pss.TrailerField.Value.IntValue;

					sig = new Crypto.Signers.PssSigner(new RsaBlindedEngine(), pssDigest, saltLen, trailer);
				}
				catch (Exception e)
				{
					throw new CmsException("failed to set RSASSA-PSS signature parameters", e);
				}
			}
			else
			{
				// TODO Probably too strong a check at the moment
//				if (sigParams != null)
//					throw new CmsException("unrecognised signature parameters provided");

				string signatureName = digestName + "with" + Helper.GetEncryptionAlgName(this.EncryptionAlgOid);

				sig = Helper.GetSignatureInstance(signatureName);
			}

			try
			{
				sig.Init(false, key);

				if (signedAttributes == null)
				{
					if (content != null)
					{
						content.Write(new CmsSignedDataGenerator.SigOutputStream(sig));
						content.Write(new CmsSignedDataGenerator.DigOutputStream(digest));

						resultDigest = DigestUtilities.DoFinal(digest);
					}
					else
					{
						resultDigest = digestCalculator.GetDigest();

						// need to decrypt signature and check message bytes
						return VerifyDigest(resultDigest, key, this.GetSignature());
					}
				}
				else
				{
					byte[] hash;
					if (content != null)
					{
						content.Write(
							new CmsSignedDataGenerator.DigOutputStream(digest));

						hash = DigestUtilities.DoFinal(digest);
					}
					else if (digestCalculator != null)
					{
						hash = digestCalculator.GetDigest();
					}
					else
					{
						hash = null;
					}

					resultDigest = hash;

					Asn1.Cms.Attribute dig = signedAttrTable[Asn1.Cms.CmsAttributes.MessageDigest];
					Asn1.Cms.Attribute type = signedAttrTable[Asn1.Cms.CmsAttributes.ContentType];

					if (dig == null)
					{
						throw new SignatureException("no hash for content found in signed attributes");
					}

					if (type == null && !contentType.Equals(CmsAttributes.CounterSignature))
					{
						throw new SignatureException("no content type id found in signed attributes");
					}

					Asn1Object hashObj = dig.AttrValues[0].ToAsn1Object();

					if (hashObj is Asn1OctetString)
					{
						byte[] signedHash = ((Asn1OctetString)hashObj).GetOctets();

						if (!Arrays.AreEqual(hash, signedHash))
						{
							throw new SignatureException("content hash found in signed attributes different");
						}
					}
					else if (hashObj is DerNull)
					{
						if (hash != null)
						{
							throw new SignatureException("NULL hash found in signed attributes when one expected");
						}
					}

					if (type != null)
					{
						DerObjectIdentifier typeOID = (DerObjectIdentifier)type.AttrValues[0];

						if (!typeOID.Equals(contentType))
						{
							throw new SignatureException("contentType in signed attributes different");
						}
					}

					byte[] tmp = this.GetEncodedSignedAttributes();
					sig.BlockUpdate(tmp, 0, tmp.Length);
				}

				return sig.VerifySignature(this.GetSignature());
			}
			catch (InvalidKeyException e)
			{
				throw new CmsException(
					"key not appropriate to signature in message.", e);
			}
			catch (IOException e)
			{
				throw new CmsException(
					"can't process mime object to create signature.", e);
			}
			catch (SignatureException e)
			{
				throw new CmsException(
					"invalid signature format in message: " + e.Message, e);
			}
		}

		private bool IsNull(
			Asn1Encodable o)
		{
			return (o is Asn1Null) || (o == null);
		}

		private DigestInfo DerDecode(
			byte[] encoding)
		{
			if (encoding[0] != (int)(Asn1Tags.Constructed | Asn1Tags.Sequence))
			{
				throw new IOException("not a digest info object");
			}

			DigestInfo digInfo = DigestInfo.GetInstance(Asn1Object.FromByteArray(encoding));

			// length check to avoid Bleichenbacher vulnerability

			if (digInfo.GetEncoded().Length != encoding.Length)
			{
				throw new CmsException("malformed RSA signature");
			}

			return digInfo;
		}

		private bool VerifyDigest(
			byte[]					digest,
			AsymmetricKeyParameter	key,
			byte[]					signature)
		{
			string algorithm = Helper.GetEncryptionAlgName(this.EncryptionAlgOid);

			try
			{
				if (algorithm.Equals("RSA"))
				{
					IBufferedCipher c = CipherUtilities.GetCipher("RSA//PKCS1Padding");

					c.Init(false, key);

					byte[] decrypt = c.DoFinal(signature);

					DigestInfo digInfo = DerDecode(decrypt);

					if (!digInfo.AlgorithmID.ObjectID.Equals(digestAlgorithm.ObjectID))
					{
						return false;
					}

					if (!IsNull(digInfo.AlgorithmID.Parameters))
					{
						return false;
					}

					byte[] sigHash = digInfo.GetDigest();

					return Arrays.AreEqual(digest, sigHash);
				}
				else if (algorithm.Equals("DSA"))
				{
					ISigner sig = SignerUtilities.GetSigner("NONEwithDSA");

					sig.Init(false, key);

					sig.BlockUpdate(digest, 0, digest.Length);

					return sig.VerifySignature(signature);
				}
				else
				{
					throw new CmsException("algorithm: " + algorithm + " not supported in base signatures.");
				}
			}
			catch (SecurityUtilityException e)
			{
				throw e;
			}
			catch (GeneralSecurityException e)
			{
				throw new CmsException("Exception processing signature: " + e, e);
			}
			catch (IOException e)
			{
				throw new CmsException("Exception decoding signature: " + e, e);
			}
		}

		/**
		* verify that the given public key succesfully handles and confirms the
		* signature associated with this signer.
		*/
		public bool Verify(
			AsymmetricKeyParameter pubKey)
		{
			if (pubKey.IsPrivate)
				throw new ArgumentException("Expected public key", "pubKey");

			return DoVerify(pubKey, this.SignedAttributes);
		}

		/**
		* verify that the given certificate successfully handles and confirms
		* the signature associated with this signer and, if a signingTime
		* attribute is available, that the certificate was valid at the time the
		* signature was generated.
		*/
		public bool Verify(
			X509Certificate cert)
		{
			Asn1.Cms.AttributeTable attr = this.SignedAttributes;

			if (attr != null)
			{
				Asn1EncodableVector v = attr.GetAll(CmsAttributes.SigningTime);
				switch (v.Count)
				{
					case 0:
						break;
					case 1:
					{
						Asn1.Cms.Attribute t = (Asn1.Cms.Attribute) v[0];
						Debug.Assert(t != null);

						Asn1Set attrValues = t.AttrValues;
						if (attrValues.Count != 1)
							throw new CmsException("A signing-time attribute MUST have a single attribute value");

						Asn1.Cms.Time time = Asn1.Cms.Time.GetInstance(attrValues[0].ToAsn1Object());

						cert.CheckValidity(time.Date);
						break;
					}
					default:
						throw new CmsException("The SignedAttributes in a signerInfo MUST NOT include multiple instances of the signing-time attribute");
				}
			}

			return DoVerify(cert.GetPublicKey(), attr);
		}

		/**
		* Return the base ASN.1 CMS structure that this object contains.
		*
		* @return an object containing a CMS SignerInfo structure.
		*/
		public SignerInfo ToSignerInfo()
		{
			return info;
		}

		/**
		* Return a signer information object with the passed in unsigned
		* attributes replacing the ones that are current associated with
		* the object passed in.
		*
		* @param signerInformation the signerInfo to be used as the basis.
		* @param unsignedAttributes the unsigned attributes to add.
		* @return a copy of the original SignerInformationObject with the changed attributes.
		*/
		public static SignerInformation ReplaceUnsignedAttributes(
			SignerInformation		signerInformation,
			Asn1.Cms.AttributeTable	unsignedAttributes)
		{
			SignerInfo sInfo = signerInformation.info;
			Asn1Set unsignedAttr = null;

			if (unsignedAttributes != null)
			{
				unsignedAttr = new DerSet(unsignedAttributes.ToAsn1EncodableVector());
			}

			return new SignerInformation(
				new SignerInfo(
					sInfo.SignerID,
					sInfo.DigestAlgorithm,
					sInfo.AuthenticatedAttributes,
					sInfo.DigestEncryptionAlgorithm,
					sInfo.EncryptedDigest,
					unsignedAttr),
				signerInformation.contentType,
				signerInformation.content,
				null);
		}

		/**
		 * Return a signer information object with passed in SignerInformationStore representing counter
		 * signatures attached as an unsigned attribute.
		 *
		 * @param signerInformation the signerInfo to be used as the basis.
		 * @param counterSigners signer info objects carrying counter signature.
		 * @return a copy of the original SignerInformationObject with the changed attributes.
		 */
		public static SignerInformation AddCounterSigners(
			SignerInformation		signerInformation,
			SignerInformationStore	counterSigners)
		{
			SignerInfo sInfo = signerInformation.info;
			Asn1.Cms.AttributeTable unsignedAttr = signerInformation.UnsignedAttributes;
			Asn1EncodableVector v;

			if (unsignedAttr != null)
			{
				v = unsignedAttr.ToAsn1EncodableVector();
			}
			else
			{
				v = new Asn1EncodableVector();
			}

			Asn1EncodableVector sigs = new Asn1EncodableVector();

			foreach (SignerInformation sigInf in counterSigners.GetSigners())
			{
				sigs.Add(sigInf.ToSignerInfo());
			}

			v.Add(new Asn1.Cms.Attribute(CmsAttributes.CounterSignature, new DerSet(sigs)));

			return new SignerInformation(
				new SignerInfo(
					sInfo.SignerID,
					sInfo.DigestAlgorithm,
					sInfo.AuthenticatedAttributes,
					sInfo.DigestEncryptionAlgorithm,
					sInfo.EncryptedDigest,
					new DerSet(v)),
				signerInformation.contentType,
				signerInformation.content,
				null);
		}
	}
}
