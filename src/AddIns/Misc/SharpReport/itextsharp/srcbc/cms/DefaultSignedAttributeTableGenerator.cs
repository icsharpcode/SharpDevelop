using System;
using System.Collections;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;

namespace Org.BouncyCastle.Cms
{
	/**
	 * Default signed attributes generator.
	 */
	public class DefaultSignedAttributeTableGenerator
		: CmsAttributeTableGenerator
	{
		private readonly Hashtable table;

		/**
		 * Initialise to use all defaults
		 */
		public DefaultSignedAttributeTableGenerator()
		{
			table = new Hashtable();
		}

		/**
		 * Initialise with some extra attributes or overrides.
		 *
		 * @param attributeTable initial attribute table to use.
		 */
		public DefaultSignedAttributeTableGenerator(
			AttributeTable attributeTable)
		{
			if (attributeTable != null)
			{
				table = attributeTable.ToHashtable();
			}
			else
			{
				table = new Hashtable();
			}
		}

		/**
		 * Create a standard attribute table from the passed in parameters - this will
		 * normally include contentType, signingTime, and messageDigest. If the constructor
		 * using an AttributeTable was used, entries in it for contentType, signingTime, and
		 * messageDigest will override the generated ones.
		 *
		 * @param parameters source parameters for table generation.
		 *
		 * @return a filled in Hashtable of attributes.
		 */
		protected virtual Hashtable createStandardAttributeTable(
			IDictionary parameters)
		{
			Hashtable std = (Hashtable)table.Clone();

			if (!std.ContainsKey(CmsAttributes.ContentType))
			{
				Asn1.Cms.Attribute attr = new Asn1.Cms.Attribute(CmsAttributes.ContentType,
					new DerSet((DerObjectIdentifier)parameters[CmsAttributeTableParameter.ContentType]));
				std[attr.AttrType] = attr;
			}

			if (!std.ContainsKey(CmsAttributes.SigningTime))
			{
				Asn1.Cms.Attribute attr = new Asn1.Cms.Attribute(
					CmsAttributes.SigningTime, new DerSet(new Time(DateTime.UtcNow)));
				std[attr.AttrType] = attr;
			}

			if (!std.ContainsKey(CmsAttributes.MessageDigest))
			{
				byte[] hash = (byte[])parameters[CmsAttributeTableParameter.Digest];
				Asn1.Cms.Attribute attr;

				if (hash != null)
				{
					attr = new Asn1.Cms.Attribute(
						CmsAttributes.MessageDigest, new DerSet(new DerOctetString(hash)));
				}
				else
				{
					attr = new Asn1.Cms.Attribute(
						CmsAttributes.MessageDigest, new DerSet(DerNull.Instance));
				}

				std[attr.AttrType] = attr;
			}

			return std;
		}

		/**
		 * @param parameters source parameters
		 * @return the populated attribute table
		 */
		public virtual AttributeTable GetAttributes(
			IDictionary parameters)
		{
			return new AttributeTable(createStandardAttributeTable(parameters));
		}
	}
}
