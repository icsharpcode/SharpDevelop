// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A property descriptor for the WixXmlAttribute.
	/// </summary>
	public class WixXmlAttributePropertyDescriptor : PropertyDescriptor
	{
		WixXmlAttribute wixXmlAttribute;
		
		/// <summary>
		/// Creates a new instance of the WixXmlAttributePropertyDescriptor class.
		/// </summary>
		public WixXmlAttributePropertyDescriptor(WixXmlAttribute wixXmlAttribute) : base(wixXmlAttribute.Name, GetAttributes(wixXmlAttribute))
		{
			this.wixXmlAttribute = wixXmlAttribute;
		}
		
		/// <summary>
		/// Converts a collection of WixXmlAttributes into a property descriptor collection.
		/// </summary>
		public static PropertyDescriptorCollection GetProperties(IList wixXmlAttributes)
		{
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			foreach (WixXmlAttribute wixXmlAttribute in wixXmlAttributes) {
				properties.Add(new WixXmlAttributePropertyDescriptor(wixXmlAttribute));
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}
		
		public WixXmlAttribute WixXmlAttribute {
			get {
				return wixXmlAttribute;
			}
		}
		
		public override Type ComponentType {
			get {
				return wixXmlAttribute.GetType();
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
				return typeof(string);
			}
		}
		
		public override bool CanResetValue(object component)
		{
			return false;
		}
		
		public override object GetValue(object component)
		{
			return wixXmlAttribute.Value;
		}
		
		public override void ResetValue(object component)
		{
		}
		
		/// <summary>
		/// Sets the value after it has been updated in the property grid.
		/// </summary>
		public override void SetValue(object component, object value)
		{
			wixXmlAttribute.Value = (string)value;
		}
		
		/// <summary>
		/// If the current value has changed from the default value then this
		/// method will return true.
		/// </summary>
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}	
		
		/// <summary>
		/// Gets the attributes for the property descriptor based on the
		/// WixXmlAttribute.
		/// </summary>
		/// <remarks>Adds an EditorAttribute for types (e.g. AutogenUuid)
		/// that have a custom UITypeEditor.</remarks>
		static Attribute[] GetAttributes(WixXmlAttribute wixXmlAttribute)
		{
			List<Attribute> attributes = new List<Attribute>();
			switch (wixXmlAttribute.AttributeType) {
				case WixXmlAttributeType.ComponentGuid:
				case WixXmlAttributeType.AutogenGuid:
				case WixXmlAttributeType.Guid:
					attributes.Add(new EditorAttribute(typeof(GuidEditor), typeof(UITypeEditor)));
					break;
				case WixXmlAttributeType.FileName:
					attributes.Add(new EditorAttribute(typeof(RelativeFileNameEditor), typeof(UITypeEditor)));
					break;
				case WixXmlAttributeType.Text:
					if (wixXmlAttribute.HasValues) {
						attributes.Add(new EditorAttribute(typeof(WixDropDownEditor), typeof(UITypeEditor)));
					}
					break;
			}
			return attributes.ToArray();
		}
	}
}
