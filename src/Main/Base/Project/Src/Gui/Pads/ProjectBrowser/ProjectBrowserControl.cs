// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class ProjectBrowserControl : System.Windows.Forms.UserControl, IHasPropertyContainer
	{
		ExtTreeView treeView;
		
		public bool ShowAll {
			get {
				return AbstractProjectBrowserTreeNode.ShowAll;
			}
			set {
				if (AbstractProjectBrowserTreeNode.ShowAll != value) {
					treeView.BeginUpdate();
					AbstractProjectBrowserTreeNode.ShowAll = value;
					foreach (AbstractProjectBrowserTreeNode node in treeView.Nodes) {
						node.UpdateVisibility();
					}
					treeView.Sort();
					treeView.EndUpdate();
				}
			}
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
			treeView.BeforeSelect   += TreeViewBeforeSelect;
			FileService.FileRenamed += FileServiceFileRenamed;
			FileService.FileRemoved += FileServiceFileRemoved;
			
			ProjectService.ProjectItemAdded += ProjectServiceProjectItemAdded;
			ProjectService.SolutionFolderRemoved += ProjectServiceSolutionFolderRemoved;
			treeView.DrawNode += TreeViewDrawNode;
		}
		
		void TreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			if (node != null) {
				Image img = node.Overlay;
				if (img != null) {
					Graphics g = e.Graphics;
					g.DrawImageUnscaled(img, e.Bounds.X - img.Width, e.Bounds.Bottom - img.Height);
				}
			}
		}
		
		void CallVisitor(ProjectBrowserTreeNodeVisitor visitor)
		{
			foreach (AbstractProjectBrowserTreeNode treeNode in treeView.Nodes) {
				treeNode.AcceptVisitor(visitor, null);
			}
		}
		void ProjectServiceSolutionFolderRemoved(object sender, SolutionFolderEventArgs e)
		{
			CallVisitor(new SolutionFolderRemoveVisitor(e.SolutionFolder));
		}
		void ProjectServiceProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.ProjectItem is ReferenceProjectItem)  {
				CallVisitor(new UpdateReferencesVisitor(e));
			}
		}
		
		void FileServiceFileRemoved(object sender, FileEventArgs e)
		{
			CallVisitor(new FileRemoveTreeNodeVisitor(e.FileName));
		}
		
		void FileServiceFileRenamed(object sender, FileRenameEventArgs e)
		{
			CallVisitor(new FileRenameTreeNodeVisitor(e.SourceFile, e.TargetFile));
		}
		
		public void RefreshView()
		{
			if (treeView.Nodes.Count > 0) {
				ViewSolution(((AbstractProjectBrowserTreeNode)treeView.Nodes[0]).Solution);
			}
		}
		
		FileNode FindFileNode(TreeNodeCollection nodes, string fileName)
		{
			FileNode fn;
			foreach (TreeNode node in nodes) {
				fn = node as FileNode;
				if (fn != null) {
					if (FileUtility.IsEqualFileName(fn.FileName, fileName))
						return fn;
				}
				fn = FindFileNode(node.Nodes, fileName);
				if (fn != null)
					return fn;
			}
			return null;
		}
		
		/// <summary>
		/// Finds the node of a file in the project browser.
		/// WARNING: this method only finds the node if it already is created. Since the tree
		/// is lazy-loaded, not every file has a node!
		/// </summary>
		public FileNode FindFileNode(string fileName)
		{
			return FindFileNode(treeView.Nodes, fileName);
		}
		
		/// <summary>
		/// Selects the node of a file if it is visible
		/// </summary>
		public void SelectFile(string fileName)
		{
			FileNode node = FindFileNode(fileName);
			if (node != null) {
				// select first parent that is not collapsed
				TreeNode nodeToSelect = node;
				TreeNode p = node.Parent;
				while (p != null) {
					if (!p.IsExpanded)
						nodeToSelect = p;
					p = p.Parent;
				}
				if (nodeToSelect != null) {
					treeView.SelectedNode = nodeToSelect;
				}
			}
		}
		
		public void ViewSolution(Solution solution)
		{
			AbstractProjectBrowserTreeNode solutionNode = new SolutionNode(solution);
			treeView.Nodes.Clear();
			solutionNode.AddTo(treeView);
			
			foreach (object treeObject in solution.Folders) {
				if (treeObject is IProject) {
					NodeBuilders.AddProjectNode(solutionNode, (IProject)treeObject);
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
		
		public void PadActivated()
		{
			TreeViewBeforeSelect(null, new TreeViewCancelEventArgs(treeView.SelectedNode, false, TreeViewAction.Unknown));
		}
		
		void TreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
		{ // set current project & current combine
			
			AbstractProjectBrowserTreeNode node = e.Node as AbstractProjectBrowserTreeNode;
			if (node == null) {
				return;
			}
			ProjectService.CurrentProject = node.Project;
			propertyContainer.SelectedObject = node.Tag;
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
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
