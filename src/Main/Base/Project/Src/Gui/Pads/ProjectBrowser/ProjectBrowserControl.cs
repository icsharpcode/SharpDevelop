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
		
		public AbstractProjectBrowserTreeNode RootNode {
			get {
				return treeView.Nodes[0] as AbstractProjectBrowserTreeNode;
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
			treeView.AfterExpand    += TreeViewAfterExpand;
			FileService.FileRenamed += FileServiceFileRenamed;
			FileService.FileRemoved += FileServiceFileRemoved;
			
			ProjectService.ProjectItemAdded += ProjectServiceProjectItemAdded;
			ProjectService.SolutionFolderRemoved += ProjectServiceSolutionFolderRemoved;
			treeView.DrawNode += TreeViewDrawNode;
			treeView.DragDrop += TreeViewDragDrop;
		}
		
		void TreeViewDragDrop(object sender, DragEventArgs e)
		{
			Point       clientcoordinate = PointToClient(new Point(e.X, e.Y));
			ExtTreeNode node             = treeView.GetNodeAt(clientcoordinate) as ExtTreeNode;
			if (node == null) {
				// did not drag onto any node
				if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
					string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
					foreach (string file in files) {
						try {
							IProjectLoader loader = ProjectService.GetProjectLoader(file);
							if (loader != null) {
								FileUtility.ObservedLoad(new NamedFileOperationDelegate(loader.Load), file);
							} else {
								FileService.OpenFile(file);
							}
						} catch (Exception ex) {
							MessageService.ShowError(ex, "unable to open file " + file);
						}
					}
				}
			}
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
				if (node != null) {
					fn = FindFileNode(node.Nodes, fileName);
					if (fn != null)
						return fn;
				}
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
		
		// stores the fileName of the last selected target so
		// that we can select it again on opening a folder
		string lastSelectionTarget;
		
		/// <summary>
		/// Selects the deepest node open on the path to a particular file.
		/// </summary>
		public void SelectFile(string fileName)
		{
			lastSelectionTarget = fileName;
			TreeNode node = FindFileNode(fileName);
			
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
			} else {
				SelectDeepestOpenNodeForPath(fileName);
			}
		}

		#region SelectDeepestOpenNode internals
//
//		SolutionNode RootSolutionNode {
//			get {
//				if (treeView.Nodes != null && treeView.Nodes.Count>0) {
//					return treeView.Nodes[0] as SolutionNode;
//				}
//				return null;
//			}
//		}
//
		void SelectDeepestOpenNodeForPath(string fileName)
		{
			TreeNode node = FindDeepestOpenNodeForPath(fileName);
			if (node != null) {
				treeView.SelectedNode = node;
			}
		}
		
		TreeNode FindDeepestOpenNodeForPath(string fileName)
		{
			LoggingService.DebugFormatted("Finding Deepest for '{0}'", fileName);
			Solution solution = ProjectService.OpenSolution;
			if (solution == null) {
				return null;
			}

			IProject project = solution.FindProjectContainingFile(fileName);
			if (project == null) {
				LoggingService.Debug("no IProject found");
				return null;
			}

			string relativePath = String.Empty;
			TreeNode targetNode = FindProjectNode(project);

			if (targetNode == null) {
				
				// our project node is not yet created,
				// so start at the root and work down.
				
				if (treeView.Nodes == null || treeView.Nodes.Count<1) {
					// the treeView is not yet prepared to assist in this request.
					return null;
					
				} else {
					targetNode = treeView.Nodes[0];
					if (fileName.StartsWith(solution.Directory)) {
						relativePath = fileName.Replace(solution.Directory, "");
					}
				}
				
			} else {
				// start from the project node and work upwards
				// to the first visible node
				TreeNode t = targetNode;
				TreeNode p = targetNode.Parent;
				while (p != null) {
					if (!p.IsExpanded) {
						t = p;
					}
					p = p.Parent;
				}
				
				if (t != targetNode) {
					// project node is instantiated but not visible
					// so select the most visible parent node.
					return t;

				} else {
					// project node is instantiated and visible
					// so we start here and work down
					if (fileName.StartsWith((targetNode as ProjectNode).Directory)) {
						relativePath = fileName.Replace((targetNode as ProjectNode).Directory, "");
					}
				}
				
			}
			
			if (!targetNode.IsExpanded) {
				// the targetNode is not expanded so it's as deep as we can go
				LoggingService.DebugFormatted("target node '{0};{1}' is not expanded.", targetNode, targetNode.Text);
				return targetNode;
			}

			LoggingService.Debug("entering depth loop...");
			LoggingService.DebugFormatted(@"\- looking for '{0}'", relativePath);
			LoggingService.DebugFormatted(@"\- starting at '{0}'", targetNode != null ? targetNode.Text : "null");
			
			string[] targets = relativePath.Trim('/','\\').Split('/', '\\');
			TreeNode nextNode = null;
			foreach (string target in targets) {
				LoggingService.Debug("-- looking for: "+target);
				nextNode = null;
				foreach (TreeNode node in targetNode.Nodes) {
					if (node == null) {
						// can happen when the node is currently expanding
						continue;
					}
					if (node.Text == target) {
						nextNode = node;
						break;
					}
				}
				if (nextNode == null) {
					// targetNode is as deep as we can find
					break;
				} else {
					targetNode = nextNode;
				}
			}
			return targetNode;
		}

		ProjectNode FindProjectNode(IProject project)
		{
			if (project == null) {
				return null;
			}
			return FindProjectNodeByName(treeView.Nodes, project.Name);
		}
		
		// derived from FindFileNode
		ProjectNode FindProjectNodeByName(TreeNodeCollection nodes, string projectName)
		{
			if (nodes == null) {
				return null;
			}
			ProjectNode pn;
			foreach (TreeNode node in nodes) {
				if (node == null) {
					// can happen while parent node is being expanded
					continue;
				}
				pn = node as ProjectNode;
				if (pn != null) {
					if (pn.Text == projectName) {
						return pn;
					}
				}
				pn = FindProjectNodeByName(node.Nodes, projectName);
				if (pn != null)
					return pn;
			}
			return null;
		}
		
		// TODO: remove this debug code
//		void LogTreeViewPaths(TreeNodeCollection nodes, int depth)
//		{
//			System.Text.StringBuilder sb = null;
//
//			foreach (TreeNode node in nodes) {
//				sb = new System.Text.StringBuilder();
//				for(int i = 0; i<depth; i++) {
//					sb.Append("--");
//				}
//				sb.Append(node.Text+" ("+node.GetType().Name+")");
//				LoggingService.Debug(sb.ToString());
//				LogTreeViewPaths(node.Nodes, depth+1);
//			}
//		}
		#endregion
		
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
		
		void TreeViewAfterExpand(object sender, TreeViewEventArgs e)
		{
			// attempt to restore the last selection if its path has been reexpanded
			if (lastSelectionTarget != null) {
				TreeNode node = FindDeepestOpenNodeForPath(lastSelectionTarget);
				while (node != null) {
					if (node.Parent == e.Node) {
						treeView.SelectedNode = node;
						break;
					} else {
						node = node.Parent;
					}
				}
			}
		}
		
		void TreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			// set current project & current combine
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
