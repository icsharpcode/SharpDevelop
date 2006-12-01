// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestsPad : AbstractPadContent
	{
		TestTreeView treeView;
		bool disposed;
		Panel panel;
		ToolStrip toolStrip;
		List<ICompilationUnit[]> pending = new List<ICompilationUnit[]>();
		static UnitTestsPad instance;
		
		public UnitTestsPad()
		{
			instance = this;
			
			panel = new Panel();
			treeView = CreateTestTreeView();
			treeView.Dock = DockStyle.Fill;
			treeView.DoubleClick += TestTreeViewDoubleClick;
			treeView.KeyPress += TestTreeViewKeyPress;
			panel.Controls.Add(treeView);
			
			toolStrip = CreateToolStrip("/SharpDevelop/Pads/UnitTestsPad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			panel.Controls.Add(toolStrip);

			// Display currently open solution.
			Solution openSolution = ProjectService.OpenSolution;
			if (openSolution != null) {
				SolutionLoaded(openSolution);
			}
			
			ParserService.LoadSolutionProjectsThreadEnded += LoadSolutionProjectsThreadEnded;
			ParserService.ParseInformationUpdated += ParseInformationUpdated;
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.SolutionFolderRemoved += SolutionFolderRemoved;
			ProjectService.ProjectAdded += ProjectAdded;
			ProjectService.ProjectItemAdded += ProjectItemAdded;
			ProjectService.ProjectItemRemoved += ProjectItemRemoved;
			
			treeView.ContextMenuStrip = CreateContextMenu("/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
		}
		
		public static UnitTestsPad Instance {
			get {
				return instance;
			}
		}
		
		public override Control Control {
			get {
				return panel;
			}
		}
		
		public override void Dispose()
		{
			if (!disposed) {
				disposed = true;
				treeView.Dispose();
				treeView = null;
				
				ProjectService.ProjectItemRemoved -= ProjectItemRemoved;
				ProjectService.ProjectItemAdded -= ProjectItemAdded;
				ProjectService.ProjectAdded -= ProjectAdded;
				ProjectService.SolutionFolderRemoved -= SolutionFolderRemoved;
				ProjectService.SolutionClosed -= SolutionClosed;
				ParserService.ParseInformationUpdated -= ParseInformationUpdated;
				ParserService.LoadSolutionProjectsThreadEnded -= LoadSolutionProjectsThreadEnded;
			}
		}
		
		public TestTreeView TestTreeView {
			get {
				return treeView;
			}
		}
		
		/// <summary>
		/// Updates the state of the buttons on the Unit Tests pad's
		/// toolbar.
		/// </summary>
		public void UpdateToolbar()
		{
			ToolbarService.UpdateToolbar(toolStrip);
		}
		
		/// <summary>
		/// Called when a solution has been loaded.
		/// </summary>
		protected void SolutionLoaded(Solution solution)
		{
			treeView.AddSolution(solution);
		}
		
		/// <summary>
		/// Called when a solution has been closed.
		/// </summary>
		protected void SolutionClosed()
		{
			treeView.Clear();
		}
		
		protected void SolutionFolderRemoved(ISolutionFolder solutionFolder)
		{
			IProject project = solutionFolder as IProject;
			if (project != null) {
				treeView.RemoveProject(project);
			}
		}
		
		/// <summary>
		/// The project is added to the tree view only if it has a
		/// reference to a unit testing framework.
		/// </summary>
		protected void ProjectAdded(IProject project)
		{
			treeView.AddProject(project);
		}
		
		/// <summary>
		/// If the project item removed is a reference to a unit
		/// test framework then the project will be removed from the
		/// test tree.
		/// </summary>
		protected void ProjectItemRemoved(ProjectItem projectItem)
		{
			if (IsTestFrameworkReferenceProjectItem(projectItem)) {
				if (!TestProject.IsTestProject(projectItem.Project)) {
					treeView.RemoveProject(projectItem.Project);
				}
			}
		}
		
		/// <summary>
		/// Adds the test project to the test tree view if it has
		/// a reference to a unit testing framework and is not
		/// already in the test tree.
		/// </summary>
		protected void ProjectItemAdded(ProjectItem projectItem)
		{
			if (IsTestFrameworkReferenceProjectItem(projectItem)) {
				treeView.AddProject(projectItem.Project);
			}
		}
		
		protected void UpdateParseInfo(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			treeView.UpdateParseInfo(oldUnit, newUnit);
		}
		
		protected virtual ToolStrip CreateToolStrip(string name)
		{
			return ToolbarService.CreateToolStrip(treeView, "/SharpDevelop/Pads/UnitTestsPad/Toolbar");
		}
		
		protected virtual ContextMenuStrip CreateContextMenu(string name)
		{
			return MenuService.CreateContextMenu(treeView, "/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
		}
		
		protected virtual TestTreeView CreateTestTreeView()
		{
			return new TestTreeView();
		}
		
		void SolutionClosed(object source, EventArgs e)
		{
			SolutionClosed();
			UpdateToolbar();
		}
		
		void SolutionFolderRemoved(object source, SolutionFolderEventArgs e)
		{
			SolutionFolderRemoved(e.SolutionFolder);
			UpdateToolbar();
		}
		
		void ProjectAdded(object source, ProjectEventArgs e)
		{
			ProjectAdded(e.Project);
			UpdateToolbar();
		}
		
		void LoadSolutionProjectsThreadEnded(object source, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateToolbar);
			Solution solution = ProjectService.OpenSolution;
			WorkbenchSingleton.SafeThreadAsyncCall(SolutionLoaded, solution);
		}
		
		void ParseInformationUpdated(object source, ParseInformationEventArgs e)
		{
			lock (pending) {
				ICompilationUnit[] units = new ICompilationUnit[] {e.ParseInformation.MostRecentCompilationUnit as ICompilationUnit, e.CompilationUnit};
				pending.Add(units);
			}
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateParseInfo);
		}
		
		void UpdateParseInfo()
		{
			lock (pending) {
				foreach (ICompilationUnit[] units in pending) {
					UpdateParseInfo(units[0], units[1]);
				}
				pending.Clear();
			}
		}
		
		void TestTreeViewDoubleClick(object source, EventArgs e)
		{
			GotoDefinition();
		}
		
		void TestTreeViewKeyPress(object source, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r') {
				e.Handled = true;
				GotoDefinition();
			} else if (e.KeyChar == ' ') {
				e.Handled = true;
				RunTests();
			}
		}
		
		void GotoDefinition()
		{
			RunCommand(new GotoDefinitionCommand());
		}
		
		void RunTests()
		{
			RunCommand(new RunTestInPadCommand());
		}
		
		void RunCommand(ICommand command)
		{
			command.Owner = treeView;
			command.Run();
		}
		
		bool IsTestFrameworkReferenceProjectItem(ProjectItem projectItem)
		{
			ReferenceProjectItem referenceProjectItem = projectItem as ReferenceProjectItem;
			if (referenceProjectItem != null) {
				return TestProject.IsTestFrameworkReference(referenceProjectItem.Include);
			}
			return false;
		}
		
		void ProjectItemAdded(object source, ProjectItemEventArgs e)
		{
			ProjectItemAdded(e.ProjectItem);
		}
		
		void ProjectItemRemoved(object source, ProjectItemEventArgs e)
		{
			ProjectItemRemoved(e.ProjectItem);
		}
	}
}
