/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 03.01.2010
 * Zeit: 17:43
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;

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
			int y = startupPath.IndexOf(samplesDir);
			string startPath = startupPath.Substring(0,y + samplesDir.Length) + @"SampleReports\";
			
//D:\Reporting3.0_branches\SharpDevelop\samples\SharpDevelopReports\SampleReports
			
			string pathToFormSheet = startPath + formSheetDir;
			
			this.formNode = this.treeView1.Nodes[0].Nodes[0];
			this.pullNode =  this.treeView1.Nodes[0].Nodes[1];
			this.iListNode = this.treeView1.Nodes[0].Nodes[2];
			this.providerIndependent = this.treeView1.Nodes[0].Nodes[3];
			
			AddNodesToTree (this.formNode,startPath + @"FormSheet\" );
			AddNodesToTree (this.pullNode,startPath + @"PullModel\" );
			AddNodesToTree (this.iListNode,startPath + @"IList\" );
			AddNodesToTree (this.providerIndependent,startPath + @"ProviderIndependent\" );

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
			}
			
			
			
			else {
				
				ReportParameters parameters =  ReportEngine.LoadParameters(reportName);
				
				if ((parameters != null)&& (parameters.SqlParameters.Count > 0)){
					parameters.SqlParameters[0].ParameterValue = "I'm the Parameter";
				}
				
				this.previewControl1.SetupAsynchron(reportName,parameters);
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
			this.previewControl1.SetupAsynchron(reportName,parameters);
		}
			
			
		#endregion
		
		#region Contributors
		//
		
		private void RunContributors (string fileName)
		{
			ReportModel model = ReportEngine.LoadReportModel(fileName);

			// Both variable declarations  are valid
			
			ContributorCollection list = ContributorsReportData.CreateContributorsList();
			IDataManager dm = DataManager.CreateInstance(list,model.ReportSettings);
			
//			List<Contributor> list = ContributorsReportData.CreateContributorsList();
//			IDataManager dm = DataManager.CreateInstance(list,model.ReportSettings);
			
			this.previewControl1.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			this.previewControl1.SetupAsynchron(model,dm);
			
		}
		
		
		
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
						BaseDataItem r = (BaseDataItem)ri.Items.Find("unbound3");
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
		
		*/
		
	}
}
