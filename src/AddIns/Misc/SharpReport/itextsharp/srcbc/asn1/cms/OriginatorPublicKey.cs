using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Cms
{
    public class OriginatorPublicKey
        : Asn1Encodable
    {
        private AlgorithmIdentifier algorithm;
        private DerBitString        publicKey;

		public OriginatorPublicKey(
            AlgorithmIdentifier algorithm,
            byte[]              publicKey)
        {
            this.algorithm = algorithm;
            this.publicKey = new DerBitString(publicKey);
        }

		public OriginatorPublicKey(
            Asn1Sequence seq)
        {
            algorithm = AlgorithmIdentifier.GetInstance(seq[0]);
            publicKey = (DerBitString) seq[1];
        }

		/**
         * return an OriginatorPublicKey object from a tagged object.
         *
         * @param obj the tagged object holding the object we want.
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the object held by the
         *          tagged object cannot be converted.
         */
        public static OriginatorPublicKey GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		/**
         * return an OriginatorPublicKey object from the given object.
         *
         * @param obj the object we want converted.
         * @exception ArgumentException if the object cannot be converted.
         */
        public static OriginatorPublicKey GetInstance(
            object obj)
        {
            if (obj == null || obj is OriginatorPublicKey)
            {
                return (OriginatorPublicKey)obj;
            }

			if (obj is Asn1Sequence)
            {
                return new OriginatorPublicKey((Asn1Sequence) obj);
            }

			throw new ArgumentException("Invalid OriginatorPublicKey: " + obj.GetType().Name);
        }

		public AlgorithmIdentifier Algorithm
		{
			get { return algorithm; }
		}

		public DerBitString PublicKey
		{
			get { return publicKey; }
		}

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * OriginatorPublicKey ::= Sequence {
         *     algorithm AlgorithmIdentifier,
         *     publicKey BIT STRING
         * }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(algorithm, publicKey);
        }
    }
}
