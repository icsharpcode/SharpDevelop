// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class LocalizedTypeDescriptor : ICustomTypeDescriptor
	{
		string    defaultProperty = null;
		ArrayList properties      = new ArrayList();
		
		public ArrayList Properties {
			get {
				return properties;
			}
		}
		
		public string DefaultProperty {
			get {
				return defaultProperty;
			}
			set {
				defaultProperty = value;
			}
		}
		
		#region System.ComponentModel.ICustomTypeDescriptor interface implementation
		public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
		{
			return this;
		}
		
		public System.ComponentModel.PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
		{
			return new PropertyDescriptorCollection((PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor)));
		}
		
		public System.ComponentModel.PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}
		
		public System.ComponentModel.EventDescriptorCollection GetEvents(System.Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		
		public System.ComponentModel.EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		
		public object GetEditor(System.Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		
		public System.ComponentModel.PropertyDescriptor GetDefaultProperty()
		{
			return null;
		}
		
		public System.ComponentModel.EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		
		public System.ComponentModel.TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		
		public System.ComponentModel.AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		#endregion
	}
}
