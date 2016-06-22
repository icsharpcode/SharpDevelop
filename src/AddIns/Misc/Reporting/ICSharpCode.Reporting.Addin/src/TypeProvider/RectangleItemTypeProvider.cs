/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.04.2014
 * Time: 17:57
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
	/// Description of RectangleItemTypeProvider.
	/// </summary>
	class RectangleItemTypeProvider: TypeDescriptionProvider
	{
		public RectangleItemTypeProvider():  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new RectangleItemTypeDescriptor(td);
		}
	}
	
	
	class RectangleItemTypeDescriptor : CustomTypeDescriptor
	{
	
		public RectangleItemTypeDescriptor(ICustomTypeDescriptor parent)
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
			TypeProviderHelper.AddGraphicProperties(allProperties,props);
			
			PropertyDescriptor prop = null;

			prop = props.Find("CornerRadius",true);
			allProperties.Add(prop);
			
			prop = props.Find("Controls",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
