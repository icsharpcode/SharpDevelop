// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core.ReportViewer
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class PreviewControl 
	{
		public event EventHandler<EventArgs> PreviewLayoutChanged;

		private IExportRunner runner;
		private float zoom;

		private int pageNumber;
		private Bitmap bitmap;
		private IReportViewerMessages reportViewerMessages;
		private PreviewRenderer previewRenderer;

		private delegate void invokeDelegate();
		private ReportSettings reportSettings;


		#region Constructor

		public PreviewControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
			this.UpdateStyles();
			runner = new ExportRunner();
			InitZoomCombo();
			previewRenderer = PreviewRenderer.CreateInstance();
			this.CheckEnable();
			SetTransparentBackground();
			this.numericToolStripTextBox2.Navigate += new EventHandler<PageNavigationEventArgs>(OnNavigate);
		}


		private void SetTransparentBackground()
		{
			this.printButton.ImageTransparentColor = Color.White;
			this.pdfButton.ImageTransparentColor = Color.White;
			this.firstPageButton.ImageTransparentColor = Color.White;
			this.forwardButton.ImageTransparentColor = Color.White;
			this.backButton.ImageTransparentColor = Color.White;
			this.lastPageButton.ImageTransparentColor = Color.White;
		}

		#endregion


		public void RunReport(string fileName, ReportParameters parameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			this.RunReport(model, parameters);
		}


		public void RunReport(ReportModel reportModel, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			this.SetupViewer(reportModel);
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet) {
				RunFormSheet(reportModel);
			} else {
				ReportEngine.CheckForParameters(reportModel, parameters);
				var dataManager = DataManagerFactory.CreateDataManager(reportModel, parameters);
				
				RunReport(reportModel,dataManager);
				
			}
		}


		public void RunReport(ReportModel reportModel, DataTable dataTable, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
//			ReportEngine.CheckForParameters(reportModel, parameters);
			IDataManager dataManager = DataManagerFactory.CreateDataManager(reportModel, dataTable);
			RunReport(reportModel, dataManager);
		}


		public void RunReport(ReportModel reportModel, IList dataSource, ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataSource == null) {
				throw new ArgumentNullException("dataSource");
			}
//			ReportEngine.CheckForParameters(reportModel, parameters);
			var dataManager = DataManagerFactory.CreateDataManager(reportModel, dataSource);
			RunReport(reportModel, dataManager);
		}


		public void RunReport(ReportModel reportModel, IDataManager dataManager)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			this.SetupViewer(reportModel);
			RunDataReport(reportModel, dataManager);
		}


		#region Rendering

		private void RunFormSheet(ReportModel reportModel)
		{
			runner.RunReport(reportModel,(ReportParameters)null);
			ShowCompleted();
		}


		private void RunDataReport(ReportModel reportModel, IDataManager data)
		{
			runner.RunReport(reportModel,data);
			ShowCompleted();
		}

		#endregion


		#region Events from worker
