/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 18:02
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
	/// Description of LineItemTypeProvider.
	/// </summary>
	class LineItemTypeProvider : TypeDescriptionProvider
	{
		public LineItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new LineItemTypeDescriptor(td);
		}
	}
	
	
	
	class LineItemTypeDescriptor : CustomTypeDescriptor
	{

		
		public LineItemTypeDescriptor(ICustomTypeDescriptor parent)
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
			
			prop = props.Find("DashStyle",true);
			allProperties.Add(prop);
			
			prop = props.Find("Thickness",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
