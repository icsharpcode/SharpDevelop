using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

using System;

namespace Org.BouncyCastle.Asn1.Sec
{
    /**
     * the elliptic curve private key object from SEC 1
     */
    public class ECPrivateKeyStructure
        : Asn1Encodable
    {
        private readonly Asn1Sequence seq;

        public ECPrivateKeyStructure(
            Asn1Sequence seq)
        {
			if (seq == null)
				throw new ArgumentNullException("seq");

			this.seq = seq;
        }

		public ECPrivateKeyStructure(
            BigInteger key)
        {
			this.seq = new DerSequence(
				new DerInteger(1),
				new DerOctetString(key.ToByteArrayUnsigned()));
        }

		public BigInteger GetKey()
        {
            Asn1OctetString octs = (Asn1OctetString) seq[1];

			return new BigInteger(1, octs.GetOctets());
        }

		public DerBitString GetPublicKey()
		{
			return (DerBitString) GetObjectInTag(1);
		}

		public Asn1Object GetParameters()
		{
			return GetObjectInTag(0);
		}

		private Asn1Object GetObjectInTag(
			int tagNo)
		{
			foreach (Asn1Encodable ae in seq)
			{
				Asn1Object obj = ae.ToAsn1Object();

				if (obj is Asn1TaggedObject)
				{
					Asn1TaggedObject tag = (Asn1TaggedObject) obj;
					if (tag.TagNo == tagNo)
					{
						return tag.GetObject();
					}
				}
			}

			return null;
		}

		/**
		 * ECPrivateKey ::= SEQUENCE {
		 *     version INTEGER { ecPrivkeyVer1(1) } (ecPrivkeyVer1),
		 *     privateKey OCTET STRING,
		 *     parameters [0] Parameters OPTIONAL,
		 *     publicKey [1] BIT STRING OPTIONAL }
		 */
		public override Asn1Object ToAsn1Object()
        {
            return seq;
        }
    }
}
