/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.03.2014
 * Time: 20:05
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
	/// Description of AbstractItemTypeProvider.
	/// </summary>

	class AbstractItemTypeProvider : TypeDescriptionProvider {
		public AbstractItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType, instance);
			return new AbstractItemTypeDescriptor(td);
		}
	}
	
	class AbstractItemTypeDescriptor : CustomTypeDescriptor
	{
		
		public AbstractItemTypeDescriptor(ICustomTypeDescriptor parent)
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

			foreach (PropertyDescriptor p in props)
			{
				allProperties.Add(p);
			}
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
