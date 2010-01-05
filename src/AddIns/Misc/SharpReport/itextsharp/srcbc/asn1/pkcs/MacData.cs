using System;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class MacData
        : Asn1Encodable
    {
        internal DigestInfo	digInfo;
        internal byte[]		salt;
        internal BigInteger	iterationCount;

		public static MacData GetInstance(
            object obj)
        {
            if (obj is MacData)
            {
                return (MacData) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new MacData((Asn1Sequence) obj);
            }

			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		private MacData(
            Asn1Sequence seq)
        {
            this.digInfo = DigestInfo.GetInstance(seq[0]);
            this.salt = ((Asn1OctetString) seq[1]).GetOctets();

			if (seq.Count == 3)
            {
                this.iterationCount = ((DerInteger) seq[2]).Value;
            }
            else
            {
                this.iterationCount = BigInteger.One;
            }
        }

		public MacData(
            DigestInfo	digInfo,
            byte[]		salt,
            int			iterationCount)
        {
            this.digInfo = digInfo;
            this.salt = (byte[]) salt.Clone();
            this.iterationCount = BigInteger.ValueOf(iterationCount);
        }

		public DigestInfo Mac
		{
			get { return digInfo; }
		}

		public byte[] GetSalt()
        {
            return (byte[]) salt.Clone();
        }

		public BigInteger IterationCount
		{
			get { return iterationCount; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(
				digInfo,
				new DerOctetString(salt),
				new DerInteger(iterationCount));
        }
    }
}
