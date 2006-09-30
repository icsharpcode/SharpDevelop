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
		
		public WixXmlAttribute(string name, WixXmlAttributeType type) 
			: this(name, String.Empty, type)
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
	}
}
