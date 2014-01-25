// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
