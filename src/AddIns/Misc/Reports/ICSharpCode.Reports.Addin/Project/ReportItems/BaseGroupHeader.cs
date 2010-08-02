/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.07.2010
 * Time: 19:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Addin.Designer;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseGroupHeader.
	/// </summary>
	/// 
	
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.GroupHeaderDesigner))]
	
	public class BaseGroupHeader:AbstractItem
	{
		public BaseGroupHeader():base()
		{
			TypeDescriptor.AddProvider(new GroupItemTypeProvider(), typeof(BaseGroupHeader));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw (e.Graphics);
		}
		
		
		
		
		public override void Draw(System.Drawing.Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Brush b = new SolidBrush(this.BackColor)){
				graphics.FillRectangle(b, base.DrawingRectangle);
			}
			
			base.DrawControl(graphics,base.DrawingRectangle);
		}
	}
	
		internal class GroupItemTypeProvider : TypeDescriptionProvider
	{
		public GroupItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public RowItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//		
//		}

	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new GroupItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class GroupItemTypeDescriptor : CustomTypeDescriptor
	{
		public GroupItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			/*
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
			*/
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
