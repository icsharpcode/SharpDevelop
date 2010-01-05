using System;
using System.Text;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
    public class GeneralNames
        : Asn1Encodable
    {
        private readonly Asn1Sequence seq;

		public static GeneralNames GetInstance(
            object obj)
        {
            if (obj == null || obj is GeneralNames)
            {
                return (GeneralNames) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new GeneralNames((Asn1Sequence) obj);
            }

			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static GeneralNames GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		/// <summary>Construct a GeneralNames object containing one GeneralName.</summary>
		/// <param name="name">The name to be contained.</param>
		public GeneralNames(
			GeneralName name)
		{
			this.seq = new DerSequence(name);
		}

		private GeneralNames(
            Asn1Sequence seq)
        {
            this.seq = seq;
        }

		public GeneralName[] GetNames()
        {
            GeneralName[] names = new GeneralName[seq.Count];

			for (int i = 0; i != seq.Count; i++)
            {
                names[i] = GeneralName.GetInstance(seq[i]);
            }

			return names;
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * GeneralNames ::= Sequence SIZE {1..MAX} OF GeneralName
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            return seq;
        }

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			string sep = Platform.NewLine;
			GeneralName[] names = GetNames();

			buf.Append("GeneralNames:");
			buf.Append(sep);

			for (int i = 0; i != names.Length; i++)
			{
				buf.Append("    ");
				buf.Append(names[i]);
				buf.Append(sep);
			}
			return buf.ToString();
		}
	}
}
