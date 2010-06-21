// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;

using ICSharpCode.Reports.Core.old_Exporter;
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
				StandardPrinter.FillBackground(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator,this.DrawingRectangle);
			}
			 
			StandardPrinter.DrawBorder(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator,this.DrawingRectangle);
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


