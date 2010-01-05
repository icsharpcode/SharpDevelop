using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class Pbkdf2Params
		: Asn1Encodable
    {
        internal Asn1OctetString	octStr;
        internal DerInteger			iterationCount;
        internal DerInteger			keyLength;

		public static Pbkdf2Params GetInstance(
            object obj)
        {
            if (obj is Pbkdf2Params || obj == null)
            {
                return (Pbkdf2Params) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new Pbkdf2Params((Asn1Sequence) obj);
            }

			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public Pbkdf2Params(
			Asn1Sequence seq)
        {
            IEnumerator e = seq.GetEnumerator();

			e.MoveNext();
            octStr = (Asn1OctetString) e.Current;

			e.MoveNext();
            iterationCount = (DerInteger) e.Current;

			if (e.MoveNext())
            {
                keyLength = (DerInteger) e.Current;
            }
        }

        public Pbkdf2Params(
            byte[] salt,
            int iterationCount)
        {
            this.octStr = new DerOctetString(salt);
            this.iterationCount = new DerInteger(iterationCount);
        }

		public byte[] GetSalt()
        {
            return octStr.GetOctets();
        }

		public BigInteger IterationCount
		{
			get { return iterationCount.Value; }
		}

		public BigInteger KeyLength
		{
			get { return keyLength == null ? null : keyLength.Value; }
        }

		public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(
				octStr, iterationCount);

			if (keyLength != null)
            {
                v.Add(keyLength);
            }

			return new DerSequence(v);
        }
    }
}
