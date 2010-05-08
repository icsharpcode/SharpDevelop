// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core{
	/// <summary>
	/// Description of BaseRowItem.
	/// </summary>
	
	public class BaseRowItem:BaseReportItem,ISimpleContainer,IExportColumnBuilder
	{
		
		private ReportItemCollection items;
		private Color alternateBackColor;
		private int changeBackColorEveryNRow;
		
		
		public BaseRowItem():base()
		{
			base.Visible = true;
		}
		
		#region IExportColumnBuilder  implementation
		
		public BaseExportColumn CreateExportColumn()
		{
			BaseStyleDecorator st = this.CreateItemStyle();
			ExportContainer item = new ExportContainer(st);
			return item;
		}

		protected BaseStyleDecorator CreateItemStyle () 
		{
			BaseStyleDecorator style = new BaseStyleDecorator();
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			style.FrameColor = this.FrameColor;
			
			/*
			if (this.Parent != null) {
				Rectangle rect = base.DrawingRectangle;
				style.Location = new Point(rect.Left,this.Location.Y);
			} else {
				style.Location = this.Location;
			}
			*/
			style.Location = this.Location;
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			return style;
		}
		
		#endregion
		
		
		#region overrides
		
		public override void Render(ReportPageEventArgs rpea)
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			Point point = this.Location;
			base.Render(rpea);
			
			if (this.BackColor != GlobalValues.DefaultBackColor) {
				base.FillBackground(rpea.PrintPageEventArgs.Graphics);       
			}
			 
			
			Border b = new Border(new BaseLine (this.FrameColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			
			base.DrawFrame (rpea.PrintPageEventArgs.Graphics,b);
			
			this.Location = new Point(base.DrawingRectangle.Left,this.Location.Y);
			
			if ((this.items != null) && (this.items.Count > 0)) {
				foreach (BaseReportItem childItem in this.items) {
					childItem.Parent = this;
					
					Point saveloc = new Point (childItem.Location.X,childItem.Location.Y);
					
					childItem.Location = new Point(childItem.Location.X,
					                               base.DrawingRectangle.Top + childItem.Location.Y);
					
					
					if (this.BackColor != GlobalValues.DefaultBackColor) {
							childItem.BackColor = this.BackColor;
					}
					childItem.Render (rpea);
					childItem.Location = saveloc;
				}
			}
			this.Location = point;
			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
		
		
		public override string ToString()
		{
			return this.GetType().Name;
		}
		
		#endregion
		
	
		#region properties
		
		public Color AlternateBackColor {
			get {return this.alternateBackColor;}
			set {this.alternateBackColor = value;}
		}
		
		
		public int ChangeBackColorEveryNRow {
			get {
				return changeBackColorEveryNRow;
			}
			set {
				changeBackColorEveryNRow = value;
			}
		}
		
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		
		#endregion
		
		
		#region IContainerControl
		
		public ReportItemCollection Items
		{
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
		
		#endregion
	}
}


