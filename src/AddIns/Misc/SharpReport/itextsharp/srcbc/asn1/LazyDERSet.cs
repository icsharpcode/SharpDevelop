using System;
using System.Collections;
using System.Diagnostics;

namespace Org.BouncyCastle.Asn1
{
	internal class LazyDerSet
		: DerSet
	{
		private byte[]	encoded;
		private bool	parsed = false;

		internal LazyDerSet(
			byte[] encoded)
		{
			this.encoded = encoded;
		}

		private void Parse()
		{
			Debug.Assert(parsed == false);

			Asn1InputStream e = new LazyAsn1InputStream(encoded);

			Asn1Object o;
			while ((o = e.ReadObject()) != null)
			{
				AddObject(o);
			}

			encoded = null;
			parsed = true;
		}

		public override Asn1Encodable this[int index]
		{
			get
			{
				if (!parsed)
				{
					Parse();
				}

				return base[index];
			}
		}

		public override IEnumerator GetEnumerator()
		{
			if (!parsed)
			{
				Parse();
			}

			return base.GetEnumerator();
		}

		public override int Count
		{
			get
			{
				if (!parsed)
				{
					Parse();
				}

				return base.Count;
			}
		}

		internal override void Encode(
			DerOutputStream derOut)
		{
			if (parsed)
			{
				base.Encode(derOut);
			}
			else
			{
				derOut.WriteEncoded(Asn1Tags.Set | Asn1Tags.Constructed, encoded);
			}
		}
	}
}
