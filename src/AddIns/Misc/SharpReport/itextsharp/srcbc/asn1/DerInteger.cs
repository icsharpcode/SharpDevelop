using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    public class DerInteger
        : Asn1Object
    {
        private readonly byte[] bytes;

        /**
         * return an integer from the passed in object
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerInteger GetInstance(
            object obj)
        {
            if (obj == null)
            {
                return null;
            }

            DerInteger i = obj as DerInteger;
            if (i != null)
            {
                return i;
            }

			Asn1OctetString octs = obj as Asn1OctetString;
            if (octs != null)
            {
                return new DerInteger(octs.GetOctets());
            }

            Asn1TaggedObject tagged = obj as Asn1TaggedObject;
            if (tagged != null)
            {
                return GetInstance(tagged.GetObject());
            }

            throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return an Integer from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerInteger GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

			return GetInstance(obj.GetObject());
        }

        public DerInteger(
            int value)
        {
            bytes = BigInteger.ValueOf(value).ToByteArray();
        }

		public DerInteger(
            BigInteger value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

			bytes = value.ToByteArray();
        }

		public DerInteger(
            byte[] bytes)
        {
            this.bytes = bytes;
        }

		public BigInteger Value
        {
            get { return new BigInteger(bytes); }
        }

		/**
         * in some cases positive values Get crammed into a space,
         * that's not quite big enough...
         */
        public BigInteger PositiveValue
        {
            get { return new BigInteger(1, bytes); }
        }

        internal override void Encode(
            DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.Integer, bytes);
        }

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(bytes);
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			DerInteger other = asn1Object as DerInteger;

			if (other == null)
				return false;

			return Arrays.AreEqual(this.bytes, other.bytes);
        }

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
