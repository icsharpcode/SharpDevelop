// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.ReportViewer
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class PreviewControl
	{
		public event EventHandler <EventArgs> PreviewLayoutChanged;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		private float zoom;
		
		private int pageNumber;
		private Bitmap bitmap;
		private PagesCollection pages;
		private IReportViewerMessages reportViewerMessages;
		private PreviewRenderer previewRenderer;
		
		private delegate void invokeDelegate();
		private ReportSettings reportSettings;
		private IDataManager dataManager;
//		private string pagesCreatedMessage;
		
		#region Constructor
		
		public PreviewControl(){
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.ResizeRedraw,
			              true);
			this.UpdateStyles();
			InitZoomCombo ();
			
			previewRenderer = PreviewRenderer.CreateInstance();
			this.CheckEnable();
			this.printButton.Enabled = false;
			this.printButton.ImageTransparentColor = Color.White;
			
			this.pdfButton.Enabled = false;
			this.pdfButton.ImageTransparentColor = Color.White;
			
			this.firstPageButton.Enabled = false;
			this.firstPageButton.ImageTransparentColor = Color.White;
			
			this.forwardButton.Enabled = false;
			this.forwardButton.ImageTransparentColor = Color.White;
			
			this.backButton.Enabled = false;
			this.backButton.ImageTransparentColor = Color.White;
			
			this.lastPageButton.Enabled = false;
			this.lastPageButton.ImageTransparentColor = Color.White;
			this.numericToolStripTextBox2.Navigate += new EventHandler <PageNavigationEventArgs> (OnNavigate);
		}
		
		#endregion

		public void RunReport (string fileName,ReportParameters parameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			this.RunReport(model,parameters);
		}
		
		[Obsolete("Use RunReport (string fileName,ReportParameters parameters)" )]
		public void SetupAsynchron (string fileName,ReportParameters parameters)
		{
			
			
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel m = ReportEngine.LoadReportModel(fileName);
			RunReport(m,parameters);
		}
		
		
		public void RunReport (ReportModel reportModel,ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			System.Console.WriteLine("RunReport");
			this.SetupViewer(reportModel);
			
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet) {
				RunFormSheet(reportModel);
			} else {
				ReportEngine.CheckForParameters(reportModel,parameters);
				this.dataManager = DataManagerFactory.CreateDataManager(reportModel,parameters);
				RunDataReport (reportModel,dataManager);
			}
		}
		
		[Obsolete("Use RunReport (reportModel,parameters)")]
		public void SetupAsynchron (ReportModel reportModel,ReportParameters parameters)
		{
			
			RunReport (reportModel,parameters);
			/*
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			
			this.SetupViewer(reportModel);
			
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet) {
				RunFormSheet(reportModel);
			} else {
				ReportEngine.CheckForParameters(reportModel,parameters);
				this.dataManager = DataManagerFactory.CreateDataManager(reportModel,parameters);
				RunDataReport (reportModel,dataManager);
			}
			 */
		}
		
		
		public void RunReport (ReportModel reportModel,DataTable dataTable,ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			RunReport (reportModel,DataManagerFactory.CreateDataManager(reportModel,dataTable));
		}
		
		
		[Obsolete("Use RunReport(reportModel,dataTable,parameters)")]
		public void SetupAsynchron (ReportModel reportModel,DataTable dataTable,ReportParameters parameters)
		{
			RunReport(reportModel,dataTable,parameters);
			/*
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			SetupAsynchron (reportModel,DataManagerFactory.CreateDataManager(reportModel,dataTable));
			 */
		}
		
		
		public void RunReport (ReportModel reportModel,IDataManager dataManager)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			
			this.SetupViewer(reportModel);
			this.dataManager = dataManager;
			RunDataReport(reportModel,dataManager);
		}
		
		
		[Obsolete("Use RunReport(reportModel,dataManager)")]
		public void SetupAsynchron (ReportModel reportModel,IDataManager dataManager)
		{
			RunReport(reportModel,dataManager);
		}
		
		
		#region Rendering
		
		private void RunFormSheet (ReportModel reportModel)
		{
			
			Layouter layouter = new Layouter();
			IReportCreator reportCreator = FormPageBuilder.CreateInstance(reportModel,layouter);
			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
			ShowCompleted ();
		}
		
		
		private void RunDataReport (ReportModel reportModel,IDataManager data)
		{
			ILayouter layouter = new Layouter();
			IReportCreator reportCreator = DataPageBuilder.CreateInstance(reportModel,data,layouter);
			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
			ShowCompleted();
		}
		
		#endregion
		
		
		#region Events from worker
		
		
		private void OnPageCreated (object sender, PageCreatedEventArgs e)
		{
			this.Pages.Add(e.SinglePage);
			if (this.Pages.Count == 1) {
				ShowSelectedPage();
				this.pageNumber = 0;
			}
			
		}
		
		private void PushPrinting (object sender, SectionRenderEventArgs e ) {
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,e);
			string sectionName = e.Section.Name;
			
			if (sectionName == ReportSectionNames.ReportHeader) {
				Console.WriteLine("xx  " + ReportSectionNames.ReportHeader);
			} 
			
			else if (sectionName == ReportSectionNames.ReportPageHeader) {
				Console.WriteLine("xx " +ReportSectionNames .ReportPageHeader);
			} 
			
			else if (sectionName == ReportSectionNames.ReportDetail){
				Console.WriteLine("xx " + ReportSectionNames.ReportDetail);
			}
			
			else if (sectionName == ReportSectionNames.ReportPageFooter){
				Console.WriteLine("xx " + ReportSectionNames.ReportPageFooter);
			}
			
			else if (sectionName == ReportSectionNames.ReportFooter){
				Console.WriteLine("xx " + ReportSectionNames.ReportFooter);
			}
			
			else{
				throw new WrongSectionException(sectionName);
			}
		}
		
		//testcode to handle sectionrenderevent
		
	
		
		/*
		private void PushPrinting (object sender,SectionRenderEventArgs e)
		{

			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
					BaseRowItem ri = e.Section.Items[0] as BaseRowItem;
					if (ri != null) {
						BaseDataItem r = (BaseDataItem)ri.Items.Find("Kategoriename");
						if (r != null) {
							r.DBValue = "xxxxxxx";
						}
					}
					
					break;
				case GlobalEnums.ReportSection.ReportPageFooter:
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:
					break;
					
				default:
					break;
			}
		}
		 */
		
		#endregion
		
		
		private void AdjustDrawArea()
		{
			if (this.reportSettings != null) {
				if (this.reportSettings.Landscape == false) {
					this.drawingPanel.ClientSize = new Size((int)(this.reportSettings.PageSize.Width * this.zoom),
					                                        (int)(this.reportSettings.PageSize.Height * this.zoom));
				} else {
					this.drawingPanel.ClientSize = new Size((int)(this.reportSettings.PageSize.Height * this.zoom),
					                                        (int)(this.reportSettings.PageSize.Width * this.zoom));
				}
			}
		}
		
		#region setup
		
		private void SetupViewer (ReportModel reportModel)
		{
			this.pages = new PagesCollection();
			this.reportSettings = reportModel.ReportSettings;
			this.AdjustDrawArea();
		}
		
		
		private void ShowCompleted()
		{
			if (this.InvokeRequired) {
				invokeDelegate updateControl = delegate(){};
				updateControl = ShowCompleted;
				Invoke (updateControl);
			}
			if (this.dataManager != null) {
				this.dataManager.GetNavigator.Reset();
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
		
		private void InitZoomCombo ()
		{
			foreach (string s in GlobalLists.ZoomValues())
			{
				this.comboZoom.Items.Add(s);
			}
			this.comboZoom.SelectedIndex = this.comboZoom.FindString("100");
		}
		
		
		void comboZoomSelectedIndexChange(object sender, System.EventArgs e)
		{
			if (this.comboZoom.SelectedItem.ToString().IndexOf("%") > 0) {
				string s1 = this.comboZoom.SelectedItem.ToString().Substring(0,this.comboZoom.SelectedItem.ToString().IndexOf("%"));
				this.zoom = (float)Convert.ToDecimal(s1) / 100;
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
			if ((this.pages != null) && (pageNumber < pages.Count))
             {
                 this.ShowSelectedPage();
             }
			EventHelper.Raise<EventArgs>(this.PreviewLayoutChanged,this,e);
		}
		
		
		private void Localize ()
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
//				this.pagesCreatedMessage = this.reportViewerMessages.PagesCreatedMessage;
			}
		}
		
		
		private void UpdateToolStrip ()
		{
			if (this.InvokeRequired) {
				invokeDelegate updateControl = delegate(){};
				updateControl = UpdateToolStrip;
				Invoke (updateControl);
			}
			if (this.toolStrip1 != null){
				string str = String.Empty;
				if (this.pages != null) {

					str = String.Format (CultureInfo.CurrentCulture,
					                     "of {0}",this.pages.Count);
				}
				this.numericToolStripTextBox2.Text = (this.pageNumber+1).ToString(CultureInfo.CurrentCulture);
				this.pageInfoLabel.Text = str;
			}
		}
		
		
		private void OnNavigate (object sender, PageNavigationEventArgs e)
		{
			int i = e.PageNumber -1;
			
			if ((i > -1)) {
				if (i == this.pageNumber) {
					return;
				}
				this.pageNumber = i;
				if (this.pageNumber < this.pages.Count) {
					this.CheckEnable();
					this.ShowSelectedPage();
				}
			}
		}
		
		#endregion
		
		
		#region Drawing
		
		private void ShowSelectedPage ()
		{
			if (this.InvokeRequired) {
				invokeDelegate updateControl = delegate(){};
				updateControl = ShowSelectedPage;
				Invoke (updateControl);
			}
			if (this.pageNumber < this.pages.Count) {
				ExporterPage sp = pages[this.pageNumber];
				
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
				e.Graphics.DrawImage(this.bitmap,0,0);
			}
		}
		
		
		private Bitmap CreateBitmap (ExporterPage page)
		{
			Bitmap bm = new Bitmap(this.drawingPanel.ClientSize.Width,this.drawingPanel.ClientSize.Height,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			
			using (Graphics gr = Graphics.FromImage(bm)) {

				// Reset Transform to org. Value
				gr.Clear(this.drawingPanel.BackColor);
				//	this.Invalidate();
				gr.ScaleTransform(1F,1F);
				gr.Clear(this.drawingPanel.BackColor);
				gr.ScaleTransform(this.zoom,this.zoom);

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
		
		private void CheckEnable ()
		{
			if ((this.pages == null)||(this.pages.Count == 0)) {
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
			if ((this.pages != null) && (this.pageNumber > 0)) {
				this.pageNumber = 0;
			}
			this.CheckEnable();
			this.ShowSelectedPage();
		}
		
		
		
		private void BackButtonClick(object sender, System.EventArgs e)
		{
			CheckEnable();
			if (this.pageNumber > 0) {
				this.pageNumber --;
				
			} else {
				this.pageNumber = this.pages.Count -1;
			}
			this.CheckEnable();
			this.ShowSelectedPage();
		}
		
		
		private void ForwardButtonClick(object sender, System.EventArgs e)
		{
			if (this.pageNumber < this.pages.Count-1) {
				this.pageNumber ++;
			} else {
				this.pageNumber = 0;
			}
			CheckEnable();
			this.ShowSelectedPage();
		}
		
		
		private void LastPageButtonClick(object sender, System.EventArgs e)
		{
			this.pageNumber = this.pages.Count -1;
			CheckEnable();
			this.ShowSelectedPage();
		}
		
		
		
		private void PrintButton(object sender, System.EventArgs e)
		{
			using (PrintDialog dlg = new PrintDialog()) {
				DialogResult result = dlg.ShowDialog();
				if (result==DialogResult.OK){
					PrintRenderer printer = PrintRenderer.CreateInstance(this.pages,dlg.PrinterSettings);
					printer.Start();
					printer.RenderOutput();
					printer.End();
				}
			}
		}
		
		
		private void PdfButtonClick(object sender, EventArgs e)
		{
			this.CreatePdf();
		}
		
		
		private void CreatePdf ()
		{
			using (SaveFileDialog saveDialog = new SaveFileDialog()){
				saveDialog.FileName = this.reportSettings.ReportName;
				saveDialog.DefaultExt = "PDF";
				saveDialog.ValidateNames = true;
				if(saveDialog.ShowDialog() == DialogResult.OK){
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(this.reportSettings,
					                                                            this.pages,
					                                                            saveDialog.FileName,
					                                                            true)) {
						pdfRenderer.Start();
						pdfRenderer.RenderOutput();
						pdfRenderer.End();
					}
				}
			}
		}
		
		#endregion
		
		private void SetPages ()
		{
			this.pageNumber = 0;
			this.CheckEnable();
		}
		
		
		public PagesCollection Pages
		{
			get {
				if (this.pages == null) {
					this.pages = new PagesCollection();
				}
				return this.pages;
			}
		}
		
		
		public IReportViewerMessages Messages
		{
			get { return this.reportViewerMessages;}
			set { this.reportViewerMessages = value;
				Localize();
			}
		}
		
		#region Asyncron use of PageBuilder
		/*
		public void SetupAsynchron (string fileName,ReportParameters parameters)
		{
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel m = ReportEngine.LoadReportModel(fileName);
			this.SetupAsynchron(m,parameters);
		}
		 */
		/// <summary>
		/// For internal use only
		/// </summary>
		/// <param name="reportModel"></param>
		/*
		public void SetupAsynchron (ReportModel reportModel,ReportParameters parameters)
		{
			
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			
			this.SetupViewer(reportModel);
			this.SetupWorker();
			if (reportModel.DataModel == GlobalEnums.PushPullModel.FormSheet) {
				this.bgw.DoWork += new DoWorkEventHandler(FormSheetWorker);
			} else {
				ReportEngine.CheckForParameters(reportModel,parameters);
				this.dataManager = DataManagerFactory.CreateDataManager(reportModel,parameters);
				this.bgw.DoWork += new DoWorkEventHandler(PushPullWorker);
			}
			this.bgw.RunWorkerAsync(reportModel);
		}
		 */
		#region PushModel - IList
		
		/*
		public void SetupAsynchron (ReportModel reportModel,DataTable dataTable,ReportParameters parameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			SetupAsynchron (reportModel,DataManagerFactory.CreateDataManager(reportModel,dataTable));
		}
		
		public void SetupAsynchron (ReportModel reportModel,IDataManager dataManager)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			
			this.SetupViewer(reportModel);
			SetupWorker();
			
			this.dataManager = dataManager;
			this.bgw.DoWork += new DoWorkEventHandler(PushPullWorker);
			this.bgw.RunWorkerAsync(reportModel);
		}
		 */
		#endregion
		
		/*
		private void SetupWorker ()
		{
			this.bgw = new BackgroundWorker();
			this.bgw.WorkerReportsProgress = true;
			this.bgw.WorkerSupportsCancellation = true;
			this.bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
			this.bgw.ProgressChanged +=    new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
		}
		 */
		
		#region Worker
		
		/*
		private void FormSheetWorker(object sender,DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			e.Result = RunFormSheet((ReportModel)e.Argument,worker,e);
		}
		 */
		/*
		
		private void PushPullWorker(object sender,DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			e.Result = RunDataReport((ReportModel)e.Argument,this.dataManager,worker,e);
		}
		 */
		#endregion
		
		
		#region WorkerEvents
		/*
		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			System.Console.WriteLine("BackgroundWorker_RunWorkerCompleted");
			
			if (e.Error != null) {
				throw new ReportException("error in background worker", e.Error);
			}
			if (this.Pages.Count > 0) {
				this.pageNumber = 0;
				this.ShowSelectedPage();
			}
			this.ShowCompleted();
		}
		 */
		/*
		
		private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.pageNumber = e.ProgressPercentage;
			this.ShowSelectedPage();
		}
		 */
		#endregion
		
		#region Worker
		/*
		private object RunFormSheet (ReportModel reportModel,BackgroundWorker worker, DoWorkEventArgs e)
		{
			if (worker.CancellationPending)
			{
				e.Cancel = true;
			}
			else
			{
				Layouter layouter = new Layouter();
				IReportCreator inst = FormPageBuilder.CreateInstance(reportModel,layouter);
				inst.PageCreated += delegate (Object sender,PageCreatedEventArgs ee) {
					worker.ReportProgress(inst.Pages.Count);
					this.Pages.Add(ee.SinglePage);
				};
				
				inst.BuildExportList();
				return inst.Pages.Count;
			}
			return null;
		}
		
		
		private object RunPushData (ReportModel reportModel,IDataManager data,BackgroundWorker worker,DoWorkEventArgs e)
		{
			if (worker.CancellationPending)
			{
				e.Cancel = true;
			}
			else
			{
				RunDataReport(reportModel,data,worker,e);
			}
			return null;
		}
		 */
		/*
		
		private object RunDataReport (ReportModel reportModel,IDataManager data,BackgroundWorker worker,DoWorkEventArgs e)
		{
			if (worker.CancellationPending)
			{
				e.Cancel = true;
			}
			else
			{
				ILayouter layouter = new Layouter();
				
				IReportCreator reportCreator = DataPageBuilder.CreateInstance(reportModel,data,layouter);
				//testcode to handle sectionrenderevent
				reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
				
				reportCreator.PageCreated += delegate (Object sender,PageCreatedEventArgs ee) {
					worker.ReportProgress(reportCreator.Pages.Count);
					this.Pages.Add(ee.SinglePage);
				};
				reportCreator.BuildExportList();
				return reportCreator.Pages.Count;
			}
			return null;
		}
		 */
		#endregion
		
		#endregion
		
	}
}
