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
using System.ComponentModel;
using ICSharpCode.Reports.Addin.Designer;


namespace ICSharpCode.Reports.Addin.TypeProviders
{
	
	internal class CircleItemTypeProvider : TypeDescriptionProvider
	{
		public CircleItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new CircleItemTypeDescriptor(td, instance);
		}
	}
	
	
	
	internal class CircleItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public CircleItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
		}

		
		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			System.Collections.Generic.List<PropertyDescriptor> allProperties = new System.Collections.Generic.List<PropertyDescriptor>();
			
			DesignerHelper.AddDefaultProperties(allProperties,props);
			DesignerHelper.AddGraphicProperties(allProperties,props);
			PropertyDescriptor prop = null;

			prop = props.Find("Controls",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
