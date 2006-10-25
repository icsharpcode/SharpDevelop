/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 16.10.2006
 * Time: 22:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using SharpReportCore;
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
		private List<SinglePage> pages;
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
			this.toolStripTextBox1.Text = "";
			
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
		
		private void DrawItems (Graphics gr,ExporterCollection<BaseExportColumn> items) {
			
			foreach (SharpReportCore.Exporters.BaseExportColumn ex in items) {
				if (ex != null) {
					ExportContainer cont = ex as ExportContainer;
					if (cont == null) {
//						System.Console.WriteLine("{0}",ex.GetType());
						TextDrawer.PaintString(gr,ex.ToString(),ex.StyleDecorator);
					} else {
						DrawItems(gr,cont.Items);
					}
					
				}
			}
		}
		
		private Bitmap BuildBitmap (SinglePage page) {
			System.Console.WriteLine("BuildBitmap(SinglePage)");
			System.Console.WriteLine("\tstart createBitmap {0}",DateTime.Now);
			Bitmap bm = new Bitmap(this.panel2.ClientSize.Width,this.panel2.ClientSize.Height,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			using (Graphics gr = Graphics.FromImage(bm)) {

				// Reset Transform to org. Value
				
				gr.ScaleTransform(1F,1F);
				gr.Clear(Color.Beige);
				gr.ScaleTransform(this.zoom,this.zoom);

//				if (this.pageSettings != null) {
//					gr.DrawRectangle(new Pen(Color.Black),
//					                 this.PageRectangle ());
//				}
				DrawItems (gr,page.Items);
			}
			System.Console.WriteLine("\tready createBitmap {0}",DateTime.Now);
			return bm;
	
		}
		
		private void ShowPages () {
			System.Console.WriteLine("ReportViewer:ShowPages {0}",this.pages.Count);
			this.pageNumber = 0;
			SinglePage sp = pages[this.pageNumber];
			
			if (this.bitmap != null) {
				this.bitmap.Dispose();
			}
			this.bitmap = this.BuildBitmap(sp);
		
			this.toolStripTextBox1.Text = String.Empty;
			string str = String.Format ("Page {0} of {1}",this.pageNumber +1,this.pages.Count);
			this.toolStripTextBox1.Text = str;
			this.Invalidate(true);
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
		void ToolStripTextBox1Click(object sender, System.EventArgs e)
		{
			MessageBox.Show ("TextBox Chlicked");
		}
		
		void BackButton(object sender, System.EventArgs e)
		{
			MessageBox.Show ("BackButton not implemented");
		}
		
		void ForwardButton(object sender, System.EventArgs e)
		{
			MessageBox.Show ("´Forward not implemented");
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
		
		
		public List<SinglePage> Pages {
			set {
				pages = value;
				ShowPages();
			}
		}
		
	}
}
