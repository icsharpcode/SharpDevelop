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
		private TreeNode pushNode;
		private TreeNode iListNode;
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			InitTree();
			UpdateStatusbar (Application.StartupPath);
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
			this.pushNode =  this.treeView1.Nodes[0].Nodes[2];
			this.iListNode = this.treeView1.Nodes[0].Nodes[3];
			
			AddNodesToTree (this.formNode,startPath + @"FormSheet\" );
			AddNodesToTree (this.pullNode,startPath + @"PullModel\" );
			AddNodesToTree (this.pushNode,startPath + @"PushModel\" );
			AddNodesToTree (this.iListNode,startPath + @"IList\" );
			

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
			} else {
				
				ReportParameters parameters =  ReportEngine.LoadParameters(reportName);
				
				if ((parameters != null)&& (parameters.SqlParameters.Count > 0)){
					parameters.SqlParameters[0].ParameterValue = "I'm the Parameter";
				}
				
				this.previewControl1.SetupAsynchron(reportName,parameters);
			}
		}
		
		
		#region Contributors
		//
		
		private void RunContributors (string fileName)
		{
			ReportModel model = ReportEngine.LoadReportModel(fileName);
			ReportEngine engine = new ReportEngine();
			
			engine.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
			// Both variable declarations  are valid
			
			ContributorCollection list = ContributorsReportData.CreateContributorsList();
			IDataManager dm = DataManager.CreateInstance(list,model.ReportSettings);
			
//			List<Contributor> list = ContributorsReportData.CreateContributorsList();
//			IDataManager dm = DataManager.CreateInstance(list,model.ReportSettings);
			
			this.previewControl1.SetupAsynchron(model,dm);
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
		
		#endregion
		
		
		private void SelectReport ()
		{ 
			TreeNode selectedNode = this.treeView1.SelectedNode;
			if ((selectedNode == null)|| (selectedNode.Tag == null)) {
				return;
			}
			
			if (!String.IsNullOrEmpty(selectedNode.Tag.ToString())) {
				if (selectedNode.Parent == this.pushNode) {
					MessageBox.Show("PushModel reports not implemented yet");
				} else {
					RunStandardReport(selectedNode.Tag.ToString());
				}
			}
		}
		
		
		
		void TreeView1MouseDoubleClick(object sender, MouseEventArgs e)
		{
			SelectReport();
		}
		
		
	}
}
