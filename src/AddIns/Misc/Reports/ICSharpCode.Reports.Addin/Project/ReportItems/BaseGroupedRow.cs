/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.08.2010
 * Time: 19:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.Designer;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of GroupedRow.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.GroupedRowDesigner))]
	public class BaseGroupedRow:BaseRowItem
	{
		public BaseGroupedRow()
		{
			TypeDescriptor.AddProvider(new GroupedRowTypeProvider(), typeof(BaseGroupedRow));
		}
		
		[Category("Behavior")]
		public bool PageBreakOnGroupChange {get;set;}
	}
	
	
	internal class GroupedRowTypeProvider : TypeDescriptionProvider
	{
		public GroupedRowTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}

	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new GroupedRowItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class GroupedRowItemTypeDescriptor : CustomTypeDescriptor
	{
		public GroupedRowItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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

			DesignerHelper.AddDefaultProperties(allProperties,props);
			
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
		
			prop = props.Find("AlternateBackColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("ChangeBackColorEveryNRow",true);
			allProperties.Add(prop);
			
			prop = props.Find("PageBreakOnGroupChange",true);
			allProperties.Add(prop);
			
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
