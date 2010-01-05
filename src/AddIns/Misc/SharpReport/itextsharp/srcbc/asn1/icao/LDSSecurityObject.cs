using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Icao
{
    /**
    * The LDSSecurityObject object.
    * <pre>
    * LDSSecurityObject ::= SEQUENCE {
    *   version                LDSSecurityObjectVersion,
    *   hashAlgorithm          DigestAlgorithmIdentifier,
    *   dataGroupHashValues    SEQUENCE SIZE (2..ub-DataGroups) OF DataHashGroup}
    *
    * DigestAlgorithmIdentifier ::= AlgorithmIdentifier,
    *
    * LDSSecurityObjectVersion :: INTEGER {V0(0)}
    * </pre>
    */

    public class LdsSecurityObject
        : Asn1Encodable
    {
        public const int UBDataGroups = 16;

		internal DerInteger version = new DerInteger(0);
        internal AlgorithmIdentifier digestAlgorithmIdentifier;
        internal DataGroupHash[] datagroupHash;

		public static LdsSecurityObject GetInstance(
            object obj)
        {
            if (obj == null || obj is LdsSecurityObject)
            {
                return (LdsSecurityObject) obj;
            }

            if (obj is Asn1Sequence)
            {
                return new LdsSecurityObject(Asn1Sequence.GetInstance(obj));
            }

			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName);
		}

		public LdsSecurityObject(
            Asn1Sequence seq)
        {
            if (seq == null || seq.Count == 0)
            {
                throw new ArgumentException("null or empty sequence passed.");
            }

			IEnumerator e = seq.GetEnumerator();

			// version
            e.MoveNext();
            version = DerInteger.GetInstance(e.Current);
            // digestAlgorithmIdentifier
            e.MoveNext();
            digestAlgorithmIdentifier = AlgorithmIdentifier.GetInstance(e.Current);

			e.MoveNext();
            Asn1Sequence datagroupHashSeq = Asn1Sequence.GetInstance(e.Current);

			CheckDatagroupHashSeqSize(datagroupHashSeq.Count);

			datagroupHash = new DataGroupHash[datagroupHashSeq.Count];
            for (int i= 0; i< datagroupHashSeq.Count; i++)
            {
                datagroupHash[i] = DataGroupHash.GetInstance(datagroupHashSeq[i]);
            }
        }

		public LdsSecurityObject(
            AlgorithmIdentifier	digestAlgorithmIdentifier,
            DataGroupHash[]		datagroupHash)
        {
            this.digestAlgorithmIdentifier = digestAlgorithmIdentifier;
            this.datagroupHash = datagroupHash;

			CheckDatagroupHashSeqSize(datagroupHash.Length);
        }

		private void CheckDatagroupHashSeqSize(int size)
        {
            if (size < 2 || size > UBDataGroups)
            {
                throw new ArgumentException("wrong size in DataGroupHashValues : not in (2.."+ UBDataGroups +")");
            }
        }

		public AlgorithmIdentifier DigestAlgorithmIdentifier
		{
			get { return digestAlgorithmIdentifier; }
		}

		public DataGroupHash[] GetDatagroupHash()
        {
            return datagroupHash;
        }

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(
				version,
				digestAlgorithmIdentifier,
				new DerSequence(datagroupHash));
        }
    }
}
