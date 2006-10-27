/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 03.03.2006
 * Time: 09:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SharpReportCore.Exporters;

namespace SharpReportCore{
	/// <summary>
	/// Description of BaseRowItem.
	/// </summary>
	
	public class RowItem:BaseReportItem,IContainerItem,IExportColumnBuilder{
		private string tableName;
		
		private ReportItemCollection items;
		private Padding padding;
		private Color alternateBackColor;
		private int changeBackColorEveryNRow;
		private RectangleShape shape = new RectangleShape();
		
		
		public RowItem():this (String.Empty){
		}
		
		
		public RowItem(string tableName){
			this.tableName = tableName;
			this.padding = new Padding(5);
//			this.Items.Added += OnAdded;
		}
		
		/*
		void OnAdded (object sender, CollectionItemEventArgs<IItemRenderer> e){			
			System.Console.WriteLine("");
			System.Console.WriteLine("RowItem:OnAdded did we use this function???");
		}
		*/
		#region overrides
		
		#region IExportColumnBuilder  implementation
		
		public BaseExportColumn CreateExportColumn(Graphics graphics){	
			BaseStyleDecorator st = this.CreateItemStyle(graphics);
			
			ExportContainer item = new ExportContainer(st);
			return item;
		}

		protected BaseStyleDecorator CreateItemStyle (Graphics g) {
			BaseStyleDecorator style = new BaseStyleDecorator();
			
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			style.Location = this.Location;
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			return style;
		}
		
		#endregion
		
		protected RectangleF PrepareRectangle () {
			SizeF measureSize = new SizeF ((SizeF)this.Size);
			RectangleF rect = base.DrawingRectangle (measureSize);
			return rect;
		}
		
		
		public override void Render(ReportPageEventArgs rpea){
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}

			base.Render(rpea);
			RectangleF rect = PrepareRectangle ();
			
			shape.FillShape(rpea.PrintPageEventArgs.Graphics,
			                new SolidFillPattern(this.BackColor),
			                rect);
			
			if (base.DrawBorder == true) {
				shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
				                 new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1),
				                 rect);
			}


			foreach (BaseReportItem childItem in this.items) {
				Point loc = new Point (childItem.Location.X,childItem.Location.Y);
				
				childItem.Location = new Point(this.Location.X + childItem.Location.X,
				                               this.SectionOffset + childItem.Location.Y);
			
				childItem.Render (rpea);
				childItem.Location = new Point(loc.X,loc.Y);
			}
//			System.Console.WriteLine("--End of RowItem");
//			System.Console.WriteLine("");
			base.NotiyfyAfterPrint (rpea.LocationAfterDraw);
		}
		
		public override string ToString(){
			return this.GetType().Name;
		}
		
		#endregion
		
	
		#region properties
		
		
		
		[Category("Appearance"),
		 Description("Change the Backcolor on every 'N' Row")]
		public Color AlternateBackColor {
			get {
				return this.alternateBackColor;
			}
			set {
				this.alternateBackColor = value;
				base.NotifyPropertyChanged("SecondaryBackColor");
			}
		}
		
		[Category("Appearance")]
		public int ChangeBackColorEveryNRow {
			get {
				return changeBackColorEveryNRow;
			}
			set {
				changeBackColorEveryNRow = value;
				base.NotifyPropertyChanged("ChangeBackColorEveryNRow");
				                         
			}
		}
		
		#endregion
		
		#region IContainerControl
		
		public Padding Padding {
			get {
				return padding;
			}
			set {
				padding = value;
				base.NotifyPropertyChanged("Padding");
			}
		}
		
		public bool IsValidChild(BaseReportItem childControl){
			BaseReportItem bdi = childControl as BaseDataItem;
			if (bdi != null) {
				return true;
			} else {
				return false;
			}
		}
		
		public ReportItemCollection Items{
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
		#endregion
		/*
		#region IDisposable
		public override void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~RowItem()
		{
			Dispose(false);
		}
		
		protected override void Dispose(bool disposing){
			try {
				if (disposing) {
				// Free other state (managed objects).
				if (this.baseDataItemCollection != null) {
					this.BaseDataItemCollection.Clear();
					this.baseDataItemCollection = null;
				}
			}
			} finally {
				base.Dispose();
			}
		}
		#endregion
		*/
		
	}
}
