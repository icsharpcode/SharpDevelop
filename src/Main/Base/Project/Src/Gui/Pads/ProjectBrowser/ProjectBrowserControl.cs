// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
		
		/// <summary>
		/// The selected node or its most imediate parent that is a DirectoryNode.
		/// </summary>
		/// <seealso cref="DirectoryNode"/>
		public DirectoryNode SelectedDirectoryNode {
			get {
				TreeNode selectedNode =
					treeView.SelectedNode as AbstractProjectBrowserTreeNode;
				DirectoryNode node = null;
				while (selectedNode != null && node == null) {
					node = selectedNode as DirectoryNode;
					selectedNode = selectedNode.Parent;
				}
				// If no solution is load theres a valid reason for this to return null.
				return node;
			}
		}
		
		public AbstractProjectBrowserTreeNode SelectedNode {
			get {
				return treeView.SelectedNode as AbstractProjectBrowserTreeNode;
			}
		}
		
		public AbstractProjectBrowserTreeNode RootNode {
			get {
				if (treeView.Nodes.Count > 0)
					return treeView.Nodes[0] as AbstractProjectBrowserTreeNode;
				else
					return null;
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
			treeView.CanClearSelection = false;
			treeView.BeforeSelect   += TreeViewBeforeSelect;
			treeView.AfterExpand    += TreeViewAfterExpand;
			FileService.FileRenamed += FileServiceFileRenamed;
			FileService.FileRemoved += FileServiceFileRemoved;
			
			SD.ProjectService.ProjectItemAdded += ProjectServiceProjectItemAdded;
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
							var fileName = FileName.Create(file);
							if (SD.ProjectService.IsSolutionOrProjectFile(fileName))
								SD.ProjectService.OpenSolutionOrProject(fileName);
							else
								FileService.OpenFile(fileName);
						} catch (Exception ex) {
							MessageService.ShowException(ex, "unable to open file " + file);
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
			if (FileUtility.IsEqualFileName(Path.GetDirectoryName(e.SourceFile),
			                                Path.GetDirectoryName(e.TargetFile)))
			{
				CallVisitor(new FileRenameTreeNodeVisitor(e.SourceFile, e.TargetFile));
			} else {
				CallVisitor(new FileRemoveTreeNodeVisitor(e.SourceFile));
			}
		}
		
		public void RefreshView()
		{
			if (treeView.Nodes.Count > 0) {
				Properties memento = new Properties();
				StoreViewState(memento);
				ViewSolution(((AbstractProjectBrowserTreeNode)treeView.Nodes[0]).Solution);
				ReadViewState(memento);
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
			SD.MainThread.VerifyAccess();
			return FindFileNode(treeView.Nodes, fileName);
		}
		
		// stores the fileName of the last selected target so
		// that we can select it again on opening a folder
		string lastSelectionTarget;
		
		bool inSelectFile;
		
		/// <summary>
		/// Selects the deepest node open on the path to a particular file.
		/// </summary>
		public void SelectFile(string fileName)
		{
			try {
				inSelectFile = true;
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
			} finally {
				inSelectFile = false;
			}
		}
		
		public void SelectFileAndExpand(string fileName)
		{
			try {
				inSelectFile = true;
				lastSelectionTarget = fileName;
				LoadAndExpandToNode(new FileName(fileName));
			} finally {
				inSelectFile = false;
			}
		}

		#region SelectDeepestOpenNode internals

		void LoadAndExpandToNode(FileName fileName)
		{
			IProject project = null;
			if (!SD.ProjectService.IsSolutionOrProjectFile(fileName)) {
				project = SD.ProjectService.FindProjectContainingFile(fileName);
			}
			Stack<ISolutionItem> itemsToExpand = new Stack<ISolutionItem>();
			ISolutionItem item = project;
			if (project == null) {
				item = SD.ProjectService.CurrentSolution.AllItems
					.OfType<ISolutionFileItem>().FirstOrDefault(i => i.FileName.Equals(fileName));
			}
			while (item != null) {
				itemsToExpand.Push(item);
				item = item.ParentFolder;
			}
			AbstractProjectBrowserTreeNode current = null;
			var currentChildren = treeView.Nodes;
			while (itemsToExpand.Any()) {
				var currentItem = itemsToExpand.Pop();
				current = currentChildren.OfType<AbstractProjectBrowserTreeNode>().FirstOrDefault(n => n.Tag == currentItem);
				if (current == null) break;
				current.Expand();
				currentChildren = current.Nodes;
			}
			if (project != null) {
				var fileItem = project.FindFile(fileName);
				var virtualPath = fileItem.VirtualName;
				if (!string.IsNullOrWhiteSpace(fileItem.DependentUpon)) {
					int index = virtualPath.LastIndexOf('\\') + 1;
					virtualPath = virtualPath.Insert(index, fileItem.DependentUpon + "\\");
				}
				string[] relativePath = virtualPath.Split('\\');
				for (int i = 0; i < relativePath.Length; i++) {
					current = currentChildren.OfType<AbstractProjectBrowserTreeNode>()
						.FirstOrDefault(n => n.Text.Equals(relativePath[i], StringComparison.OrdinalIgnoreCase));
					if (current == null) break;
					if (i + 1 < relativePath.Length) current.Expand();
					currentChildren = current.Nodes;
				}
			}
			treeView.SelectedNode = current;
		}

		void SelectDeepestOpenNodeForPath(string fileName)
		{
			TreeNode node = FindDeepestOpenNodeForPath(fileName);
			if (node != null) {
				treeView.SelectedNode = node;
			}
		}
		
		TreeNode FindDeepestOpenNodeForPath(string fileName)
		{
			//LoggingService.DebugFormatted("Finding Deepest for '{0}'", fileName);
			ISolution solution = ProjectService.OpenSolution;
			if (solution == null) {
				return null;
			}

			IProject project = SD.ProjectService.FindProjectContainingFile(FileName.Create(fileName));
			if (project == null) {
				//LoggingService.Debug("no IProject found");
				return null;
			}

			string relativePath = String.Empty;
			AbstractProjectBrowserTreeNode targetNode = FindProjectNode(project);

			if (targetNode == null) {
				
				// our project node is not yet created,
				// so start at the root and work down.
				
				if (treeView.Nodes == null || treeView.Nodes.Count<1) {
					// the treeView is not yet prepared to assist in this request.
					return null;
					
				} else {
					targetNode = treeView.Nodes[0] as AbstractProjectBrowserTreeNode;
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
			
			return targetNode.GetNodeByRelativePath(relativePath);
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
		#endregion
		
		public void ViewSolution(ISolution solution)
		{
			AbstractProjectBrowserTreeNode solutionNode = new SolutionNode(solution);
			treeView.Clear();
			solutionNode.AddTo(treeView);
			
			foreach (var treeObject in solution.Items) {
				if (treeObject is IProject) {
					NodeBuilders.AddProjectNode(solutionNode, (IProject)treeObject);
				} else {
					SolutionFolderNode folderNode = new SolutionFolderNode((ISolutionFolder)treeObject);
					folderNode.InsertSorted(solutionNode);
				}
			}
			
			solutionNode.Expand();
		}
		
		public void Clear()
		{
			treeView.Clear();
			propertyContainer.SelectedObject = null;
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
			if (!inSelectFile) {
				ProjectService.CurrentProject = node.Project;
			}
			propertyContainer.SelectedObject = node.Tag;
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		/// <summary>
		/// Writes the current view state into the memento.
		/// </summary>
		public void StoreViewState(Properties memento)
		{
			memento.Set("ProjectBrowserState", TreeViewHelper.GetViewStateString(treeView));
		}
		
		/// <summary>
		/// Reads the view state from the memento.
		/// </summary>
		public void ReadViewState(Properties memento)
		{
			TreeViewHelper.ApplyViewStateString(memento.Get("ProjectBrowserState", ""), treeView);
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
		
		public void ExpandOrCollapseAll(bool expand)
		{
			if (this.treeView == null) return;
			if (this.treeView.Nodes == null || this.treeView.Nodes.Count == 0) return;
			
			if (expand) {
				this.treeView.ExpandAll();
			}
			else {
				this.treeView.CollapseAll();
			}
			
			this.treeView.Nodes[0].Expand();
		}
	}
}
