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
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Templates
{
	class LocalizedTypeDescriptor : ICustomTypeDescriptor
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
