/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2011
 * Time: 19:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSharpCode.Reports.Addin.Designer;

namespace ICSharpCode.Reports.Addin.TypeProviders
{
	internal class LineItemTypeProvider : TypeDescriptionProvider
	{
		public LineItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new LineItemTypeDescriptor(td, instance);
		}
	}
	
	
	
	internal class LineItemTypeDescriptor : CustomTypeDescriptor
	{

		
		public LineItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			
			DesignerHelper.AddDefaultProperties(allProperties,props);
			
			PropertyDescriptor prop = null;
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("FromPoint",true);
			allProperties.Add(prop);
			
			prop = props.Find("ToPoint",true);
			allProperties.Add(prop);
			
			prop = props.Find("StartLineCap",true);
			allProperties.Add(prop);
			
			prop = props.Find("EndLineCap",true);
			allProperties.Add(prop);
			
			prop = props.Find("dashLineCap",true);
			allProperties.Add(prop);
			
			prop = props.Find("DashStyle",true);
			allProperties.Add(prop);
			
			prop = props.Find("Thickness",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
