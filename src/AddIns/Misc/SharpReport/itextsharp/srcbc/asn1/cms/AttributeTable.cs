using System;
using System.Collections;

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Cms
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

			foreach (Asn1Encodable o in v)
            {
                Attribute a = Attribute.GetInstance(o);

				AddAttribute(a);
            }
        }

        public AttributeTable(
            Asn1Set s)
        {
			this.attributes = new Hashtable(s.Count);

			for (int i = 0; i != s.Count; i++)
            {
                Attribute a = Attribute.GetInstance(s[i]);

                AddAttribute(a);
            }
        }

        private void AddAttribute(
            Attribute a)
        {
			DerObjectIdentifier oid = a.AttrType;
            object obj = attributes[oid];

            if (obj == null)
            {
                attributes[oid] = a;
            }
            else
            {
                ArrayList v;

                if (obj is Attribute)
                {
                    v = new ArrayList();

                    v.Add(obj);
                    v.Add(a);
                }
                else
                {
                    v = (ArrayList) obj;

                    v.Add(a);
                }

                attributes[oid] = v;
            }
        }

		/// <summary>Return the first attribute matching the given OBJECT IDENTIFIER</summary>
		public Attribute this[DerObjectIdentifier oid]
		{
			get
			{
				object obj = attributes[oid];

				if (obj is ArrayList)
				{
					return (Attribute)((ArrayList)obj)[0];
				}

				return (Attribute) obj;
			}
		}

		[Obsolete("Use 'object[oid]' syntax instead")]
        public Attribute Get(
            DerObjectIdentifier oid)
        {
			return this[oid];
        }

		/**
        * Return all the attributes matching the OBJECT IDENTIFIER oid. The vector will be
        * empty if there are no attributes of the required type present.
        *
        * @param oid type of attribute required.
        * @return a vector of all the attributes found of type oid.
        */
        public Asn1EncodableVector GetAll(
            DerObjectIdentifier oid)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

            object obj = attributes[oid];

			if (obj is ArrayList)
            {
                foreach (Attribute a in (ArrayList)obj)
                {
                    v.Add(a);
                }
            }
            else if (obj != null)
            {
                v.Add((Attribute) obj);
            }

			return v;
        }

		public Hashtable ToHashtable()
        {
            return new Hashtable(attributes);
        }

		public Asn1EncodableVector ToAsn1EncodableVector()
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

			foreach (object obj in attributes.Values)
            {
                if (obj is ArrayList)
                {
                    foreach (object el in (ArrayList)obj)
                    {
                        v.Add(Attribute.GetInstance(el));
                    }
                }
                else
                {
                    v.Add(Attribute.GetInstance(obj));
                }
            }

			return v;
        }
    }
}
