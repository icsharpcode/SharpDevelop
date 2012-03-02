// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;

/// <summary>
/// This Class is the BaseClass for <see cref="BaseTextItem"></see>
/// and <see cref="BaseGraphicItem"></see>
/// </summary>
namespace ICSharpCode.Reports.Core {
	public class BaseReportItem : IReportItem
	{
	
		[Obsolete ("will be deleted")]
		public event EventHandler<BeforePrintEventArgs> ItemPrinting;
		[Obsolete("will be deleted")]
		public event EventHandler<AfterPrintEventArgs> ItemPrinted;
		
		
		public BaseReportItem() 
		{
			this.Size = GlobalValues.PreferedSize;
			this.BackColor = GlobalValues.DefaultBackColor;
			this.FrameColor = Color.Black;
			this.ForeColor = Color.Black;
			this.Font = GlobalValues.DefaultFont;
		}
		
		
		#region EventHandling
		
		protected void NotifyAfterPrint (PointF afterPrintLocation)
		{
			if (this.ItemPrinted != null) {
				AfterPrintEventArgs rea = new AfterPrintEventArgs (afterPrintLocation);
				ItemPrinted(this, rea);
			}
		}
		
		private void NotifyBeforePrint ()
		{
			if (this.ItemPrinting != null) {
				BeforePrintEventArgs ea = new BeforePrintEventArgs ();
				ItemPrinting (this,ea);
			}
		}
		
		#endregion
		
		#region overrides
		
		public virtual void Render(ReportPageEventArgs rpea)
		{
			this.NotifyBeforePrint();
		}
		
		#endregion
		
		public BaseStyleDecorator BaseStyleDecorator
		{
			get {
				return this.GetBaseStyleDecorator();
			}
		}
		
		
		private BaseStyleDecorator GetBaseStyleDecorator()
		{
			BaseStyleDecorator bsd = new BaseStyleDecorator();
			bsd.DrawBorder = this.DrawBorder;
			bsd.BackColor = this.BackColor;
			bsd.ForeColor = this.ForeColor;
			bsd.FrameColor = this.FrameColor;
			bsd.Location = this.Location;
			bsd.Size = this.Size;
			bsd.DisplayRectangle = this.DisplayRectangle;
			
			return bsd;
		}
		
		
		protected void FillBackground (Graphics  graphics)
		{
			StandardPrinter.FillBackground(graphics,this.BaseStyleDecorator);
		}
		
		
		protected void DrawFrame (Graphics graphics,Border border) {
			if (this.DrawBorder == true) {
				border.DrawBorder(graphics,this.DisplayRectangle);
			}
		}
		
		
		protected virtual Rectangle DisplayRectangle
		{
			get {
				return new Rectangle( this.Location.X ,
				                     this.Location.Y + this.SectionOffset,
				                     this.Size.Width,this.Size.Height);
			}
		}
		
		
		#region Properties
		public bool Visible {get;set;}
		
//		public  bool VisibleInReport {get;set;}
		
		public bool CanGrow {get;set;}
			
		public bool CanShrink {get;set;}
			
		public virtual bool DrawBorder {get;set;}
		
		public virtual string Name {get;set;}
		
		public virtual Size Size {get;set;}
		
		public virtual Point Location {get;set;}
		
		public virtual int SectionOffset {get;set;}
		
		public  BaseReportItem Parent{get;set;}
		
		public virtual Color ForeColor {get;set;}
	
		public virtual Color BackColor{get;set;}
		
		public Color FrameColor{get;set;}
		
		public virtual Font Font {get;set;}
		
		#endregion
		
		
		#region IDisposeable

		protected virtual void Dispose(bool disposing)
		{
			if (disposing){
				if (this.Font != null){
					this.Font = null;
					this.Font.Dispose();
				}
			}
			
		}

		#endregion
		
	}
}
