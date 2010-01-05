using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The extendedKeyUsage object.
     * <pre>
     *      extendedKeyUsage ::= Sequence SIZE (1..MAX) OF KeyPurposeId
     * </pre>
     */
    public class ExtendedKeyUsage
        : Asn1Encodable
    {
        internal readonly Hashtable usageTable = new Hashtable();
        internal readonly Asn1Sequence seq;

		public static ExtendedKeyUsage GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static ExtendedKeyUsage GetInstance(
            object obj)
        {
            if (obj is ExtendedKeyUsage)
            {
                return (ExtendedKeyUsage) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new ExtendedKeyUsage((Asn1Sequence) obj);
            }

			if (obj is X509Extension)
			{
				return GetInstance(X509Extension.ConvertValueToObject((X509Extension) obj));
			}

			throw new ArgumentException("Invalid ExtendedKeyUsage: " + obj.GetType().Name);
        }

		private ExtendedKeyUsage(
            Asn1Sequence seq)
        {
            this.seq = seq;

			foreach (object o in seq)
			{
				if (!(o is DerObjectIdentifier))
					throw new ArgumentException("Only DerObjectIdentifier instances allowed in ExtendedKeyUsage.");

				this.usageTable.Add(o, o);
            }
        }

		public ExtendedKeyUsage(
            ArrayList usages)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

			foreach (Asn1Object o in usages)
            {
				v.Add(o);

				this.usageTable.Add(o, o);
            }

			this.seq = new DerSequence(v);
        }

		public bool HasKeyPurposeId(
            KeyPurposeID keyPurposeId)
        {
            return usageTable[keyPurposeId] != null;
        }

		/**
		 * Returns all extended key usages.
		 * The returned ArrayList contains DerObjectIdentifier instances.
		 * @return An ArrayList with all key purposes.
		 */
		public ArrayList GetUsages()
		{
			return new ArrayList(usageTable.Values);
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get { return usageTable.Count; }
		}

		public int Count
		{
			get { return usageTable.Count; }
		}

		public override Asn1Object ToAsn1Object()
        {
            return seq;
        }
    }
}
