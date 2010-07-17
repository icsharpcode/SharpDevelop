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
using ICSharpCode.Reports.Core.Exporter;


/// <summary>
/// This Class handles Lines
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 28.09.2005 23:46:19
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	public class BaseLineItem : BaseGraphicItem,IExportColumnBuilder {
		
		private LineShape shape  = new LineShape();
		private Point fromPoint;
		private Point toPoint;
		private LineCap startLineCap;
		private LineCap endLineCap;
		private DashCap dashLineCap;
		
		#region Constructor
		
		public BaseLineItem():base() 
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder  implementation
		
		public BaseExportColumn CreateExportColumn()
		{
			LineDecorator style = this.CreateLineShape();
			ExportGraphic item = new ExportGraphic(style,false);
			return item as ExportGraphic;
		}
	
		
		private LineDecorator CreateLineShape ()
		{
			LineDecorator ld = new LineDecorator(this.shape);
			ld.Size = this.Size;
			ld.Location = this.Location;
			ld.BackColor = this.BackColor;
			ld.ForeColor = this.ForeColor;
			
			ld.Thickness = base.Thickness;
			ld.DashStyle = base.DashStyle;
			ld.From = this.fromPoint;
			ld.To = this.toPoint;
			return ld;
		}
		
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea)
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render (rpea);
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 new BaseLine (this.ForeColor,base.DashStyle,
			                               base.Thickness,
			                               this.startLineCap,
			                               this.endLineCap,
			                               this.dashLineCap),
			                 new Point(base.DrawingRectangle.Left + this.fromPoint.X,
			                           this.FromPoint.Y + base.DrawingRectangle.Top),
			                 new Point (base.DrawingRectangle.Left + this.ToPoint.X,
			                            this.ToPoint.Y + base.DrawingRectangle.Top));
			
		}
		
		
		public override string ToString() 
		{
			return "BaseLineItem";
		}
		
		
		public virtual Point FromPoint {
			get { 
				return this.fromPoint;
			}
			set { 
				fromPoint = value;
			}
		}
		
		
		public virtual Point ToPoint {
			get {
				return this.toPoint;
			}
			set {
				toPoint = value;
			}
		}
		
		
		public LineCap StartLineCap {
			get { return startLineCap; }
			set { startLineCap = value; }
		}
		
		public LineCap EndLineCap {
			get { return endLineCap; }
			set { endLineCap = value; }
		}
		
		public DashCap DashLineCap {
			get { return dashLineCap; }
			set { dashLineCap = value; }
		}
	}
}
