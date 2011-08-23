/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2011
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
