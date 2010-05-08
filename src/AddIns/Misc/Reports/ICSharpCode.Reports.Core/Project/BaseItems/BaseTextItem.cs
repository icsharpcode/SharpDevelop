// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// This class is the BaseClass  for all TextBased Items 
/// like <see cref="BaseDataItem"></see> etc.
/// </summary>


namespace ICSharpCode.Reports.Core
{
	public class BaseTextItem : BaseReportItem,IExportColumnBuilder {

		private string text;
		private string dataType;
		private string formatString;
		private StringFormat stringFormat;
		private StringTrimming stringTrimming;
		private ContentAlignment contentAlignment;

		
		#region Constructor
		
		public BaseTextItem():base() {
			this.dataType = "System.String";
			this.stringFormat = StringFormat.GenericTypographic;
			this.contentAlignment = ContentAlignment.TopLeft;
			this.stringTrimming = StringTrimming.None;
		}
		
		#endregion
		
		#region IExportColumnBuilder  implementation
		
		public virtual BaseExportColumn CreateExportColumn(){
			TextStyleDecorator st = this.CreateItemStyle();
			ExportText item = new ExportText(st,false);
			item.Text = this.text;
			return item;
		}
		
		
		protected TextStyleDecorator CreateItemStyle () {
			
			TextStyleDecorator style = new TextStyleDecorator();
			
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			
			style.Font = this.Font;
			
			style.Location = this.Location;
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			
			style.StringFormat = this.stringFormat;
			style.StringTrimming = this.stringTrimming;
			style.ContentAlignment = this.contentAlignment;
			style.FormatString = this.formatString;
			style.DataType = this.dataType;
			
			return style;
		}

		#endregion
		
		
		public override void Render(ReportPageEventArgs rpea) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			base.Render(rpea);
			base.FillBackground(rpea.PrintPageEventArgs.Graphics);
			//Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			Border b = new Border(new BaseLine (this.FrameColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			base.DrawFrame(rpea.PrintPageEventArgs.Graphics,b);
			
			string formated = StandardFormatter.FormatOutput(this.text,this.FormatString,this.DataType,String.Empty);
			
			Print (rpea,formated,base.DrawingRectangle);
			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
		
		
		public override string ToString() {
			return "BaseTextItem";
		}
		
		
		/// <summary>
		/// Standart Function to Draw Strings
		/// </summary>
		/// <param name="e">ReportpageEventArgs</param>
		/// <param name="toPrint">Formatted String toprint</param>
		/// <param name="rectangle">rectangle where to draw the string</param>
	
		protected void Print (ReportPageEventArgs rpea,
		                              string toPrint,
		                              RectangleF rectangle ) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			TextDrawer.DrawString(rpea.PrintPageEventArgs.Graphics,
			                      toPrint,this.Font,
			                      new SolidBrush(this.ForeColor),
			                      rectangle,
			                      this.stringTrimming,this.contentAlignment);
			
			
			rpea.LocationAfterDraw = new Point (this.Location.X + this.Size.Width,
			                                    this.Location.Y + this.Size.Height);
		}
	
		
		public virtual string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public string DataType 
		{
			get {
				if (String.IsNullOrEmpty(this.dataType)) {
					this.dataType = typeof(System.String).ToString();
				}
				return dataType;
			}
			set {
				dataType = value;
			}
		}
		
		
		///<summary>
		/// Formatstring like in MSDN
		/// </summary>

		
		
		public virtual string FormatString {
			get {
				return formatString;
			}
			set {
				formatString = value;
			}
		}
		
		
		public  StringTrimming StringTrimming {
			get {
				return stringTrimming;
			}
			set {
				stringTrimming = value;
			}
		}
		
		
		public virtual System.Drawing.ContentAlignment ContentAlignment {
			get {
				return this.contentAlignment;
			}
			set {
				this.contentAlignment = value;
			}
		}
	

		public virtual StringFormat StringFormat {
			get {
				return TextDrawer.BuildStringFormat (this.StringTrimming,this.ContentAlignment);                                       
			}
		}
		
	}
}

