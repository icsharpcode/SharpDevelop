/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2011
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSharpCode.Reports.Addin.Designer;


namespace ICSharpCode.Reports.Addin.TypeProviders
{
	internal class AbstractItemTypeProvider : TypeDescriptionProvider {
		public AbstractItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
		public AbstractItemTypeProvider(TypeDescriptionProvider parent): base(parent)
		{
		}

		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType, instance);
			return new AbstractItemTypeDescriptor(td, instance);
		}
	}
	
	internal class AbstractItemTypeDescriptor : CustomTypeDescriptor
	{
//		private AbstractItem _instance;
		
		public AbstractItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
//			_instance = instance as AbstractItem;
		}

		

		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();

			foreach (PropertyDescriptor p in props)
			{
				allProperties.Add(p);
			}
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
