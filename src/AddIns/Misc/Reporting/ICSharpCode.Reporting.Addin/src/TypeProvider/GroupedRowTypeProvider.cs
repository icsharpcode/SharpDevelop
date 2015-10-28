/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.05.2014
 * Time: 17:19
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
	/// Description of GroupedRowTypeProvider.
	/// </summary>
	class GroupedRowTypeProvider : TypeDescriptionProvider
	{
		public GroupedRowTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}

	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new GroupedRowItemTypeDescriptor(td);
		}
	}
	
	
	class GroupedRowItemTypeDescriptor : CustomTypeDescriptor
	{
		public GroupedRowItemTypeDescriptor(ICustomTypeDescriptor parent)
			: base(parent)
		{
		}

		
		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var props = base.GetProperties(attributes);
			var allProperties = new List<PropertyDescriptor>();

			TypeProviderHelper.AddDefaultProperties(allProperties,props);
			
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
		
			prop = props.Find("PageBreakOnGroupChange",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
