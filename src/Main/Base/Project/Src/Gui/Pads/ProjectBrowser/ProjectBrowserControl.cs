using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectBrowserControl.
	/// </summary>
	public class ProjectBrowserControl : System.Windows.Forms.UserControl
	{
		ExtTreeView treeView;
		static Dictionary<Image, int> projectBrowserImageIndex = new Dictionary<Image, int>();
		
		public bool ShowAll {
			get {
				return AbstractProjectBrowserTreeNode.ShowAll;
			}
			set {
				if (AbstractProjectBrowserTreeNode.ShowAll != value) {
					AbstractProjectBrowserTreeNode.ShowAll = value;
					foreach (AbstractProjectBrowserTreeNode node in treeView.Nodes) {
						node.UpdateVisibility();
					}
					treeView.SortNodes(null);
				}
			}
		}
		
		static ProjectBrowserControl()
		{
		}
		
		public AbstractProjectBrowserTreeNode SelectedNode {
			get {
				return treeView.SelectedNode as AbstractProjectBrowserTreeNode;
			}
		}
		
		public ExtTreeView TreeView {
			get {
				return treeView;
			}
		}
		
		public ProjectBrowserControl()
		{
			InitializeComponent();
			treeView.AfterSelect       += new TreeViewEventHandler(TreeViewAfterSelect);
			FileService.FileRenaming   += new FileRenameEventHandler(FileServiceFileRenaming);
			FileService.FileRemoving   += new FileEventHandler(FileServiceFileRemoving);

			ProjectService.ReferenceAdded += new ProjectReferenceEventHandler(ProjectServiceReferenceAdded);
			ProjectService.SolutionFolderRemoved += new SolutionFolderEventHandler(ProjectServiceSolutionFolderRemoved);
		}
		
		void CallVisitor(ProjectBrowserTreeNodeVisitor visitor)
		{
			foreach (AbstractProjectBrowserTreeNode treeNode in treeView.Nodes) {
				treeNode.AcceptVisitor(visitor, null);
			}
		}
		void ProjectServiceSolutionFolderRemoved(object sender, SolutionFolderEventArgs e)
		{
			Console.WriteLine("projectbrowser: Solution folder remove!!!");
			CallVisitor(new SolutionFolderRemoveVisitor(e.SolutionFolder));
			Console.WriteLine("projectbrowser: Solution folder remove done.!!!");
		}
		void ProjectServiceReferenceAdded(object sender, ProjectReferenceEventArgs e)
		{
			CallVisitor(new UpdateReferencesVisitor(e));
		}
		
		void FileServiceFileRemoving(object sender, FileEventArgs e)
		{
			Console.WriteLine("projectbrowser: Solution file removing!!!");
			CallVisitor(new FileRemoveTreeNodeVisitor(e.FileName));
			Console.WriteLine("projectbrowser: Solution file removing!!!");
		}
		
		void FileServiceFileRenaming(object sender, FileRenameEventArgs e)
		{
			Console.WriteLine("projectbrowser: Solution file renaming!!!");
			CallVisitor(new FileRenameTreeNodeVisitor(e.SourceFile, e.TargetFile));
			Console.WriteLine("projectbrowser: Solution file renaming!!!");
		}
		
		void SelectFile(ProjectNode projectNode, string fileName)
		{
			string relativeName = FileUtility.GetRelativePath(projectNode.Directory, fileName);
			string file         = Path.GetFileName(relativeName);
		}
		
		public void RefreshView()
		{
			// TODO implement refresh.
		}
		
		public void SelectFile(string fileName)
		{
			if (treeView.Nodes.Count == 0) {
				return;
			}
			
			SolutionNode solutionNode = treeView.Nodes[0] as SolutionNode;
			if (solutionNode!= null) {
				foreach (object o in solutionNode.Nodes) {
					ProjectNode projectNode = o as ProjectNode;
					if (projectNode != null && projectNode.Project.IsFileInProject(fileName)) {
						SelectFile(projectNode, fileName);
						return;
					}
				}
			}
			treeView.SelectedNode         = null;
			ProjectService.CurrentProject = null;
		}
		
		public void ViewSolution(Solution solution)
		{
			AbstractProjectBrowserTreeNode solutionNode = new SolutionNode(solution);
			treeView.Nodes.Clear();
			solutionNode.AddTo(treeView);
			DefaultDotNetNodeBuilder       nodeBuilder = new DefaultDotNetNodeBuilder();
			
			foreach (object treeObject in solution.Folders) {
				if (treeObject is IProject) {
					nodeBuilder.AddProjectNode(solutionNode, (IProject)treeObject);
				} else {
					SolutionFolderNode folderNode = new SolutionFolderNode(solution, (SolutionFolder)treeObject);
					folderNode.AddTo(solutionNode);
				}
			}
			
			solutionNode.Expand();
		}
		
		public void Clear()
		{
			treeView.Clear();
		}
		
		#region Label editing
		
		#endregion
		
		
		void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{ // set current project & current combine
			
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			if (node == null) {
				return;
			}
			ProjectService.CurrentProject = node.Project;
			ICSharpCode.SharpDevelop.Gui.PropertyPad.SetDesignableObject(node.Tag);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView = new ExtTreeView();
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(292, 266);
			this.treeView.TabIndex = 0;
			
			// 
			// ProjectBrowserControl
			// 
			this.Controls.Add(this.treeView);
			this.Name = "ProjectBrowserControl";
			this.Size = new System.Drawing.Size(292, 266);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
