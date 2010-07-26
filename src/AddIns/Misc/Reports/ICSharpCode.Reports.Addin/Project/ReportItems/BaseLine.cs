/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 04.12.2007
 * Zeit: 09:00
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseLine.
	/// </summary>
	[Designer(typeof(ICSharpCode.Reports.Addin.LineDesigner))]
	public class BaseLineItem:AbstractItem
	{
		private Point fromPoint;
		private Point toPoint;
		private LineCap startLineCap;
		private LineCap endLineCap;
		private DashCap dashLineCap;
		private DashStyle dashStyle;
		private float thickness;
		
		public BaseLineItem()
		{
			this.thickness = 1;
			this.dashStyle = DashStyle.Solid;
			this.Size = new Size(50,10);
			TypeDescriptor.AddProvider(new LineItemTypeProvider(), typeof(BaseLineItem));
			this.SetStartEndPoint();
		}
		
		private void SetStartEndPoint ()
		{
			this.fromPoint = new Point(ClientRectangle.Left + 10,ClientRectangle.Height / 2);
			this.toPoint = new Point(ClientRectangle.Left + ClientRectangle.Width - 10,
			                         ClientRectangle.Height/ 2);
			this.Invalidate();
		}
		
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			base.OnPaint(e);
			Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Pen p = new Pen(this.ForeColor,this.Thickness)) {
				p.SetLineCap(this.StartLineCap,this.EndLineCap,this.DashLineCap);
				graphics.DrawLine(p,this.fromPoint,this.toPoint);
			}
		}
		
		
		public Point FromPoint {
			get { return fromPoint; }
			set {
				Point x = value;
				if (!this.ClientRectangle.Contains(x)) {
					this.fromPoint = new Point(x.X - this.Location.X,
					                           x.Y - this.Location.Y);
				} else {
					this.fromPoint = x;
				}
				this.Invalidate();
			}
		}
		
		
		public Point ToPoint {
			get { return toPoint; }
			set {
				Point x = value;
				if (!this.ClientRectangle.Contains(x)) {
					this.toPoint = new Point(x.X - this.Location.X,
					                         x.Y - this.Location.Y);
				}
				else {
					this.toPoint = x;
				}
				this.Invalidate();
			}
		}
		
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("LineStyle")]
		public DashStyle DashStyle {
			get { return dashStyle; }
			set {
				dashStyle = value;
				this.Invalidate();
			}
		}
		
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Thickness of Line")]
		public float Thickness {
			get { return thickness; }
			set {
				thickness = value;
				this.Invalidate();
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("LineCap at Startposition")]
		public LineCap StartLineCap {
			get { return startLineCap; }
			set {
				startLineCap = value;
				this.Invalidate();
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Linecap at Endposition")]
		public LineCap EndLineCap {
			get { return endLineCap; }
			set {
				endLineCap = value;
				this.Invalidate();
			}
		}
		
		[Browsable(true),
		 Category("Appearance"),
		 Description("Dashlinecap")]
		public DashCap DashLineCap {
			get { return dashLineCap; }
			set {
				dashLineCap = value;
				this.Invalidate();
			}
		}
		
	}
	
	
	internal class LineItemTypeProvider : TypeDescriptionProvider
	{
		public LineItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
//		public LineItemTypeProvider(TypeDescriptionProvider parent): base(parent)
//		{
//			
//		}

		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType,instance);
			return new LineItemTypeDescriptor(td, instance);
		}
	}
	
	internal class LineItemTypeDescriptor : CustomTypeDescriptor
	{
//		private BaseTextItem instance;
		
		public LineItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
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
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();
			
			DesignerHelper.AddDefaultProperties(allProperties,props);
			
			PropertyDescriptor prop = null;
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("FromPoint",true);
			allProperties.Add(prop);
			
			prop = props.Find("ToPoint",true);
			allProperties.Add(prop);
			
			prop = props.Find("StartLineCap",true);
			allProperties.Add(prop);
			
			prop = props.Find("EndLineCap",true);
			allProperties.Add(prop);
			
			prop = props.Find("dashLineCap",true);
			allProperties.Add(prop);
			
			prop = props.Find("DashStyle",true);
			allProperties.Add(prop);
			
			prop = props.Find("Thickness",true);
			allProperties.Add(prop);
			
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
}
