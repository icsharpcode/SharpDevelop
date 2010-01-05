using System;

namespace Org.BouncyCastle.Asn1.X509
{
    public class AlgorithmIdentifier
        : Asn1Encodable
    {
        private readonly DerObjectIdentifier	objectID;
        private readonly Asn1Encodable			parameters;

		public static AlgorithmIdentifier GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static AlgorithmIdentifier GetInstance(
            object obj)
        {
            if (obj == null || obj is AlgorithmIdentifier)
            {
                return (AlgorithmIdentifier) obj;
            }

			if (obj is DerObjectIdentifier)
            {
                return new AlgorithmIdentifier((DerObjectIdentifier) obj);
            }

			if (obj is string)
            {
                return new AlgorithmIdentifier((string) obj);
            }

			if (obj is Asn1Sequence)
            {
                return new AlgorithmIdentifier((Asn1Sequence) obj);
            }

			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public AlgorithmIdentifier(
            DerObjectIdentifier objectID)
        {
            this.objectID = objectID;
        }

		public AlgorithmIdentifier(
            string objectID)
        {
            this.objectID = new DerObjectIdentifier(objectID);
        }

		public AlgorithmIdentifier(
            DerObjectIdentifier	objectID,
            Asn1Encodable		parameters)
        {
            this.objectID = objectID;
            this.parameters = parameters;
        }

		internal AlgorithmIdentifier(
            Asn1Sequence seq)
        {
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}

			objectID = DerObjectIdentifier.GetInstance(seq[0]);

			if (seq.Count == 2)
            {
                parameters = seq[1];
            }
        }

		public virtual DerObjectIdentifier ObjectID
		{
			get { return objectID; }
		}

		public Asn1Encodable Parameters
		{
			get { return parameters; }
		}

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *      AlgorithmIdentifier ::= Sequence {
         *                            algorithm OBJECT IDENTIFIER,
         *                            parameters ANY DEFINED BY algorithm OPTIONAL }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(objectID);

			if (parameters != null)
            {
                v.Add(parameters);
            }

			return new DerSequence(v);
        }
    }
}
