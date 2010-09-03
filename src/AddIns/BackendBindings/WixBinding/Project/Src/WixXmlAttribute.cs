// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Gives the Xml Attribute a type.
	/// </summary>
	public class WixXmlAttribute
	{
		string[] values;
		string name = String.Empty;
		string attributeValue = String.Empty;
		WixXmlAttributeType type = WixXmlAttributeType.Text;
		WixDocument document;
		
		public WixXmlAttribute(string name, string value, WixXmlAttributeType type, string[] values, WixDocument document)
		{
			this.name = name;
			attributeValue = value;
			this.type = type;
			this.values = values;
			this.document = document;
		}
		
		public WixXmlAttribute(string name, string value, WixXmlAttributeType type)
			: this(name, value, type, new string[0], null)
		{
		}

		public WixXmlAttribute(string name, WixXmlAttributeType type)
			: this(name, String.Empty, type, new string[0], null)
		{
		}
		
		public WixXmlAttribute(string name, WixXmlAttributeType type, string[] values, WixDocument document)
			: this(name, String.Empty, type, values, document)
		{
		}

		/// <summary>
		/// Gets the name of the attribute.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// Gets or sets the value of the attribute.
		/// </summary>
		public string Value {
			get {
				return attributeValue;
			}
			set {
				if (value != null) {
					attributeValue = value;
				} else {
					attributeValue = String.Empty;
				}
			}
		}
		
		/// <summary>
		/// Gets the attribute type.
		/// </summary>
		public WixXmlAttributeType AttributeType {
			get {
				return type;
			}
		}
		
		/// <summary>
		/// Gets the set of allowed values for this attribute.
		/// </summary>
		public string[] Values {
			get {
				return values;
			}
		}
		
		/// <summary>
		/// Gets whether this attribute has any allowed values.
		/// </summary>
		public bool HasValues {
			get {
				if (values != null) {
					return values.Length > 0;
				}
				return false;
			}
		}
		
		/// <summary>
		/// Gets the WixDocument this attribute is associated with.
		/// </summary>
		public WixDocument Document {
			get {
				return document;
			}
		}
	}
}
