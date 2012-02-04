// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.Reports.Addin.Designer;

namespace ICSharpCode.Reports.Addin.TypeProviders
{
	/// <summary>
	/// Description of RectangleItemTypeProvider.
	/// </summary>
	internal class RectangleItemTypeProvider : TypeDescriptionProvider
	{
		public RectangleItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new RectangleItemTypeDescriptor(td, instance);
		}
	}
	
	
	
	internal class RectangleItemTypeDescriptor : CustomTypeDescriptor
	{
	
		public RectangleItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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

			prop = props.Find("CornerRadius",true);
			allProperties.Add(prop);
			
			prop = props.Find("Controls",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
