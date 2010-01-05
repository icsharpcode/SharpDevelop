/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 18.11.2009
 * Zeit: 19:29
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Windows.Forms;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;

namespace TestProject
{
	/// <summary>
	/// Description of Form2.
	/// </summary>
	public partial class Form2 : Form
	{
		
//		string reportName = @"D:\SharpReport_TestReports\TestReports\FirstAggFunction.srd";
		string reportName = @"D:\Reportdesigner3.0\TestProject\NoConnectionReport.srd";
		string conOleDbString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\SharpReport_TestReports\TestReports\Nordwind.mdb;Persist Security Info=False";
		ReportParameters parameters;
		IReportCreator pageBuilder;
		
		public Form2()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//D:\SharpReport_TestReports\TestReports
		}
		
		
		void Button1Click(object sender, EventArgs e)
		{
			parameters =  ReportEngine.LoadParameters(reportName);
			ConnectionObject con = ConnectionObject.CreateInstance(this.conOleDbString,
			                                                       System.Data.Common.DbProviderFactories.GetFactory("System.Data.OleDb") );
			
			parameters.ConnectionObject = con;
			ReportEngine engine = new ReportEngine();
			engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			this.previewControl1.SetupAsynchron(reportName,parameters);
		}
		
		
		private void PushPrinting (object sender,SectionRenderEventArgs e) {

			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
					BaseRowItem ri = e.Section.Items[0] as BaseRowItem;
//					if (ri != null) {
//						BaseDataItem r = (BaseDataItem)ri.Items.Find("unbound1");
//						if (r != null) {
//							System.Console.WriteLine("ubound1");
//				
//						}
//					}
					break;
					
				case GlobalEnums.ReportSection.ReportPageFooter:
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:
					break;
					
				default:
					break;
			}
		}
		void Button2Click(object sender, EventArgs e)
		{
			// get Filename to save *.pdf
			string saveTo = this.SelectFilename();
			
			// Create connectionobject
			parameters =  ReportEngine.LoadParameters(reportName);
			ConnectionObject con = ConnectionObject.CreateInstance(this.conOleDbString,
			                                                       System.Data.Common.DbProviderFactories.GetFactory("System.Data.OleDb") );
			
			parameters.ConnectionObject = con;
			
			
			// create a Pagebuilder
			pageBuilder = ReportEngine.CreatePageBuilder(reportName,parameters);
			pageBuilder.BuildExportList();
		
			using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(pageBuilder,saveTo,true)){
				pdfRenderer.Start();
				pdfRenderer.RenderOutput();
				pdfRenderer.End();
			}
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
