using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class KeyAgreeRecipientIdentifier
		: Asn1Encodable
	{
		/**
		 * return an KeyAgreeRecipientIdentifier object from a tagged object.
		 *
		 * @param obj the tagged object holding the object we want.
		 * @param isExplicit true if the object is meant to be explicitly
		 *              tagged false otherwise.
		 * @exception ArgumentException if the object held by the
		 *          tagged object cannot be converted.
		 */
		public static KeyAgreeRecipientIdentifier GetInstance(
			Asn1TaggedObject	obj,
			bool				isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}
    
		/**
		 * return an KeyAgreeRecipientIdentifier object from the given object.
		 *
		 * @param obj the object we want converted.
		 * @exception ArgumentException if the object cannot be converted.
		 */
		public static KeyAgreeRecipientIdentifier GetInstance(
			object obj)
		{
			if (obj == null || obj is KeyAgreeRecipientIdentifier)
			{
				return (KeyAgreeRecipientIdentifier)obj;
			}

			if (obj is Asn1Sequence)
			{
				return new KeyAgreeRecipientIdentifier((Asn1Sequence)obj);
			}

			throw new ArgumentException("Invalid KeyAgreeRecipientIdentifier: " + obj.GetType().FullName, "obj");
		} 

		private readonly IssuerAndSerialNumber issuerSerial;
		private const RecipientKeyIdentifier rKeyID = null;

		public KeyAgreeRecipientIdentifier(
			IssuerAndSerialNumber issuerSerial)
		{
			this.issuerSerial = issuerSerial;
		}

		private KeyAgreeRecipientIdentifier(
			Asn1Sequence seq)
		{
			this.issuerSerial = IssuerAndSerialNumber.GetInstance(seq);
		}

		public IssuerAndSerialNumber IssuerAndSerialNumber
		{
			get { return issuerSerial; }
		}

		public RecipientKeyIdentifier RKeyID
		{
			get { return rKeyID; }
		}

		/** 
		 * Produce an object suitable for an Asn1OutputStream.
		 * <pre>
		 * KeyAgreeRecipientIdentifier ::= CHOICE {
		 *     issuerAndSerialNumber IssuerAndSerialNumber,
		 *     rKeyId [0] IMPLICIT RecipientKeyIdentifier
		 * }
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			if (issuerSerial != null)
			{
				return issuerSerial.ToAsn1Object();
			}

			return new DerTaggedObject(false, 0, rKeyID);
		}
	}
}
