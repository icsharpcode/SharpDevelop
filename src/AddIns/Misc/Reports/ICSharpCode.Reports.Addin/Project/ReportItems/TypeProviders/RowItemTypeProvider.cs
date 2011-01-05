/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2011
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

using ICSharpCode.Reports.Addin.Designer;

namespace ICSharpCode.Reports.Addin.TypeProviders
{
	
	internal class RowItemTypeProvider : TypeDescriptionProvider
	{
		public RowItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new RowItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class RowItemTypeDescriptor : CustomTypeDescriptor
	{
		public RowItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("Visible",true);
			allProperties.Add(prop);
			
			prop = props.Find("FrameColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("Controls",true);
			allProperties.Add(prop);
	
			prop = props.Find("AlternateBackColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("ChangeBackColorEveryNRow",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
