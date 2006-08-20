// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Type descriptor that allows us to display properties in the property grid
	/// for Wix element attributes.
	/// </summary>
	public class WixXmlAttributeTypeDescriptor : ICustomTypeDescriptor
	{
		PropertyDescriptorCollection properties;
		
		public WixXmlAttributeTypeDescriptor(IList wixXmlAttributes)
		{
			properties = WixXmlAttributePropertyDescriptor.GetProperties(wixXmlAttributes);
		}
		
		public WixXmlAttributeTypeDescriptor() : this(new List<WixXmlAttribute>())
		{
		}
		
		public AttributeCollection GetAttributes()
		{
			return null;
		}
		
		public string GetClassName()
		{
			return null;
		}
		
		public string GetComponentName()
		{
			return null;
		}
		
		public TypeConverter GetConverter()
		{
			return null;
		}
		
		public EventDescriptor GetDefaultEvent()
		{
			return null;
		}
		
		public PropertyDescriptor GetDefaultProperty()
		{
			return null;
		}
		
		public object GetEditor(Type editorBaseType)
		{
			return null;
		}
		
		public EventDescriptorCollection GetEvents()
		{
			return null;
		}
		
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return null;
		}
		
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[0]);
		}
		
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return properties;
		}
		
		/// <summary>
		/// Returns this class instance.
		/// </summary>
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
	}
}
