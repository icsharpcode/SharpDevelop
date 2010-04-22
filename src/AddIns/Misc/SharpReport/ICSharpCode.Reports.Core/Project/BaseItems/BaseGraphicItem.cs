// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Baseclass for all Graphical Items	
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 31.08.2005 13:24:59
	/// </remarks>
	public class BaseGraphicItem : BaseReportItem {
		
		private int thickness = 1;
		private DashStyle dashStyle = DashStyle.Solid;
		
		public BaseGraphicItem():base() {
		}
		
		protected IGraphicStyleDecorator CreateItemStyle (BaseShape shape) {
			GraphicStyleDecorator style = new GraphicStyleDecorator(shape);
			
			style.Size = this.Size;
			style.Location = this.Location;
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			style.FrameColor = this.FrameColor;
			
			style.Thickness = this.thickness;
			style.DashStyle = this.dashStyle;
			return style;
		}
		
		protected static SizeF MeasureReportItem (IReportItem item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			return new SizeF (item.Size.Width,item.Size.Height);
		}
		
		protected BaseLine Baseline()
		{
			if (this.BackColor == GlobalValues.DefaultBackColor) {
				return new BaseLine (this.ForeColor,this.DashStyle,this.Thickness);
			} else {
				return new BaseLine (this.BackColor,this.DashStyle,this.Thickness);
			}
		}
			
		#region Overrides
		
		public override string ToString()
		{
			return "BaseGraphicItem";
		}
		
		#endregion
		
		
		#region property's
		/// <summary>
		/// Line Thickness of graphical Element
		/// </summary>
		
		public virtual int Thickness {
			get {
				return thickness;
			}
			set {
				thickness = value;
			}
		}
		
	
		public virtual DashStyle DashStyle {
			get {
				return dashStyle;
			}
			set {
				dashStyle = value;
			}
		}
		
		
		[XmlIgnoreAttribute]
		[Browsable(false)]
		public override bool DrawBorder {
			get { return base.DrawBorder; }
			set { base.DrawBorder = value; }
		}
		
		
		[XmlIgnoreAttribute]
		[Browsable(false)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		
		
		#endregion
	}
}
