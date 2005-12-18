// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

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
				return instance;
			}
		}
		ProjectBrowserPanel projectBrowserPanel = new ProjectBrowserPanel();
		
		public AbstractProjectBrowserTreeNode SelectedNode {
			get {
				return projectBrowserPanel.SelectedNode;
			}
		}
		
		public ProjectBrowserControl ProjectBrowserControl {
			get {
				return projectBrowserPanel.ProjectBrowserControl;
			}
		}
		
		public override Control Control {
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
			
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += ActiveWindowChanged;
			if (ProjectService.OpenSolution != null) {
				projectBrowserPanel.ViewSolution(ProjectService.OpenSolution);
			}
		}
		
		public void StartLabelEdit(ExtTreeNode node)
		{
			ProjectBrowserControl.TreeView.StartLabelEdit(node);
		}
		
		void ProjectServiceSolutionLoaded(object sender, SolutionEventArgs e)
		{
			projectBrowserPanel.ViewSolution(e.Solution);
		}
		
		void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			projectBrowserPanel.Clear();
		}
		
		string lastFileName;
		
		void ActiveWindowChanged(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.ActiveContent == this) {
				projectBrowserPanel.ProjectBrowserControl.PadActivated();
			} else {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				if (window == null) {
					return;
				}
				string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
				if (fileName == null || lastFileName == fileName) {
					return;
				}
				
				if (!FileUtility.IsValidFileName(fileName)) {
					return;
				}
				lastFileName = fileName;
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
		
	}
}
