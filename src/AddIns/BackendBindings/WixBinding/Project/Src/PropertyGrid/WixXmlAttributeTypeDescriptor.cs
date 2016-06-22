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
