/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2014
 * Time: 20:18
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
	/// Description of SectionItemTypeProvider.
	/// </summary>
	class SectionItemTypeProvider : TypeDescriptionProvider
	{
		public SectionItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new SectionItemDescriptor(td);
		}
	}
	
	
	class SectionItemDescriptor : CustomTypeDescriptor
	{

		
		public SectionItemDescriptor(ICustomTypeDescriptor parent)
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
			PropertyDescriptor prop = null;
			
//			prop = props.Find("SectionOffset",true);
//			allProperties.Add(prop);
			
//			prop = props.Find("SectionMargin",true);
//			allProperties.Add(prop);
			
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
//			prop = props.Find("PageBreakAfter",true);
//			allProperties.Add(prop);
			
			prop = props.Find("Controls",true);
			allProperties.Add(prop);
			
			prop = props.Find("FrameColor",true);
			allProperties.Add(prop);
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
