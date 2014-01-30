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

using ICSharpCode.Reports.Addin.Designer;

namespace ICSharpCode.Reports.Addin.TypeProviders
{
	internal class ImageItemTypeProvider : TypeDescriptionProvider
	{
		public ImageItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		

		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new ImageItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class ImageItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public ImageItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
//			instance = instance as BaseTextItem;
		}

		
		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();
			

			DesignerHelper.AddDefaultProperties(allProperties,props);
			
			PropertyDescriptor prop = prop = props.Find("imageFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("Image",true);
			allProperties.Add(prop);
			
			prop = props.Find("ScaleImageToSize",true);
			allProperties.Add(prop);
			
			prop = props.Find("ImageSource",true);
			allProperties.Add(prop);
			
			prop = props.Find("ReportFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("RelativeFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("AbsoluteFileName",true);
			allProperties.Add(prop);
			
			prop = props.Find("ColumnName",true);
			allProperties.Add(prop);
			
			prop = props.Find("BaseTableName",true);
			allProperties.Add(prop);
			
			prop = props.Find("Name",true);
			allProperties.Add(prop);
			
			prop = props.Find("DataType",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
