// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;

namespace ReportSamples
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		ReportEngine engine;
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.btnReportViewer.Checked = true;
			
			// Localisation of ReportViewer
			this.previewControl1.Messages = new ReportViewerMessagesProvider();
			ShowPath();
		}
		
		private void ShowPath()
		{
			
			foreach (string d in Directory.GetDirectories(Application.StartupPath))
			{
				Console.WriteLine(d);
			}
			
			string path1 = Path.GetFullPath(Application.StartupPath);
			string[] folders1 = path1.Split(Path.DirectorySeparatorChar);
			Console.WriteLine (path1);

			foreach (string dd in folders1){
				Console.WriteLine(dd);
			}

			int i = path1.IndexOf("samples");

			string subPath = path1.Substring (0,i + 8);
			
			string reportsDirPath = string.Concat(subPath,"Reports" + Path.DirectorySeparatorChar);
			Console.WriteLine(subPath);
			

			foreach (string dir in Directory.GetDirectories(reportsDirPath))
			{
				if ( !dir.StartsWith(".")) {
					string []dn =  dir.Split(Path.DirectorySeparatorChar);
					Console.WriteLine (dn.GetLength(0));
					Console.WriteLine("Directory : {0}",dn[dn.GetLength(0) -1]);
				}
				
				Console.WriteLine("FileList");
				foreach (string fileName in Directory.GetFiles(dir,"*.srd")) {
					Console.WriteLine ("\t{0}",Path.GetFileNameWithoutExtension(fileName));
				}
			}
		}
		
		
		
		
		#region FormSheet
		
		void FormSheetToolStripMenuItemClick(object sender, EventArgs e)
		{
			/*
			JointCopyright jca = new JointCopyright();
			this.engine = jca.Engine;
			this.DisplayFormSheet(jca.ReportName);
			*/
			MessageBox.Show("Not implemented");
		}
		
		#endregion
		
		
		void StandartPullModelClick(object sender, EventArgs e)
		{
			/*
			StandartPullModel emp = new StandartPullModel();
			this.engine = emp.Engine;
			this.DisplayPullData(emp.ReportName,null);
			*/
			MessageBox.Show("Not implemented");
		}
		
		void ProviderIndependentClick(object sender, EventArgs e){
			
//			ProviderIndependent ms = new ProviderIndependent();
//			this.engine = ms.Engine;
//			this.DisplayPullData(ms.ReportName,ms.Parameters);
			
		}
		
		void StandartPushModelClick(object sender, EventArgs e)
		{
			/*
			StandartPushModel emp = new StandartPushModel();
			if (emp.DataTable != null) {
				this.engine = emp.Engine;
				this.DisplayPushDataStandart(emp.ReportName,emp.ReportModel,emp.DataTable);
			}
			*/
			MessageBox.Show("Not implemented");
		}
		
		
		#region EventLogger
		
		void EventLoggerClick(object sender, EventArgs e)
		{
//			EventLogger ev = new EventLogger();
//			ev.Run();
			MessageBox.Show("Not implementet yet");
		}
		
		#endregion
		
		#region Contributor#s sorted by Lastname
		
		void ContributorsSortedByLastnameClick(object sender, EventArgs e)
		{
			/*
			ContributorsList conReport = new ContributorsList();
			this.engine = conReport.Engine;
			ReportParameters parameters =  ReportEngine.LoadParameters(conReport.ReportName);
			parameters.SortColumnCollection.Add(new SortColumn("First",
			                                                   System.ComponentModel.ListSortDirection.Ascending));
			
			this.rowNr = 0;
			this.ContributorsbyLastname(conReport.ReportName,conReport.ReportModel,conReport.Contributors,parameters);
		*/
		MessageBox.Show("Not implemented");
		}
		
		/*
		void ContributorsbyLastname (string fileName,ReportModel model,IList list,ReportParameters reportParameters) {
			ICSharpCode.Reports.Core.Exporter.BasePager pageBuilder;
			if (this.btnReportViewer.Checked) {
				this.previewControl1.SetupAsynchron(model,list);
			}else if(this.btnPreviewControl.Checked){
				engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(OnRenderSortedContributors);
				engine.PreviewPushDataReport (fileName,list,reportParameters);
			}else if (this.btnPrinter.Checked){
				pageBuilder = engine.CreatePageBuilder(model,list,reportParameters);
				pageBuilder.BuildExportList();
				using (PrintRenderer printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages)){
					this.OutputToPrinter(printRenderer);
				}
			}else if(this.btnPDF.Checked){
				
				string fN = this.SelectFilename();
				if (!String.IsNullOrEmpty(fN)) {
					pageBuilder = engine.CreatePageBuilder(model,list,reportParameters);
					pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(OnRenderSortedContributors);
					pageBuilder.BuildExportList();
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder.Pages,fN,true)){
						this.OutputToPdf(pdfRenderer);
					}
				}
			}
		}
		*/
		
		private void OnRenderSortedContributors (object sender,	SectionRenderEventArgs e){

			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
//					System.Console.WriteLine("\tReportHeader");
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
//					System.Console.WriteLine("\tPageheader");
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
//					this.rowNr ++;
					BaseRowItem rowItem = e.Section.Items[0] as BaseRowItem;
					if (rowItem != null) {
						if(e.RowNumber %2 == 0){
							rowItem.DrawBorder = true;
							
						} else {
							rowItem.DrawBorder = false;
						}
						if (e.Section.Items.Count > 0){
							foreach(BaseReportItem bri in rowItem.Items) {
								BaseDataItem dbi = bri as BaseDataItem;
								if (dbi != null) {
									if (String.IsNullOrEmpty(dbi.DBValue)) {
										dbi.DBValue = "Misc.";
									}
								}
							}
						}
					}
					break;
					
				case GlobalEnums.ReportSection.ReportPageFooter:
//					System.Console.WriteLine("\tPageFooter");
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:
//					System.Console.WriteLine("\tReportFooter");

					BaseTextItem item1 = new BaseTextItem();
					item1.Text = "Nr of Contributor's:";
					item1.Location =new System.Drawing.Point(70,10);
					item1.Size = new System.Drawing.Size(110,20);
					item1.Font = new System.Drawing.Font("Microsoft Sans Serif",12);
		            item1.ForeColor = System.Drawing.Color.Black;          
					e.Section.Items.Add (item1);
					
					BaseTextItem item = new BaseTextItem();
//					item.Text = rowNr.ToString();
					item.Location =new System.Drawing.Point(180,10);
					item.Size = new System.Drawing.Size(20,20);
					item.Font = new System.Drawing.Font("Microsoft Sans Serif",12);
					
		             item.ForeColor = System.Drawing.Color.Black;          
					e.Section.Items.Add (item);
					break;
					
				default:
					break;
			}

		}
		
		#endregion
		
		#region standart Contributors
		
	
		
		/*
		void DisplayContributors (string fileName,ReportModel model,IList list)
		{
			ICSharpCode.Reports.Core.Exporter.BasePager pageBuilder;
			if (this.btnReportViewer.Checked) {
				this.previewControl1.SetupAsynchron(model,list);
			}else if(this.btnPreviewControl.Checked){
				this.engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(OnRenderContributors);
				this.engine.PreviewPushDataReport (fileName,list,null);
				
			}else if (this.btnPrinter.Checked){
				pageBuilder = engine.CreatePageBuilder(model,list,null);
				pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(OnRenderContributors);
				pageBuilder.BuildExportList();
				using (PrintRenderer printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages)){
					this.OutputToPrinter(printRenderer);
				}
			}else if(this.btnPDF.Checked){
				
				string fN = this.SelectFilename();
				if (!String.IsNullOrEmpty(fN)) {
					
					pageBuilder = engine.CreatePageBuilder(model,list,null);
					pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(OnRenderContributors);
					pageBuilder.BuildExportList();
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder.Pages,fN,true)){
						this.OutputToPdf(pdfRenderer);
					}	
				}
			}
		}
		*/
		#endregion
		
		#region Contributors Customized
		
		void ContributorsCustomizedClick(object sender, EventArgs e)
		{
			/*
			ContributorsList conReport = new ContributorsList();
			this.engine = conReport.Engine;
			this.rowNr = 0;
			this.CustomizedContributors(conReport.ReportName,conReport.ReportModel,conReport.Contributors);
			*/
			MessageBox.Show("Not implemented");
		}
		
		
		/*
		void CustomizedContributors (string fileName,ReportModel model,IList list) 
		{
			ICSharpCode.Reports.Core.Exporter.BasePager pageBuilder;
			if (this.btnReportViewer.Checked) {
				this.previewControl1.SetupAsynchron(model,list);
			}else if(this.btnPreviewControl.Checked){
				engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(OnRenderContributors);
				engine.PreviewPushDataReport (fileName,list,null);
				
			}else if (this.btnPrinter.Checked){
				pageBuilder = engine.CreatePageBuilder(model,list,null);
				pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(OnRenderContributors);
				pageBuilder.BuildExportList();
				using (PrintRenderer printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages)){
					this.OutputToPrinter(printRenderer);
				}
			}else if(this.btnPDF.Checked){
				
				string fN = this.SelectFilename();
				if (!String.IsNullOrEmpty(fN)) {
					pageBuilder = engine.CreatePageBuilder(model,list,null);
					pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(OnRenderContributors);
					pageBuilder.BuildExportList();
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder.Pages,fN,true)){
						this.OutputToPdf(pdfRenderer);
					}
				}
			}
		}
		
		
		
		int rowNr;
		private void OnRenderContributors (object sender,	SectionRenderEventArgs e)
		{

			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
					this.rowNr ++;
				
					BaseRowItem rowItem = e.Section.Items[0] as BaseRowItem;
					if (rowItem != null) {
						
						if(e.RowNumber %2 == 0){
							rowItem.BackColor = System.Drawing.Color.LightGray;
						} else {
							rowItem.BackColor = System.Drawing.Color.White;
						}
						
						if (e.Section.Items.Count > 0){
							foreach(BaseReportItem baseReportItem in rowItem.Items) {
								BaseDataItem baseDataItem = baseReportItem as BaseDataItem;
								if (baseDataItem != null) {
									if (String.IsNullOrEmpty(baseDataItem.DBValue)) {
										baseDataItem.DBValue = "Misc.";
									}
								}
							}
						}
					}	
				
					break;
					
				case GlobalEnums.ReportSection.ReportPageFooter:
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:

					BaseTextItem item1 = new BaseTextItem();
					item1.Text = "Nr of Contributor's:";
					item1.Location =new System.Drawing.Point(70,10);
					item1.Size = new System.Drawing.Size(200,20);
					item1.Font = new System.Drawing.Font("Microsoft Sans Serif",12);
					item1.BackColor = System.Drawing.Color.AliceBlue;
					item1.ForeColor = System.Drawing.Color.Black;
					e.Section.Items.Add (item1);
					
					BaseTextItem item = new BaseTextItem();
					item.Text = rowNr.ToString();
					item.Location =new System.Drawing.Point(300 ,10);
				
					item.Font = new System.Drawing.Font("Microsoft Sans Serif",12);
					item.BackColor = System.Drawing.Color.AliceBlue;
					item.ForeColor = System.Drawing.Color.Black;

					e.Section.Items.Add (item);
					break;
					
				default:
					break;
			}
		}
		
		*/
		#endregion
		
		#region SaleybyYear with parameters
		void SaleyByYearWithParameters(object sender, EventArgs e)
		{
		/*
			NorthwindSalesbyYear nws = new NorthwindSalesbyYear();
			this.engine = nws.Engine;
			this.CustomizedSalesbyYear(nws.ReportName,nws.Parameters,null,null);
			*/
			MessageBox.Show("Not implemented");
		}
	
		/*
		void CustomizedSalesbyYear (string fileName,ReportParameters parameters,ReportModel model,DataManager dataManager) {
			ICSharpCode.Reports.Core.Exporter.BasePager pageBuilder;
			
			if (this.btnReportViewer.Checked) {
				if (parameters == null) {
					this.previewControl1.SetupAsynchron(model,dataManager);
				} else {
					this.previewControl1.SetupAsynchron(model,parameters);
				}
				
			} else if(this.btnPreviewControl.Checked){
				// send the report to the standart Windows.Forms.PreviewControl
				engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(NorthwindSalesbyYear.OnRenderSalesByYear);
				this.RunStandartPreview(engine,fileName,parameters);
			}
			
			else if (this.btnPrinter.Checked){
				// send report directly to the printer using the same code as we use in creating the pdf files
				stopWatch = new System.Diagnostics.Stopwatch();
				stopWatch.Start();
				
				pageBuilder = this.CreatePageBuilder(engine,fileName,parameters);
				pageBuilder.BuildExportList();
				pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(NorthwindSalesbyYear.OnRenderSalesByYear);
				using (PrintRenderer printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages)){
					this.OutputToPrinter(printRenderer);
				}
				this.ShowTime(pageBuilder.Pages.Count);
			}else if(this.btnPDF.Checked){
				string fN = this.SelectFilename();
				if (!String.IsNullOrEmpty(fN)) {
					stopWatch = new System.Diagnostics.Stopwatch();
					stopWatch.Start();
					
					pageBuilder = this.CreatePageBuilder(engine,fileName,parameters);
					pageBuilder.Rendering += new EventHandler<SectionRenderEventArgs>(NorthwindSalesbyYear.OnRenderSalesByYear);
					pageBuilder.BuildExportList();
					
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder.Pages,fN,true)){
						this.OutputToPdf(pdfRenderer);
						this.ShowTime(pageBuilder.Pages.Count);
					}
					
				}
			}
		}
		*/
		
		#endregion
		
		
		/*
		void DisplayPushDataStandart (string fileName,ReportModel model,DataTable dataTable) 
		{
			ICSharpCode.Reports.Core.Exporter.BasePager pageBuilder;
			if (this.btnReportViewer.Checked) {
				this.previewControl1.SetupAsynchron(model,dataTable);
				
			} else if(this.btnPreviewControl.Checked){
				// send the report to the standart Windows.Forms.PreviewControl
				engine.PreviewPushDataReport(fileName,dataTable,null);
			}else if (this.btnPrinter.Checked){
				stopWatch = new System.Diagnostics.Stopwatch();
				stopWatch.Start();
				
				pageBuilder = engine.CreatePageBuilder(model,dataTable,null);
				pageBuilder.BuildExportList();
				using (PrintRenderer printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages)){
					this.OutputToPrinter(printRenderer);
				}
				this.ShowTime(pageBuilder.Pages.Count);
				
			}else if(this.btnPDF.Checked){
				string fN = this.SelectFilename();
				if (!String.IsNullOrEmpty(fN)) {
					stopWatch = new System.Diagnostics.Stopwatch();
					stopWatch.Start();
					
					pageBuilder = engine.CreatePageBuilder(model,dataTable,null);
					pageBuilder.BuildExportList();
					
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder.Pages,fN,true)){
						this.OutputToPdf(pdfRenderer);
						this.ShowTime(pageBuilder.Pages.Count);
					}
				}
			}
		}
		*/
	
		void DisplayPullData (string fileName,ReportParameters parameters) 
		{
			ICSharpCode.Reports.Core.IReportCreator pageBuilder;
			
			if (this.btnReportViewer.Checked) {
				this.previewControl1.SetupAsynchron(fileName,parameters);
			} else if(this.btnPreviewControl.Checked){
				// send the report to the standart Windows.Forms.PreviewControl
				this.RunStandartPreview(engine,fileName,parameters);
			}
			
			else if (this.btnPrinter.Checked){
				// send report directly to the printer using the same code as we use in creating the pdf files
				pageBuilder = ReportEngine.CreatePageBuilder(fileName,parameters);
				pageBuilder.BuildExportList();
				PrintRenderer printRenderer = null;
				
				using (PrintDialog dlg = new PrintDialog()) {
					DialogResult result = dlg.ShowDialog();
					if (result==DialogResult.OK){
						printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages,dlg.PrinterSettings);
					} else {
						printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages);
					}
				}
				
				this.OutputToPrinter (printRenderer);
				printRenderer.Dispose();
				
				
			}else if(this.btnPDF.Checked){
				//Output to PdfFile
				string saveTo = this.SelectFilename();
				if (!String.IsNullOrEmpty(fileName)) {

					pageBuilder = ReportEngine.CreatePageBuilder(fileName,parameters);
					pageBuilder.BuildExportList();
					
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder,saveTo,true)){                                                   
						this.OutputToPdf(pdfRenderer);
					}
				}	
			}
		}
		
	
		
		/*
		void DisplayFormSheet (string fileName)
		{
			ICSharpCode.Reports.Core.Exporter.BasePager pageBuilder;
			
			if (this.btnReportViewer.Checked) {
				this.previewControl1.SetupAsynchron(fileName,null);
				
			} else if(this.btnPreviewControl.Checked){
				this.RunStandartPreview(engine,fileName,null);
			}
		
			else if (this.btnPrinter.Checked){
				pageBuilder = engine.CreatePageBuilder(fileName,null);
				pageBuilder.BuildExportList();
				PrintRenderer printRenderer = null;
				
				using (PrintDialog dlg = new PrintDialog()) {
					DialogResult result = dlg.ShowDialog();
					if (result==DialogResult.OK){
						printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages,dlg.PrinterSettings);
					} else {
						printRenderer = PrintRenderer.CreateInstance(pageBuilder.Pages);
					}
				}
				this.OutputToPrinter (printRenderer);
				printRenderer.Dispose();
				
			}else if(this.btnPDF.Checked){
				string fN = this.SelectFilename();
				if (!String.IsNullOrEmpty(fN)) {
					pageBuilder = engine.CreatePageBuilder(fileName,null);
					pageBuilder.BuildExportList();
					
					// set to false means no PdfWindow
					using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder.Pages,fN,true)){
						this.OutputToPdf(pdfRenderer);
					}
				}
			}		
		}
		*/
		
		private IReportCreator CreatePageBuilder (string fileName,
		                                          ReportParameters parameters)                                          
		{
		
				return CreatePageBuilder(fileName,parameters);
		}
		
		
		private void RunStandartPreview(ReportEngine engine,string fileName,ReportParameters parameters)
		{
			engine.PreviewStandardReport(fileName,parameters);
		}
		
		
		private void OutputToPdf (ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfRenderer renderer)
		{
			renderer.Start();
			renderer.RenderOutput();
			renderer.End();
		}
		
		
		private void OutputToPrinter (ICSharpCode.Reports.Core.Exporter.ExportRenderer.PrintRenderer renderer)
		{
			renderer.Start();
			renderer.RenderOutput();
			renderer.End();
		}
		
		
		
		private string SelectFilename() 
		{
			using (SaveFileDialog saveDialog = new SaveFileDialog()){

				saveDialog.FileName = "_pdf";
				saveDialog.DefaultExt = "PDF";
				saveDialog.ValidateNames = true;
				if(saveDialog.ShowDialog() == DialogResult.OK){
					return saveDialog.FileName;
				} else {
					return String.Empty;
				}
			}
		}
	}
}






