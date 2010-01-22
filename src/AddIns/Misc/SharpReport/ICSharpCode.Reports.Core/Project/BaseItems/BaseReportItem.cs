// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

/// <summary>
/// This Class is the BaseClass for <see cref="BaseTextItem"></see>
/// and <see cref="BaseGraphicItem"></see>
/// </summary>
namespace ICSharpCode.Reports.Core {
	public class BaseReportItem : IReportItem
	{
		private bool drawBorder;
		private bool visible = true;
		private bool canGrow;
		private bool canShrink;
		
		private Color foreColor;
		private string name;
		private BaseReportItem parent;
		
		private Size size;
		private Point location;
		private Color backColor;
		private int  sectionOffset;
		private Font font;
		
		[Obsolete ("will be deleted")]
		public event EventHandler<BeforePrintEventArgs> ItemPrinting;
		[Obsolete("will be deleted")]
		public event EventHandler<AfterPrintEventArgs> ItemPrinted;
		private RectangleShape backgroundShape = new RectangleShape();
		
		
		public BaseReportItem() 
		{
			this.size = GlobalValues.PreferedSize;
			this.backColor = GlobalValues.DefaultBackColor;
			this.foreColor = Color.Black;
			this.font = GlobalValues.DefaultFont;
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
		
		protected void FillBackground (Graphics  graphics) {
			backgroundShape.FillShape(graphics,
			                new SolidFillPattern(this.BackColor),
			                this.DrawingRectangle);
		}
		
		protected void DrawFrame (Graphics graphics,Border border) {
			if (this.DrawBorder == true) {
				border.DrawBorder(graphics,this.DrawingRectangle);
			}
		}
		
		
		protected Rectangle DrawingRectangle
		{
			get {
				return new Rectangle(this.Parent.Location.X + this.Location.X ,
				                     this.Location.Y + this.SectionOffset,
				                     this.Size.Width,this.Size.Height);
			}
		}
		
	
		#region Properties
		
		//
		public virtual bool Visible 
		{
			get {
				return visible;
			}
			set {
				visible = value;
			}
		}
		
		
		public bool CanGrow {
			get { return canGrow; }
			set { canGrow = value; }
		}
		
		
		public bool CanShrink {
			get { return canShrink; }
			set { canShrink = value; }
		}
		
		
		public virtual bool DrawBorder 
		{
			get {
				return drawBorder;
			}
			set {
				drawBorder = value;
			}
		}
		
		
		public virtual string Name 
		{
			get {
				return name;
			}
			set {
				name = value;
			}			
		}
		
		
		public virtual Size Size 
		{
			get {
				return size;
			}
			set {
				size = value;
			}
		}
		
		
		public virtual Point Location 
		{
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		
		public virtual Color BackColor 
		{
			get {
				return backColor;
			}
			set {
				backColor = value;
			}
		}
		
		
		public virtual int SectionOffset 
		{
			get {
				return sectionOffset;
			}
			set {
				sectionOffset = value;
			}
		}
		
		
		public  BaseReportItem Parent
		{
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}
		

		public virtual Color ForeColor 
		{
			get {
				return foreColor;
			}
			set {
				foreColor = value;
			}
		}
		
		
		public virtual Font Font 
		{
			get {
				return this.font;
			}
			set {
				this.font = value;
			}
		}
			
		#endregion
		
		
		#region IDisposeable

		protected virtual void Dispose(bool disposing)
		{
			if (disposing){
				if (this.font != null){
					this.font = null;
					this.font.Dispose();
				}
			}
			
		}

		#endregion
		
	}
}

