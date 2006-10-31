/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 16.10.2006
 * Time: 22:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

using SharpReportCore.Exporters;

namespace SharpReportCore.ReportViewer
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class PreviewControl
	{
		private PageSettings pageSettings;
		private float zoom;
		private Bitmap bitmap;
		private PagesCollection pages;
		private int pageNumber;
	
		private Rectangle PageRectangle () {
			
					return new Rectangle (this.pageSettings.Margins.Left,
			                      this.pageSettings.Margins.Top,
			                      this.pageSettings.Bounds.Width - this.pageSettings.Margins.Left - this.pageSettings.Margins.Right,
			                      this.pageSettings.Bounds.Height - this.pageSettings.Margins.Top - this.pageSettings.Margins.Bottom);
			
		
		}
	
		
		private void AdjustDrawArea() {
			if (this.pageSettings != null) {
				this.panel2.ClientSize = new Size((int)(this.pageSettings.PaperSize.Width * this.zoom),
				                                  (int)(this.pageSettings.PaperSize.Height * this.zoom));
			}
			
			
			if (this.pages != null) {
				this.bitmap = this.BuildBitmap(pages[this.pageNumber]);
			
			}
			this.Invalidate(true);
			
		}
		
		
		public PreviewControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			InitZoomCombo ();
	
		}
		
		private void InitZoomCombo (){
			this.toolStripComboBox1.Items.Add("100%");
			this.toolStripComboBox1.Items.Add("75%");
			this.toolStripComboBox1.Items.Add("50%");
			this.toolStripComboBox1.SelectedIndex = 0;
		}
		
		void Panel2Paint(object sender, System.Windows.Forms.PaintEventArgs e){
			e.Graphics.Clear(this.panel2.BackColor);
			
			if (this.bitmap != null) {
				e.Graphics.DrawImage(this.bitmap,0,0);
			}
		}
		
		
		private void DrawItems (Graphics graphics,ExporterCollection<BaseExportColumn> items) {
			
			foreach (SharpReportCore.Exporters.BaseExportColumn baseExportColumn in items) {
				
				if (baseExportColumn != null) {
					ExportContainer container = baseExportColumn as ExportContainer;
					if (container == null) {
						baseExportColumn.DrawItem(graphics);
					} else {
						container.DrawItem(graphics);
						DrawItems(graphics,container.Items);
					}
					
				}
			}
		}
		
		private Bitmap BuildBitmap (SinglePage page) {
			System.Console.WriteLine("BuildBitmap started");
			System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
			s.Start();
			
			Bitmap bm = new Bitmap(this.panel2.ClientSize.Width,this.panel2.ClientSize.Height,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			using (Graphics gr = Graphics.FromImage(bm)) {

				// Reset Transform to org. Value
				gr.ScaleTransform(1F,1F);
				gr.Clear(this.panel2.BackColor);
				gr.ScaleTransform(this.zoom,this.zoom);

				DrawItems (gr,page.Items);
			}
			s.Stop();
			TimeSpan ts = s.Elapsed;
			System.Console.WriteLine("BuildBitmap finished {0}:{1} Seconds",ts.Seconds,ts.Milliseconds);
				System.Console.WriteLine("");
			return bm;
	
		}
		
		private void UpdateToolStrip () {
			string str = String.Empty;
			if (this.pages != null) {
				 str = String.Format (CultureInfo.CurrentCulture,
			                            "Page {0} of {1}",this.pageNumber +1,this.pages.Count);
				
			}
			this.toolStripTextBox1.Text = str;
		}
		
		private void ShowPages () {
			if (this.pageNumber < this.pages.Count) {
				SinglePage sp = pages[this.pageNumber];
				if (this.bitmap != null) {
					this.bitmap.Dispose();
				}
				this.bitmap = this.BuildBitmap(sp);				
				this.Invalidate(true);
				
			} 
			this.UpdateToolStrip();
		}
		
		
		void ToolStripComboBox1SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (this.toolStripComboBox1.SelectedIndex) {
				case 0:
					this.zoom = (float)1.0;
					break;
				case 1:
					this.zoom = (float)0.75;
					break;
				case 2:
					this.zoom = (float) 0.5;
					break;
				default:
					this.zoom = (float)1.0;
					break;
			}
	
			this.AdjustDrawArea();
		}
		
		
		private void CheckEnable () {
			if (this.pageNumber < this.pages.Count -1) {
				this.ForwardButton.Enabled = true;
			} else {
				this.ForwardButton.Enabled = false;
			}
			if ((this.pageNumber > 0) ) {
				this.BackButton.Enabled = true;
			} else {
				this.BackButton.Enabled = false;
			}
//			this.UpdateToolStrip();
		}
		
		void BackButtonClick(object sender, System.EventArgs e){
			CheckEnable();
			if (this.pageNumber > 0) {
				this.pageNumber --;
				CheckEnable();
				this.ShowPages();
			}
		}
		
		void ForwardButtonClick(object sender, System.EventArgs e){
			if (this.pageNumber < this.pages.Count) {
				this.pageNumber ++;
				CheckEnable();
				this.ShowPages();
			} 
		}
		
		void PrintButton(object sender, System.EventArgs e)
		{
			MessageBox.Show ("Print not implemented");
		}
		
		public PageSettings PageSettings {
			get {

				return pageSettings;
			}
			set {
				pageSettings = value;
				this.AdjustDrawArea();
			}
		}
		public void SetPages (PagesCollection pages) {
			this.pages = pages;
			this.pageNumber = 0;
			this.CheckEnable();
			this.ShowPages();
		}
		
		public PagesCollection Pages {
			get {
				return this.pages;
			}
		}
		
	}
}
