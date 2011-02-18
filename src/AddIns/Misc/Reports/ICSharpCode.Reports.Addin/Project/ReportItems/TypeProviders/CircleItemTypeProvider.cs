/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2011
 * Time: 19:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using ICSharpCode.Reports.Addin.Designer;


namespace ICSharpCode.Reports.Addin.TypeProviders
{
	
	internal class CircleItemTypeProvider : TypeDescriptionProvider
	{
		public CircleItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new CircleItemTypeDescriptor(td, instance);
		}
	}
	
	
	
	internal class CircleItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public CircleItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			System.Collections.Generic.List<PropertyDescriptor> allProperties = new System.Collections.Generic.List<PropertyDescriptor>();
			
			DesignerHelper.AddDefaultProperties(allProperties,props);
			DesignerHelper.AddGraphicProperties(allProperties,props);
			PropertyDescriptor prop = null;

			prop = props.Find("Controls",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}

