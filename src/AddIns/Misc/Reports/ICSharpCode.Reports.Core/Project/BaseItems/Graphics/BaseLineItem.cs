// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			LineDecorator decorator = new LineDecorator(this.shape);
			decorator.Size = this.Size;
			decorator.Location = this.Location;
			decorator.BackColor = this.BackColor;
			decorator.ForeColor = this.ForeColor;
			
			decorator.Thickness = base.Thickness;
			decorator.DashStyle = base.DashStyle;
			decorator.From = this.fromPoint;
			decorator.To = this.toPoint;
			return decorator;
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
			                 new Point(base.DisplayRectangle.Left + this.fromPoint.X,
			                           this.FromPoint.Y + base.DisplayRectangle.Top),
			                 new Point (base.DisplayRectangle.Left + this.ToPoint.X,
			                            this.ToPoint.Y + base.DisplayRectangle.Top));
			
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
