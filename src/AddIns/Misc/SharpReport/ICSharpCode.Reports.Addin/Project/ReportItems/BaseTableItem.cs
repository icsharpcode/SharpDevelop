/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 23.09.2008
 * Zeit: 19:38
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseTableItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.TableDesigner))]
	public class BaseTableItem:AbstractItem
	{
		
		
		public BaseTableItem():base()
		{
			Size s = new Size((GlobalValues.PreferedSize.Width * 3) + 10,
			                     GlobalValues.PreferedSize.Height * 2 + 10);
			base.DefaultSize = s;
			base.Size = s;
			base.BackColor = Color.White;
			TypeDescriptor.AddProvider(new TableItemTypeProvider(), typeof(BaseTableItem));
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			this.Draw (e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
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
	
	
	internal class TableItemTypeProvider : TypeDescriptionProvider
	{
		public TableItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public TableItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//		
//		}

	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new TableItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class TableItemTypeDescriptor : CustomTypeDescriptor
	{
		public TableItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
