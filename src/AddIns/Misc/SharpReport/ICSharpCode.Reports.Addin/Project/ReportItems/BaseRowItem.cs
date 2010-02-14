/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 28.01.2008
 * Zeit: 16:41
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseRowItem.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.RowItemDesigner))]
	public class BaseRowItem:AbstractItem
	{

		private Color alternateBackColor;
		private int changeBackColorEveryNRow;
		private RectangleShape backgroundShape = new RectangleShape();
		
		public BaseRowItem():base()
		{
			Size s = new Size((GlobalValues.PreferedSize.Width * 3) + 10,
			                     GlobalValues.PreferedSize.Height + 10);
			base.DefaultSize = s;
			base.Size = s;
			base.BackColor = Color.White;
			TypeDescriptor.AddProvider(new RowItemTypeProvider(), typeof(BaseRowItem));
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
		
		#region Properties
		
		public Color AlternateBackColor {
			get { return alternateBackColor; }
			set { alternateBackColor = value; }
		}
	
		
		public int ChangeBackColorEveryNRow {
			get { return changeBackColorEveryNRow; }
			set { changeBackColorEveryNRow = value; }
		}
		
		public RectangleShape BackgroundShape {
			get { return backgroundShape; }
			set { backgroundShape = value; }
		}
		
	
		#endregion
		
	}
	
	internal class RowItemTypeProvider : TypeDescriptionProvider
	{
		public RowItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public RowItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//		
//		}

	
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new RowItemTypeDescriptor(td, instance);
		}
	}
	
	
	internal class RowItemTypeDescriptor : CustomTypeDescriptor
	{
		public RowItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			/*
			prop = props.Find("Padding",true);
			allProperties.Add(prop);
			*/
			prop = props.Find("AlternateBackColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("ChangeBackColorEveryNRow",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
