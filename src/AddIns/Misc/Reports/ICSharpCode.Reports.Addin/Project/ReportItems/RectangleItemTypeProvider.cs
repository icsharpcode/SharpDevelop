/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 23.04.2009
 * Zeit: 19:38
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.ComponentModel;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of RectangleItemTypeProvider.
	/// </summary>
	internal class RectangleItemTypeProvider : TypeDescriptionProvider
	{
		public RectangleItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public RectangleItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//			
//		}

		
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
			
			PropertyDescriptor prop = null;
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("DrawBorder",true);
			allProperties.Add(prop);
			
			prop = props.Find("DashStyle",true);
			allProperties.Add(prop);
			
			prop = props.Find("Thickness",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