/*
		private void OnPageCreated(object sender, PageCreatedEventArgs e)
		{
			if (this.Pages.Count == 1) {
				ShowSelectedPage();
				this.pageNumber = 0;
			}

		}
*/


		private void PushPrinting(object sender, SectionRenderEventArgs e)
		{
//			string sectionName = e.Section.Name;
		}
			/*
			if (sectionName == ReportSectionNames.ReportHeader) {
				Console.WriteLine("PushPrinting  :" + ReportSectionNames.ReportHeader);
			} 
			
			else if (sectionName == ReportSectionNames.ReportPageHeader) {
				Console.WriteLine("PushPrinting :" +ReportSectionNames .ReportPageHeader);
			} 
			
			else if (sectionName == ReportSectionNames.ReportDetail){
				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportDetail);
			}
			
			else if (sectionName == ReportSectionNames.ReportPageFooter){
				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportPageFooter);
			}
			
			else if (sectionName == ReportSectionNames.ReportFooter){
				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportFooter);
			}
			
			else{
				throw new WrongSectionException(sectionName);
			}
			*/


		private void GroupHeaderRendering(object sender, GroupHeaderEventArgs ghea)
		{
//			Console.WriteLine("ReportViewer - GroupHeaderRendering  :");
//			BaseGroupedRow v = ghea.GroupHeader;
//			v.BackColor = System.Drawing.Color.Red;
		}


		private void GroupFooterRendering(object sender, GroupFooterEventArgs gfea)
		{
//			Console.WriteLine();
//			Console.WriteLine("ReportViewer - GroupFooterRendering  :");
//			var v = gfea.GroupFooter;
//			v.BackColor = System.Drawing.Color.Red;
//			BaseTextItem i = (BaseTextItem)v.Items[0];
//			i.Text ="neuer text";
		}


		private void RowRendering(object sender, RowRenderEventArgs rrea)
		{
			//Console.WriteLine("ReportViewer - RowRendering  :");
		}

		#endregion


		private void AdjustDrawArea()
		{
			if (this.reportSettings != null) {
				this.drawingPanel.ClientSize = this.drawingPanel.ClientSize = new Size((int)(this.reportSettings.PageSize.Width * this.zoom), (int)(this.reportSettings.PageSize.Height * this.zoom));
			}
		}


		#region setup

		private void SetupViewer(ReportModel reportModel)
		{
			this.reportSettings = reportModel.ReportSettings;
			this.AdjustDrawArea();
		}


		private void ShowCompleted()
		{
			if (this.InvokeRequired) {
				invokeDelegate updateControl = delegate() { };
				updateControl = ShowCompleted;
				Invoke(updateControl);
			}
			this.SetPages();
			this.CheckEnable();
			this.printButton.Enabled = true;
			this.pdfButton.Enabled = true;
			this.UpdateToolStrip();
			ShowSelectedPage();
		}

		#endregion

		#region ToolStrip

		private void InitZoomCombo()
		{
			foreach (string s in GlobalLists.ZoomValues()) {
				this.comboZoom.Items.Add(s);
			}
			this.comboZoom.SelectedIndex = this.comboZoom.FindString("100");
		}


		void comboZoomSelectedIndexChange(object sender, System.EventArgs e)
		{
			if (this.comboZoom.SelectedItem.ToString().IndexOf("%") > 0) {
				string s1 = this.comboZoom.SelectedItem.ToString().Substring(0, this.comboZoom.SelectedItem.ToString().IndexOf("%"));
				this.zoom = (float)Convert.ToDecimal(s1, CultureInfo.InvariantCulture) / 100;
			} else {
				string sel = this.comboZoom.SelectedItem.ToString();
				switch (sel) {
					case "Actual Size":
						this.zoom = 1;
						break;
					default:
						this.zoom = 1;
						break;
				}
			}

			this.Invalidate(true);
			this.Update();
			this.AdjustDrawArea();
			if ((this.Pages != null) && (pageNumber < Pages.Count)) {
				this.ShowSelectedPage();
			}
			EventHelper.Raise<EventArgs>(this.PreviewLayoutChanged, this, e);
		}


		private void Localize()
		{
			if (this.reportViewerMessages != null) {
				this.firstPageButton.ToolTipText = this.reportViewerMessages.FirstPageMessage;
				this.backButton.ToolTipText = this.reportViewerMessages.BackButtonText;
				this.forwardButton.ToolTipText = this.reportViewerMessages.NextButtonMessage;
				this.lastPageButton.ToolTipText = this.reportViewerMessages.LastPageMessage;

				this.printButton.ToolTipText = this.reportViewerMessages.PrintButtonMessage;
				this.comboZoom.ToolTipText = this.reportViewerMessages.ZoomMessage;
				this.createPdfMenu.Text = this.reportViewerMessages.PdfFileMessage;
				this.pdfButton.ToolTipText = this.reportViewerMessages.PdfFileMessage;
			}
		}


		private void UpdateToolStrip()
		{
			if (this.InvokeRequired) {
				invokeDelegate updateControl = delegate() { };
				updateControl = UpdateToolStrip;
				Invoke(updateControl);
			}
			if (this.toolStrip1 != null) {
				string str = String.Empty;
				if (this.Pages != null) {

					str = String.Format(CultureInfo.CurrentCulture, "of {0}", this.Pages.Count);
				}
				this.numericToolStripTextBox2.Text = (this.pageNumber + 1).ToString(CultureInfo.CurrentCulture);
				this.pageInfoLabel.Text = str;
			}
		}


		private void OnNavigate(object sender, PageNavigationEventArgs e)
		{
			int i = e.PageNumber - 1;

			if ((i > -1)) {
				if (i == this.pageNumber) {
					return;
				}
				this.pageNumber = i;
				if (this.pageNumber < this.Pages.Count) {
					this.CheckEnable();
					this.ShowSelectedPage();
				}
			}
		}

		#endregion


		#region Drawing

		private void ShowSelectedPage()
		{
			if (this.InvokeRequired) {
				invokeDelegate updateControl = delegate() { };
				updateControl = ShowSelectedPage;
				Invoke(updateControl);
			}
			if (this.pageNumber < this.Pages.Count) {
				ExporterPage sp = Pages[this.pageNumber];

				if (this.bitmap != null) {
					this.bitmap.Dispose();
				}
				this.bitmap = this.CreateBitmap(sp);
				this.Invalidate(true);
				this.Update();
			}
			this.UpdateToolStrip();
		}


		private void CenterDisplayPanel()
		{
			if (this.Width > drawingPanel.Width)
				drawingPanel.Left = (this.Width - drawingPanel.Width) / 2;
			else
				drawingPanel.Left = 3;
		}



		private void DrawingPanelPaint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(this.drawingPanel.BackColor);
			CenterDisplayPanel();
			if (this.bitmap != null) {
				e.Graphics.DrawImage(this.bitmap, 0, 0);
			}
		}


		private Bitmap CreateBitmap(ExporterPage page)
		{
			Bitmap bm = new Bitmap(this.drawingPanel.ClientSize.Width, this.drawingPanel.ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			using (Graphics gr = Graphics.FromImage(bm)) {

				// Reset Transform to org. Value
				gr.Clear(this.drawingPanel.BackColor);
				//	this.Invalidate();
				gr.ScaleTransform(1f, 1f);
				gr.Clear(this.drawingPanel.BackColor);
				gr.ScaleTransform(this.zoom, this.zoom);

				gr.TextRenderingHint = TextRenderingHint.AntiAlias;
				gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
				gr.InterpolationMode = InterpolationMode.High;

				previewRenderer.Graphics = gr;
				previewRenderer.Page = page;
				previewRenderer.RenderOutput();
			}
			return bm;
		}

		#endregion


		#region PageNavigation

		private void CheckEnable()
		{
			if ((this.Pages == null) || (this.Pages.Count == 0)) {
				this.firstPageButton.Enabled = false;
				this.forwardButton.Enabled = false;
				this.backButton.Enabled = false;
				this.lastPageButton.Enabled = false;
				return;
			} else {
				this.firstPageButton.Enabled = true;
				this.forwardButton.Enabled = true;
				this.backButton.Enabled = true;
				this.lastPageButton.Enabled = true;
			}

		}


		private void FirstPageButtonClick(object sender, System.EventArgs e)
		{
			if ((this.Pages != null) && (this.pageNumber > 0)) {
				this.pageNumber = 0;
			}
			this.CheckEnable();
			this.ShowSelectedPage();
		}



		private void BackButtonClick(object sender, System.EventArgs e)
		{
			CheckEnable();
			if (this.pageNumber > 0) {
				this.pageNumber--;

			} else {
				this.pageNumber = this.Pages.Count - 1;
			}
			this.CheckEnable();
			this.ShowSelectedPage();
		}


		private void ForwardButtonClick(object sender, System.EventArgs e)
		{
			if (this.pageNumber < this.Pages.Count - 1) {
				this.pageNumber++;
			} else {
				this.pageNumber = 0;
			}
			CheckEnable();
			this.ShowSelectedPage();
		}


		private void LastPageButtonClick(object sender, System.EventArgs e)
		{
			this.pageNumber = this.Pages.Count - 1;
			CheckEnable();
			this.ShowSelectedPage();
		}



		private void PrintButton(object sender, System.EventArgs e)
		{
			using (PrintDialog dlg = new PrintDialog()) {
				DialogResult result = dlg.ShowDialog();
				if (result == DialogResult.OK) {
					PrintRenderer printer = PrintRenderer.CreateInstance(this.Pages, dlg.PrinterSettings);
					printer.Start();
					printer.RenderOutput();
					printer.End();
				}
			}
		}


		private void PdfButtonClick(object sender, EventArgs e)
		{
			using (SaveFileDialog saveDialog = new SaveFileDialog()) {
				saveDialog.FileName = this.reportSettings.ReportName;
				saveDialog.DefaultExt = "PDF";
				saveDialog.ValidateNames = true;
				if (saveDialog.ShowDialog() == DialogResult.OK) {
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(this.reportSettings, this.Pages, saveDialog.FileName, true)) {
						pdfRenderer.Start();
						pdfRenderer.RenderOutput();
						pdfRenderer.End();
					}
				}
			}
		}


		#endregion

		private void SetPages()
		{
			this.pageNumber = 0;
			this.CheckEnable();
		}


		public PagesCollection Pages
		{
			get {return runner.Pages;}
		}
	

		public IReportViewerMessages Messages {
			get { return this.reportViewerMessages; }
			set {
				this.reportViewerMessages = value;
				Localize();
			}
		}
	}
}
