// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectBrowserPad.
	/// </summary>
	public class ProjectBrowserPad : AbstractPadContent, IClipboardHandler, IHasPropertyContainer
	{
		static ProjectBrowserPad instance;
		public static ProjectBrowserPad Instance {
			get {
				if (instance == null) {
					PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(ProjectBrowserPad));
					if (pad != null) {
						pad.CreatePad();
					} else {
						// Pad is not used (stripped-down SD version, e.g. SharpReport)
						// Create dummy pad to prevent NullReferenceExceptions
						instance = new ProjectBrowserPad();
					}
				}
				return instance;
			}
		}
		ProjectBrowserPanel projectBrowserPanel = new ProjectBrowserPanel();
		
		public AbstractProjectBrowserTreeNode SelectedNode {
			get {
				return projectBrowserPanel.SelectedNode;
			}
		}
		public ProjectNode CurrentProject {
			get {
				AbstractProjectBrowserTreeNode node = SelectedNode;
				while (node != null && !(node is ProjectNode))
					node = (AbstractProjectBrowserTreeNode)node.Parent;
				return (ProjectNode)node;
			}
		}
		/// <summary>
		/// Gets the root node of the project tree view.
		/// </summary>
		public AbstractProjectBrowserTreeNode SolutionNode {
			get {
				return projectBrowserPanel.RootNode;
			}
		}
		
		public ProjectBrowserControl ProjectBrowserControl {
			get {
				return projectBrowserPanel.ProjectBrowserControl;
			}
		}
		
		public override object Control {
			get {
				return projectBrowserPanel;
			}
		}
		
		public PropertyContainer PropertyContainer {
			get {
				return projectBrowserPanel.ProjectBrowserControl.PropertyContainer;
			}
		}
		
		public ProjectBrowserPad()
		{
			instance = this;
			ProjectService.SolutionLoaded += ProjectServiceSolutionLoaded;
			ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
			ProjectService.SolutionPreferencesSaving += ProjectServiceSolutionPreferencesSaving;
			
			WorkbenchSingleton.Workbench.ActiveContentChanged += ActiveContentChanged;
			if (ProjectService.OpenSolution != null) {
				this.LoadSolution(ProjectService.OpenSolution);
			}
			ActiveContentChanged(null, null);
		}
		
		public void StartLabelEdit(ExtTreeNode node)
		{
			ProjectBrowserControl.TreeView.StartLabelEdit(node);
		}
		
		void ProjectServiceSolutionPreferencesSaving(object sender, SolutionEventArgs e)
		{
			projectBrowserPanel.StoreViewState(e.Solution.Preferences.Properties);
		}
		
		void ProjectServiceSolutionLoaded(object sender, SolutionEventArgs e)
		{
			this.LoadSolution(e.Solution);
		}
		
		void LoadSolution(Solution solution)
		{
			if (!ProjectBrowserControl.TreeView.IsHandleCreated) {
				LoggingService.Debug("ProjectBrowser: Attempt to load solution " + solution.ToString() + " before handle of ProjectBrowserControl.TreeView created");
				this.solutionToLoadWhenHandleIsCreated = solution;
				if (!this.treeViewHandleCreatedAttached) {
					LoggingService.Debug("-> Attaching event handler to ProjectBrowserControl.TreeView.HandleCreated");
					this.treeViewHandleCreatedAttached = true;
					ProjectBrowserControl.TreeView.HandleCreated += this.ProjectBrowserTreeViewHandleCreated;
				}
			} else {
				LoggingService.Debug("ProjectBrowser: Loading solution " + solution.ToString() + " into project tree view");
				this.solutionToLoadWhenHandleIsCreated = null;
				projectBrowserPanel.ViewSolution(solution);
				projectBrowserPanel.ReadViewState(solution.Preferences.Properties);
			}
		}
		
		bool treeViewHandleCreatedAttached;
		Solution solutionToLoadWhenHandleIsCreated;
		
		void ProjectBrowserTreeViewHandleCreated(object sender, EventArgs e)
		{
			TreeView treeView = (TreeView)sender;
			this.treeViewHandleCreatedAttached = false;
			treeView.HandleCreated -= this.ProjectBrowserTreeViewHandleCreated;
			if (this.solutionToLoadWhenHandleIsCreated != null) {
				LoggingService.Debug("ProjectBrowser: Tree view handle created, will load " + this.solutionToLoadWhenHandleIsCreated.ToString() + ".");
				treeView.BeginInvoke(new Action<Solution>(this.LoadSolution), this.solutionToLoadWhenHandleIsCreated);
				this.solutionToLoadWhenHandleIsCreated = null;
			} else {
				LoggingService.Debug("ProjectBrowser: Tree view handle created, no solution to load.");
			}
		}
		
		void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			this.solutionToLoadWhenHandleIsCreated = null;
			projectBrowserPanel.Clear();
		}
		
		bool activeContentChangedEnqueued;
		
		void ActiveContentChanged(object sender, EventArgs e)
		{
			// this event may occur several times quickly after another (e.g. when opening or closing multiple files)
			// do the potentially expensive selection of the item in the tree view only once after the last change
			if (!activeContentChangedEnqueued) {
				activeContentChangedEnqueued = true;
				WorkbenchSingleton.SafeThreadAsyncCall(ActiveContentChangedInvoked);
			}
		}
		
		void ActiveContentChangedInvoked()
		{
			activeContentChangedEnqueued = false;
			if (WorkbenchSingleton.Workbench.ActiveContent == this) {
				projectBrowserPanel.ProjectBrowserControl.PadActivated();
			} else {
				// we don't use ActiveViewContent here as this is the ActiveContent change event handler
				IViewContent content = WorkbenchSingleton.Workbench.ActiveContent as IViewContent;
				if (content == null)
					return;
				string fileName = content.PrimaryFileName;
				if (fileName == null) {
					return;
				}
				
				if (!FileUtility.IsValidPath(fileName)) {
					return;
				}
				projectBrowserPanel.SelectFile(fileName);
			}
		}
		
		#region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
		public bool EnableCut {
			get {
				ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
				return node != null ? node.EnableCut : false;
			}
		}
		
		public bool EnableCopy {
			get {
				ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
				return node != null ? node.EnableCopy : false;
			}
		}
		
		public bool EnablePaste {
			get {
				ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
				return node != null ? node.EnablePaste : false;
			}
		}
		
		public bool EnableDelete {
			get {
				ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
				return node != null ? node.EnableDelete : false;
			}
		}
		
		public bool EnableSelectAll {
			get {
				ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
				return node != null ? node.EnableSelectAll : false;
			}
		}
		
		public void Cut()
		{
			ProjectBrowserControl.TreeView.ClearCutNodes();
			ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
			if (node != null) {
				node.Cut();
			}
		}
		
		public void Copy()
		{
			ProjectBrowserControl.TreeView.ClearCutNodes();
			ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
			if (node != null) {
				node.Copy();
			}
		}
		
		public void Paste()
		{
			ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
			if (node != null) {
				node.Paste();
			}
			ProjectBrowserControl.TreeView.ClearCutNodes();
		}
		
		public void Delete()
		{
			ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
			if (node != null) {
				node.Delete();
			}
			ProjectBrowserControl.TreeView.ClearCutNodes();
		}
		
		public void SelectAll()
		{
			ExtTreeNode node = ProjectBrowserControl.TreeView.SelectedNode as ExtTreeNode;
			if (node != null) {
				node.SelectAll();
			}
		}
		#endregion
		
		static bool refreshViewEnqueued;
		
		public static void RefreshViewAsync()
		{
			WorkbenchSingleton.AssertMainThread();
			if (refreshViewEnqueued || WorkbenchSingleton.Workbench == null)
				return;
			refreshViewEnqueued = true;
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					refreshViewEnqueued = false;
					if (instance != null) {
						instance.ProjectBrowserControl.RefreshView();
					}
				});
		}
	}
}
