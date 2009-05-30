// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Property descriptor for an XmlAttribute. This is used when displaying
	/// an XmlAttribute in the property grid.
	/// </summary>
	public class XmlAttributePropertyDescriptor : PropertyDescriptor
	{
		XmlAttribute xmlAttribute;
		public XmlAttributePropertyDescriptor(XmlAttribute xmlAttribute)
			: base(xmlAttribute.LocalName, new Attribute[0])
		{
			this.xmlAttribute = xmlAttribute;
		}
		
		/// <summary>
		/// Gets the property descriptors for the specified attributes.
		/// </summary>
		public static PropertyDescriptorCollection GetProperties(XmlAttributeCollection xmlAttributes)
		{
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			foreach (XmlAttribute xmlAttribute in xmlAttributes) {
				properties.Add(new XmlAttributePropertyDescriptor(xmlAttribute));
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		
		public override Type ComponentType {
			get {
				return typeof(String);
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Returns the property type in this case a string.
		/// </summary>
		public override Type PropertyType {
			get {
				return typeof(String);
			}
		}
		
		public override bool CanResetValue(object component)
		{
			return false;
		}
		
		/// <summary>
		/// Gets the value of the xml attribute.
		/// </summary>
		public override object GetValue(object component)
		{
			return xmlAttribute.Value;
		}
		
		public override void ResetValue(object component)
		{
		}
		
		/// <summary>
		/// Sets the xml attribute value.
		/// </summary>
		public override void SetValue(object component, object value)
		{
			xmlAttribute.Value = (String)value;
		}
		
		/// <summary>
		/// If the current value has changed from the default value then this
		/// method will return true.
		/// </summary>
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
