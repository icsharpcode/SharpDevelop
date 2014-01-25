// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;

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
		}
		
		#region IExportColumnBuilder  implementation
		
		public IBaseExportColumn CreateExportColumn()
		{
			BaseStyleDecorator st = this.CreateItemStyle();
			return  new ExportContainer(st);
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
			
			StandardPrinter.AdjustBackColor(this);
		
			StandardPrinter.FillBackground(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator);
			
			StandardPrinter.DrawBorder(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator);
			
			rpea.LocationAfterDraw = new Point(rpea.LocationAfterDraw.X,this.Location.Y + this.Size.Height);
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
