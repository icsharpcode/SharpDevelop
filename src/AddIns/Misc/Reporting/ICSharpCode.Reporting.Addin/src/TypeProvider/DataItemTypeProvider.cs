/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.04.2014
 * Time: 20:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSharpCode.Reporting.Addin.DesignableItems;

namespace ICSharpCode.Reporting.Addin.TypeProvider
{
	/// <summary>
	/// Description of DataItemTypeProvider.
	/// </summary>
	class DataItemTypeProvider : TypeDescriptionProvider
	{
		
		public DataItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new DataItemTypeDescriptor(td, instance);
		}
	}
	
	
	
	class DataItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public DataItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();
		
			TypeProviderHelper.AddDefaultProperties(allProperties,props);
			TypeProviderHelper.AddTextBasedProperties(allProperties,props);
	
			PropertyDescriptor prop = props.Find("Text",true);
			allProperties.Add(prop);
			
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
			prop = props.Find("FrameColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("Visible",true);
			allProperties.Add(prop);
			
			prop = props.Find("ColumnName",true);
			allProperties.Add(prop);
			
//			prop = props.Find("BaseTableName",true);
//			allProperties.Add(prop);
			
//			prop = props.Find("DbValue",true);
//			allProperties.Add(prop);
			
			prop = props.Find("NullValue",true);
			allProperties.Add(prop);
			
//			prop = props.Find("Expression",true);
//			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
