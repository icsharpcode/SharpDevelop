// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.ComponentModel;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Type descriptor that allows us to display properties in the property grid
	/// for Xml attributes.
	/// </summary>
	public class XmlAttributeTypeDescriptor : ICustomTypeDescriptor
	{
		PropertyDescriptorCollection properties;
		
		public XmlAttributeTypeDescriptor(XmlAttributeCollection xmlAttributes)
		{
			if (xmlAttributes != null) {
				properties = XmlAttributePropertyDescriptor.GetProperties(xmlAttributes);
			} else {
				properties = new PropertyDescriptorCollection(new XmlAttributePropertyDescriptor[0]);
			}
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
