// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Gives the Xml Attribute a type.
	/// </summary>
	public class WixXmlAttribute
	{
		string name = String.Empty;
		string attributeValue = String.Empty;
		WixXmlAttributeType type = WixXmlAttributeType.Text;
		
		public WixXmlAttribute(string name, string value, WixXmlAttributeType type)
		{
			this.name = name;
			attributeValue = value;
			this.type = type;
		}
		
		public WixXmlAttribute(string name) : this(name, String.Empty, WixXmlAttributeType.Text)
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
		/// Gets the attributes for the specified element. Also adds any standard
		/// attributes for the element which are not set.
		/// </summary>
		/// <param name="attributes"/>All attributes that should exist in the returned
		/// attribute collection</param>
		public static WixXmlAttributeCollection GetAttributes(XmlElement element, string[] attributeNames)
		{
			WixXmlAttributeCollection attributes = new WixXmlAttributeCollection();
			foreach (XmlAttribute attribute in element.Attributes) {
				WixXmlAttribute wixAttribute = new WixXmlAttribute(attribute.Name, attribute.Value, WixXmlAttributeType.Text);
				attributes.Add(wixAttribute);
			}
			attributes.AddRange(GetMissingAttributes(attributes, attributeNames));
			return attributes;
		}
		
		/// <summary>
		/// Gets the attributes that have not been added to the 
		/// <paramref name="existingAttributes"/>.
		/// </summary>		
		static WixXmlAttributeCollection GetMissingAttributes(WixXmlAttributeCollection existingAttributes, string[] attributes)
		{
			WixXmlAttributeCollection missingAttributes = new WixXmlAttributeCollection();
			foreach (string name in attributes) {
				if (existingAttributes[name] == null) {
					missingAttributes.Add(new WixXmlAttribute(name));
				}
			}
			return missingAttributes;
		}
	}
}
