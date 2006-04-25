/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 03.03.2006
 * Time: 09:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SharpReportCore{
	/// <summary>
	/// Description of BaseRowItem.
	/// </summary>
	
	public class RowItem:BaseReportItem,IContainerItem{
		private string tableName;
		
		private ReportItemCollection items;
		private Padding padding;
		private Color alternateBackColor;
		private int changeBackColorEveryNRow;
		
		public RowItem():this (String.Empty){
		}
		
		
		public RowItem(string tableName){
			this.tableName = tableName;
			this.Items.Added += OnAdded;
		}
		
		void OnAdded (object sender, CollectionItemEventArgs<IItemRenderer> e){			
			System.Console.WriteLine("");
			System.Console.WriteLine("RowItem:OnAdded");
		}
		
		#region overrides
		private void Decorate (ReportPageEventArgs rpea,Rectangle border) {
			using (SolidBrush brush = new SolidBrush(base.BackColor)) {
				rpea.PrintPageEventArgs.Graphics.FillRectangle(brush,border);
			}
			if (base.DrawBorder == true) {
				using (Pen pen = new Pen(Color.Black, 1)) {
					rpea.PrintPageEventArgs.Graphics.DrawRectangle (pen,border);
				}
			}
		}
		
		protected RectangleF PrepareRectangle (ReportPageEventArgs e) {
			SizeF measureSize = new SizeF ((SizeF)this.Size);
			RectangleF rect = base.DrawingRectangle (e,measureSize);
			return rect;
		}
		
		
		public override void Render(ReportPageEventArgs rpea){
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			System.Console.WriteLine("");
			System.Console.WriteLine("--Start Row Item");
			base.Render(rpea);
			RectangleF rect = PrepareRectangle (rpea);
			
			Decorate (rpea,System.Drawing.Rectangle.Ceiling (rect));

			foreach (BaseReportItem childItem in this.items) {
				Point loc = new Point (childItem.Location.X,childItem.Location.Y);
				
				childItem.Location = new Point(this.Location.X + childItem.Location.X,
				                               this.SectionOffset + childItem.Location.Y);
			
				childItem.Render (rpea);
				childItem.Location = new Point(loc.X,loc.Y);
			}
			System.Console.WriteLine("--End of RowItem");
			System.Console.WriteLine("");
			base.NotiyfyAfterPrint (rpea.LocationAfterDraw);
		}
		
		public override string ToString(){
			return "RowItem";
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
