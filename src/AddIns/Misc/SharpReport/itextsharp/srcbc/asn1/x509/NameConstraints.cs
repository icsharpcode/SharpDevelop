using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class NameConstraints
		: Asn1Encodable
	{
		private Asn1Sequence permitted, excluded;

		public NameConstraints(
			Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject o in seq)
			{
				switch (o.TagNo)
				{
					case 0:
						permitted = Asn1Sequence.GetInstance(o, false);
						break;
					case 1:
						excluded = Asn1Sequence.GetInstance(o, false);
						break;
				}
			}
		}

		/**
		 * Constructor from a given details.
		 *
		 * <p>permitted and excluded are Vectors of GeneralSubtree objects.</p>
		 *
		 * @param permitted Permitted subtrees
		 * @param excluded Excluded subtrees
		 */
		public NameConstraints(
			ArrayList	permitted,
			ArrayList	excluded)
		{
			if (permitted != null)
			{
				this.permitted = createSequence(permitted);
			}

			if (excluded != null)
			{
				this.excluded = createSequence(excluded);
			}
		}

		private DerSequence createSequence(
			ArrayList subtree)
		{
			GeneralSubtree[] gsts = (GeneralSubtree[]) subtree.ToArray(typeof(GeneralSubtree));

			return new DerSequence(gsts);
		}

		public Asn1Sequence PermittedSubtrees
		{
			get { return permitted; }
		}

		public Asn1Sequence ExcludedSubtrees
		{
			get { return excluded; }
		}

		/*
		 * NameConstraints ::= SEQUENCE { permittedSubtrees [0] GeneralSubtrees
		 * OPTIONAL, excludedSubtrees [1] GeneralSubtrees OPTIONAL }
		 */
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			if (permitted != null)
			{
				v.Add(new DerTaggedObject(false, 0, permitted));
			}

			if (excluded != null)
			{
				v.Add(new DerTaggedObject(false, 1, excluded));
			}

			return new DerSequence(v);
		}
	}
}
