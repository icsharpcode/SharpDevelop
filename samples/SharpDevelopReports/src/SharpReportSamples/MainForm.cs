/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.01.2010
 * Zeit: 17:43
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;

namespace SharpReportSamples
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		
		private TreeNode formNode;
		private TreeNode pullNode;
		private TreeNode iListNode;
		private TreeNode providerIndependent;
		private TreeNode customized;
		private ImageList imageList;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			InitTree();
			UpdateStatusbar (Application.StartupPath);
			this.previewControl1.Messages = new ReportViewerMessagesProvider();
		}
		
		
		private void InitTree ()
		{
			string formSheetDir = @"\FormSheet\JCA.srd";
			
			string startupPath = Application.StartupPath;
			string samplesDir = @"SharpDevelopReports\";
			int i = startupPath.IndexOf(samplesDir);
			string startPath = startupPath.Substring(0,i + samplesDir.Length) + @"SampleReports\";
			
			
			string pathToFormSheet = startPath + formSheetDir;
			
			this.formNode = this.treeView1.Nodes[0].Nodes[0];
			this.pullNode =  this.treeView1.Nodes[0].Nodes[1];
			this.iListNode = this.treeView1.Nodes[0].Nodes[2];
			this.providerIndependent = this.treeView1.Nodes[0].Nodes[3];
			this.customized = this.treeView1.Nodes[0].Nodes[4];
			
			AddNodesToTree (this.formNode,startPath + @"FormSheet\" );
			AddNodesToTree (this.pullNode,startPath + @"PullModel\" );
			AddNodesToTree (this.iListNode,startPath + @"IList\" );
			AddNodesToTree (this.providerIndependent,startPath + @"ProviderIndependent\" );
			AddNodesToTree (this.customized,startPath + @"Customized\" );
		}
		
		
		private void AddNodesToTree (TreeNode parent,string path)
		{
			if (!Directory.Exists(path)) {
				return;
			}
			string[] filePaths = Directory.GetFiles(path, "*.srd");
			TreeNode reportNode = null;
			foreach (string fullPath in filePaths){
				string fileName = Path.GetFileNameWithoutExtension(fullPath);
				reportNode = new TreeNode(fileName);
				reportNode.Tag = fullPath;
				parent.Nodes.Add(reportNode);
			}
		}
		
		
		
		private void UpdateStatusbar (string text)
		{
			this.label1.Text = text;
		}
		
		
		private void RunStandardReport(string reportName)
		{
			string s = Path.GetFileNameWithoutExtension(reportName);
			if (s == "ContributorsList" ) {
				this.RunContributors(reportName);
			} else if (s == "NoConnectionReport") {
				this.RunProviderIndependent(reportName);
			} else if (s =="EventLog")
			this.RunEventLogger(reportName);
//			this.RunEventLogger_Pdf(reportName);
			else {
				
				ReportParameters parameters =  ReportEngine.LoadParameters(reportName);
				
				if ((parameters != null)&& (parameters.SqlParameters.Count > 0)){
					parameters.SqlParameters[0].ParameterValue = "I'm the Parameter";
				}
				this.previewControl1.PreviewLayoutChanged += delegate (object sender, EventArgs e)
				{
					this.RunStandardReport(reportName);
				};
				this.previewControl1.RunReport(reportName,parameters);
			}
		}
		
		
		#region ProviderIndependent
		private void RunProviderIndependent (string reportName)
		{
			string conOleDbString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\SharpReport_TestReports\TestReports\Nordwind.mdb;Persist Security Info=False";
			ReportParameters parameters =  ReportEngine.LoadParameters(reportName);
			ConnectionObject con = ConnectionObject.CreateInstance(conOleDbString,
			                                                       System.Data.Common.DbProviderFactories.GetFactory("System.Data.OleDb") );
			
			parameters.ConnectionObject = con;
			parameters.SqlParameters[0].ParameterValue = "Provider Independent";
			this.previewControl1.PreviewLayoutChanged += delegate (object sender, EventArgs e)
			{
				this.RunProviderIndependent(reportName);
			};
			this.previewControl1.RunReport(reportName,parameters);
		}
		
		
		#endregion
		
		#region Contributors
		//
		/// <summary>
		/// Some values in the Datastructure are not set (means they are null), you can handle this values by setting
		/// the NullValue in the properties of this Item, or, you can use the SectionRenderingEvent as shown
		/// below
		/// </summary>
		/// <param name="fileName"></param>
		private void RunContributors (string fileName)
		{
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			
			// sorting is done here, but, be carefull, misspelled fieldnames will cause an exception
			
			//ReportSettings settings = model.ReportSettings;
			//settings.SortColumnCollection.Add(new SortColumn("First",System.ComponentModel.ListSortDirection.Ascending));
		
			// Both variable declarations  are valid
			
			ContributorCollection contributorCollection = ContributorsReportData.CreateContributorsList();
			IDataManager dataManager = DataManager.CreateInstance(contributorCollection,model.ReportSettings);
			
//			List<Contributor> list = ContributorsReportData.CreateContributorsList();
//			IDataManager dm = DataManager.CreateInstance(list,model.ReportSettings);
			
			
			this.previewControl1.PreviewLayoutChanged += delegate (object sender, EventArgs e)
			{
				this.previewControl1.RunReport(model,dataManager);
			};
			this.previewControl1.RunReport(model,dataManager);
		}
		
		
		
		private void RunEventLogger_Pdf (string fileName)
		{
			Cursor.Current = Cursors.WaitCursor;
			EventLogger eventLogger = new EventLogger(fileName);
			Cursor.Current = Cursors.Default;

			this.imageList = eventLogger.Images;
			
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			IReportCreator creator = ReportEngine.CreatePageBuilder(model,eventLogger.EventLog,null);
			creator.SectionRendering += PushPrinting;
			creator.BuildExportList();
			using (PdfRenderer pdfRenderer = PdfRenderer.CreateInstance(creator,SelectFilename(),true))
			{
				pdfRenderer.Start();
				pdfRenderer.RenderOutput();
				pdfRenderer.End();
			}
		}
		
		
		private void RunEventLogger (string fileName)
		{
			/*
			using (var provider = ProfilingDataSQLiteProvider.FromFile("ProfilingSession.sdps")){
				var functions = provider.GetFunctions(0, provider.DataSets.Count - 1);
				foreach (CallTreeNode n in functions) {
					Console.WriteLine("{0}: {1} calls, {2:f2}ms", n.Name, n.CallCount, n.TimeSpent);
				}
				
			}
			*/
			Cursor.Current = Cursors.WaitCursor;
			EventLogger eLog = new EventLogger(fileName);
			Cursor.Current = Cursors.Default;

			this.imageList = eLog.Images;
			
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			IDataManager dataManager = DataManager.CreateInstance(eLog.EventLog,model.ReportSettings);
			
			this.previewControl1.SectionRendering += PushPrinting;
			
			this.previewControl1.PreviewLayoutChanged += delegate (object sender, EventArgs e)
		
			{
				this.previewControl1.RunReport(model,dataManager);
			};
			this.previewControl1.RunReport(model,dataManager);
		}
		
		
		//Handles  SectionRenderEvent
		int hour = 0;
		
		private void PushPrinting (object sender, SectionRenderEventArgs e )
		{
			string sectionName = e.Section.Name;
			
			if (sectionName == ReportSectionNames.ReportHeader) {
				Console.WriteLine("PushPrinting  :" + ReportSectionNames.ReportHeader);
			} 
			
			else if (sectionName == ReportSectionNames.ReportPageHeader) {
				Console.WriteLine("PushPrinting :" +ReportSectionNames .ReportPageHeader);
			} 
			
			else if (sectionName == ReportSectionNames.ReportDetail){
				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportDetail);
				// TimeWritten
				BaseDataItem time = e.Section.FindItem("BaseDataItem1") as BaseDataItem;
				if (time != null) {
					DateTime dateTime = Convert.ToDateTime(time.DBValue);
					
					int newhour = dateTime.Hour;
					if (hour != newhour) {
						hour = newhour;
						e.Section.Items[0].DrawBorder = true;
						e.Section.Items[0].FrameColor = Color.Black;
//						e.Section.Items[0].BackColor = Color.LightGray;
						time.DBValue = dateTime.Hour.ToString();
						time.ContentAlignment = ContentAlignment.MiddleLeft;
					
					} else {
						time.DrawBorder = false;
						e.Section.Items[0].FrameColor = Color.White;
//						e.Section.Items[0].BackColor = Color.White;
						time.DBValue = dateTime.Minute.ToString() + ":" + dateTime.Second.ToString();
						time.ContentAlignment = ContentAlignment.MiddleRight;
						
					}
				}
//				D:\SharpDevelop3.0_WorkingCopy\SharpDevelop\samples\SharpDevelopReports\SampleReports\EventLogger\Error.png
				//  Image
				BaseDataItem dataItem = e.Section.FindItem("EntryType") as BaseDataItem;
				if (dataItem != null) {
					string str = dataItem.DBValue;
					Image image = null;
					if (str == "Information") {
						image = this.imageList.Images[1];
					} else if (str == "Warning") {
						image = this.imageList.Images[2];
					} else if (str == "Error")
					{
						image = this.imageList.Images[0];
					}
					
					if (image != null)
					{
						BaseImageItem imageItem = e.Section.FindItem("BaseImageItem1") as BaseImageItem;
						if (imageItem != null) {
							imageItem.Image = image;
						}
					}
				}
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
		
		#endregion
		

		
		void TreeView1MouseDoubleClick(object sender, MouseEventArgs e)
		{
			TreeNode selectedNode = this.treeView1.SelectedNode;
			if (selectedNode != null) {
				RunStandardReport(selectedNode.Tag.ToString());
			}
		}
		
		/*
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
		
		 */
	}
}
