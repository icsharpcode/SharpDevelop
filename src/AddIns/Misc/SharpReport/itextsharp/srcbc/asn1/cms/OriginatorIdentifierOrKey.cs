using System;

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Cms
{
    public class OriginatorIdentifierOrKey
        : Asn1Encodable
    {
        private Asn1Encodable id;

        public OriginatorIdentifierOrKey(
            IssuerAndSerialNumber id)
        {
            this.id = id;
        }

        public OriginatorIdentifierOrKey(
            Asn1OctetString id)
        {
            this.id = new DerTaggedObject(false, 0, id);
        }

        public OriginatorIdentifierOrKey(
            OriginatorPublicKey id)
        {
            this.id = new DerTaggedObject(false, 1, id);
        }

        public OriginatorIdentifierOrKey(
            Asn1Object id)
        {
            this.id = id;
        }

        /**
         * return an OriginatorIdentifierOrKey object from a tagged object.
         *
         * @param o the tagged object holding the object we want.
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the object held by the
         *          tagged object cannot be converted.
         */
        public static OriginatorIdentifierOrKey GetInstance(
            Asn1TaggedObject	o,
            bool				explicitly)
        {
            if (!explicitly)
            {
                throw new ArgumentException(
                        "Can't implicitly tag OriginatorIdentifierOrKey");
            }

			return GetInstance(o.GetObject());
        }

        /**
         * return an OriginatorIdentifierOrKey object from the given object.
         *
         * @param o the object we want converted.
         * @exception ArgumentException if the object cannot be converted.
         */
        public static OriginatorIdentifierOrKey GetInstance(
            object o)
        {
            if (o == null || o is OriginatorIdentifierOrKey)
            {
                return (OriginatorIdentifierOrKey)o;
            }

			if (o is Asn1Object)
            {
                return new OriginatorIdentifierOrKey((Asn1Object)o);
            }

			throw new ArgumentException("Invalid OriginatorIdentifierOrKey: " + o.GetType().Name);
        }

		public Asn1Encodable ID
		{
			get { return id; }
		}

		public OriginatorPublicKey OriginatorKey
		{
			get
			{
				if (id is Asn1TaggedObject && ((Asn1TaggedObject)id).TagNo == 1)
				{
					return OriginatorPublicKey.GetInstance((Asn1TaggedObject)id, false);
				}

				return null;
			}
		}

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * OriginatorIdentifierOrKey ::= CHOICE {
         *     issuerAndSerialNumber IssuerAndSerialNumber,
         *     subjectKeyIdentifier [0] SubjectKeyIdentifier,
         *     originatorKey [1] OriginatorPublicKey
         * }
         *
         * SubjectKeyIdentifier ::= OCTET STRING
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            return id.ToAsn1Object();
        }
    }
}
