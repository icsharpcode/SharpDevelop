using System.Collections;

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.X509
{
    public class AttributeTable
    {
        private readonly Hashtable attributes;

		public AttributeTable(
            Hashtable attrs)
        {
            this.attributes = new Hashtable(attrs);
        }

		public AttributeTable(
            Asn1EncodableVector v)
        {
			this.attributes = new Hashtable(v.Count);

			for (int i = 0; i != v.Count; i++)
            {
                AttributeX509 a = AttributeX509.GetInstance(v[i]);

				attributes.Add(a.AttrType, a);
            }
        }

		public AttributeTable(
            Asn1Set s)
        {
			this.attributes = new Hashtable(s.Count);

			for (int i = 0; i != s.Count; i++)
            {
                AttributeX509 a = AttributeX509.GetInstance(s[i]);

				attributes.Add(a.AttrType, a);
            }
        }

		public AttributeX509 Get(
            DerObjectIdentifier oid)
        {
            return (AttributeX509) attributes[oid];
        }

		public Hashtable ToHashtable()
        {
            return new Hashtable(attributes);
        }
    }
}
