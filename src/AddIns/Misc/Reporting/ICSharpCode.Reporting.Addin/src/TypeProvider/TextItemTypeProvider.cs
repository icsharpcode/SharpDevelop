/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.03.2014
 * Time: 20:38
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
	/// Description of TextItemTypeProvider.
	/// </summary>
	class TextItemTypeProvider : TypeDescriptionProvider
	{
		public TextItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new TextItemTypeDescriptor(td, instance);
		}
	}
	
	
	class TextItemTypeDescriptor : CustomTypeDescriptor
	{
//		BaseTextItem instance;
		
		public TextItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			var props = base.GetProperties(attributes);
			var allProperties = new List<PropertyDescriptor>();

			TypeProviderHelper.AddDefaultProperties(allProperties,props);
			
			TypeProviderHelper.AddTextBasedProperties(allProperties,props);
			
			var prop = props.Find("Text", true);
			allProperties.Add(prop);
		
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
			prop = props.Find("FrameColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("Visible",true);
			allProperties.Add(prop);
			
			prop = props.Find("Expression",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
