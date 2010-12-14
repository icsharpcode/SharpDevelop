// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	[Flags]
	public enum ClassBrowserFilter
	{
		None = 0,
		ShowProjectReferences = 1,
		ShowBaseAndDerivedTypes = 32,
		
		ShowPublic = 2,
		ShowProtected = 4,
		ShowPrivate = 8,
		ShowOther = 16,
		
		All = ShowProjectReferences | ShowPublic | ShowProtected | ShowPrivate | ShowOther | ShowBaseAndDerivedTypes
	}
	
	public class ClassBrowserPad : AbstractPadContent
	{
		static ClassBrowserPad instance;
		
		
		public static ClassBrowserPad Instance {
			get {
				return instance;
			}
		}
		ClassBrowserFilter filter               = ClassBrowserFilter.All;
		Panel              contentPanel         = new Panel();
		ExtTreeView        classBrowserTreeView = new ExtTreeView();
		
		public ClassBrowserFilter Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				foreach (TreeNode node in classBrowserTreeView.Nodes) {
					if (node is ExtTreeNode) {
						((ExtTreeNode)node).UpdateVisibility();
					}
				}
			}
		}
		
		public override object Control {
			get {
				return contentPanel;
			}
		}
		ToolStrip toolStrip;
		ToolStrip searchStrip;
		
		void UpdateToolbars()
		{
			ToolbarService.UpdateToolbar(toolStrip);
			ToolbarService.UpdateToolbar(searchStrip);
		}
		
		public ClassBrowserPad()
		{
			instance = this;
			classBrowserTreeView.Dock         = DockStyle.Fill;
			// we need to create a copy of the image list because adding image to
			// ClassBrowserIconService.ImageList is not allowed, but the ExtTreeView sometimes
			// does add images to its image list.
			classBrowserTreeView.ImageList    = new ImageList();
			classBrowserTreeView.ImageList.Images.AddRange(ClassBrowserIconService.ImageList.Images.Cast<System.Drawing.Image>().ToArray());
			classBrowserTreeView.AfterSelect += new TreeViewEventHandler(ClassBrowserTreeViewAfterSelect);
			
			contentPanel.Controls.Add(classBrowserTreeView);
			
			searchStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ClassBrowser/Searchbar");
			searchStrip.Stretch   = true;
			searchStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			contentPanel.Controls.Add(searchStrip);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ClassBrowser/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			contentPanel.Controls.Add(toolStrip);
			
			ProjectService.SolutionLoaded += ProjectServiceSolutionChanged;
			ProjectService.ProjectItemAdded += ProjectServiceSolutionChanged;
			ProjectService.ProjectItemRemoved += ProjectServiceSolutionChanged;
			ProjectService.ProjectAdded += ProjectServiceSolutionChanged; // rebuild view when project is added to solution
			ProjectService.SolutionFolderRemoved += ProjectServiceSolutionChanged; // rebuild view when project is removed from solution
			ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
			
			ParserService.ParseInformationUpdated += ParserServiceParseInformationUpdated;
			
			AmbienceService.AmbienceChanged += new EventHandler(AmbienceServiceAmbienceChanged);
			if (ProjectService.OpenSolution != null) {
				ProjectServiceSolutionChanged(null, null);
			}
			UpdateToolbars();
		}
		
		void ParserServiceParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			WorkbenchSingleton.DebugAssertMainThread();
			foreach (TreeNode node in classBrowserTreeView.Nodes) {
				AbstractProjectNode prjNode = node as AbstractProjectNode;
				if (prjNode != null && e.ProjectContent.Project == prjNode.Project) {
					prjNode.UpdateParseInformation(e.OldCompilationUnit, e.NewCompilationUnit);
				}
			}
		}
		
		#region Navigation
		Stack<TreeNode> previousNodes = new Stack<TreeNode>();
		Stack<TreeNode> nextNodes     = new Stack<TreeNode>();
		bool navigateBack    = false;
		bool navigateForward = false;
		
		public bool CanNavigateBackward {
			get {
				if (previousNodes.Count == 1 && this.classBrowserTreeView.SelectedNode == previousNodes.Peek()) {
					return false;
				}
				return previousNodes.Count > 0;
			}
		}
		
		public bool CanNavigateForward {
			get {
				if (nextNodes.Count == 1 && this.classBrowserTreeView.SelectedNode == nextNodes.Peek()) {
					return false;
				}
				return nextNodes.Count > 0;
			}
		}
		
		public void NavigateBackward()
		{
			if (previousNodes.Count > 0) {
				if (this.classBrowserTreeView.SelectedNode == previousNodes.Peek()) {
					nextNodes.Push(previousNodes.Pop());
				}
				if (previousNodes.Count > 0) {
					navigateBack = true;
					this.classBrowserTreeView.SelectedNode = previousNodes.Pop();
				}
			}
			UpdateToolbars();
		}
		
		public void NavigateForward()
		{
			if (nextNodes.Count > 0) {
				if (this.classBrowserTreeView.SelectedNode == nextNodes.Peek()) {
					previousNodes.Push(nextNodes.Pop());
				}
				if (nextNodes.Count > 0) {
					navigateForward = true;
					this.classBrowserTreeView.SelectedNode = nextNodes.Pop();
				}
			}
			UpdateToolbars();
		}
		
		public void CollapseAll() 
		{
			if (this.classBrowserTreeView == null) return;
			if (this.classBrowserTreeView.Nodes == null || this.classBrowserTreeView.Nodes.Count == 0) return;
			
			this.classBrowserTreeView.CollapseAll();
		}
		
		void ClassBrowserTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (navigateBack) {
				nextNodes.Push(e.Node);
				navigateBack = false;
			} else {
				if (!navigateForward) {
					nextNodes.Clear();
				}
				previousNodes.Push(e.Node);
				navigateForward = false;
			}
			UpdateToolbars();
		}
		#endregion
		
		bool inSearchMode = false;
		List<TreeNode> oldNodes = new List<TreeNode>();
		string searchTerm = "";
		
		public bool IsInSearchMode {
			get {
				return inSearchMode;
			}
		}
		public string SearchTerm {
			get {
				return searchTerm;
			}
			set {
				searchTerm = value.ToUpper().Trim();
			}
		}
		
		public void StartSearch()
		{
			if (searchTerm.Length == 0) {
				CancelSearch();
				return;
			}
			if (!inSearchMode) {
				foreach (TreeNode node in classBrowserTreeView.Nodes) {
					oldNodes.Add(node);
				}
				inSearchMode = true;
				previousNodes.Clear();
				nextNodes.Clear();
				UpdateToolbars();
			}
			classBrowserTreeView.BeginUpdate();
			classBrowserTreeView.Nodes.Clear();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						foreach (IClass c in projectContent.Classes) {
							if (c.Name.ToUpper().StartsWith(searchTerm)) {
								ClassNodeBuilders.AddClassNode(classBrowserTreeView, project, c);
							}
						}
					}
				}
			}
			if (classBrowserTreeView.Nodes.Count == 0) {
				ExtTreeNode notFoundMsg = new ExtTreeNode();
				notFoundMsg.Text = ResourceService.GetString("MainWindow.Windows.ClassBrowser.NoResultsFound");
				notFoundMsg.AddTo(classBrowserTreeView);
			}
			classBrowserTreeView.Sort();
			classBrowserTreeView.EndUpdate();
		}
		
		public void CancelSearch()
		{
			if (inSearchMode) {
				classBrowserTreeView.Nodes.Clear();
				foreach (TreeNode node in oldNodes) {
					classBrowserTreeView.Nodes.Add(node);
				}
				oldNodes.Clear();
				inSearchMode = false;
				previousNodes.Clear();
				nextNodes.Clear();
				UpdateToolbars();
			}
		}
		
		void ProjectServiceSolutionChanged(object sender, EventArgs e)
		{
			classBrowserTreeView.BeginUpdate();
			classBrowserTreeView.Nodes.Clear();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					if (project is MissingProject || project is UnknownProject) {
						continue;
					}
					ProjectNodeBuilders.AddProjectNode(classBrowserTreeView, project);
				}
				classBrowserTreeView.Sort();
			}
			classBrowserTreeView.EndUpdate();
		}
		
		void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			classBrowserTreeView.Nodes.Clear();
			previousNodes.Clear();
			nextNodes.Clear();
			UpdateToolbars();
		}
		
		void AmbienceServiceAmbienceChanged(object sender, EventArgs e)
		{
		}
		
	}
}
