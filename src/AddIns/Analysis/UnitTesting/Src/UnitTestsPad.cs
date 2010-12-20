// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestsPad : AbstractPadContent, IUnitTestsPad
	{
		TestTreeView treeView;
		bool disposed;
		Panel panel;
		ToolStrip toolStrip;
		List<ICompilationUnit[]> pending = new List<ICompilationUnit[]>();
		static UnitTestsPad instance;
		
		public UnitTestsPad()
			: this(TestService.RegisteredTestFrameworks)
		{
		}
		
		public UnitTestsPad(IRegisteredTestFrameworks testFrameworks)
		{
			instance = this;
			
			panel = new Panel();
			treeView = CreateTestTreeView(testFrameworks);
			treeView.Dock = DockStyle.Fill;
			treeView.DoubleClick += TestTreeViewDoubleClick;
			treeView.KeyPress += TestTreeViewKeyPress;
			panel.Controls.Add(treeView);
			
			toolStrip = CreateToolStrip("/SharpDevelop/Pads/UnitTestsPad/Toolbar");
			toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			panel.Controls.Add(toolStrip);

			// Add the load solution projects thread ended handler before
			// we try to display the open solution so the event does not
			// get missed.
			ParserService.LoadSolutionProjectsThreadEnded += LoadSolutionProjectsThreadEnded;
			OnAddedLoadSolutionProjectsThreadEndedHandler();

			// Display currently open solution.
			if (!IsParserLoadingSolution) {
				Solution openSolution = GetOpenSolution();
				if (openSolution != null) {
					SolutionLoaded(openSolution);
				}
			}
			
			ParserService.ParseInformationUpdated += ParseInformationUpdated;
			ProjectService.SolutionClosed += SolutionClosed;
			ProjectService.SolutionFolderRemoved += SolutionFolderRemoved;
			ProjectService.ProjectAdded += ProjectAdded;
			ProjectService.ProjectItemAdded += ProjectItemAdded;
			ProjectService.ProjectItemRemoved += ProjectItemRemoved;
			
			treeView.ContextMenuStrip = CreateContextMenu("/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
		}
		
		public static UnitTestsPad Instance {
			get { return instance; }
		}
		
		public override object Control {
			get { return panel; }
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
			get { return treeView; }
		}
		
		public void ResetTestResults()
		{
			treeView.ResetTestResults();
		}
		
		public IProject[] GetProjects()
		{
			return treeView.GetProjects();
		}
		
		public TestProject GetTestProject(IProject project)
		{
			return treeView.GetTestProject(project);
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
		/// Collapses all nodes.
		/// </summary>
		public void CollapseAll()
		{
			if (treeView == null || treeView.Nodes == null || treeView.Nodes.Count == 0)
				return;
			
			treeView.CollapseAll();
		}
		
		/// <summary>
		/// Called when a solution has been loaded.
		/// </summary>
		protected void SolutionLoaded(Solution solution)
		{
			// SolutionLoaded will be invoked from another thread.
			// The UnitTestsPad might be disposed by the time the event is processed by the main thread.
			if (treeView != null) {
				if (solution != null) {
					treeView.AddSolution(solution);
				} else {
					treeView.Clear();
				}
			}
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
			treeView.RemoveSolutionFolder(solutionFolder);
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
			treeView.ProjectItemRemoved(projectItem);
		}
		
		protected void ProjectItemAdded(ProjectItem projectItem)
		{
			treeView.ProjectItemAdded(projectItem);
		}
		
		/// <summary>
		/// Protected method so we can test this method.
		/// </summary>
		protected void UpdateParseInfo(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			treeView.UpdateParseInfo(oldUnit, newUnit);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy toolstrip when testing.
		/// </summary>
		protected virtual ToolStrip CreateToolStrip(string name)
		{
			return ToolbarService.CreateToolStrip(treeView, name);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ContextMenuStrip when testing.
		/// </summary>
		protected virtual ContextMenuStrip CreateContextMenu(string name)
		{
			return MenuService.CreateContextMenu(treeView, name);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy TestTreeView when testing.
		/// </summary>
		protected virtual TestTreeView CreateTestTreeView(IRegisteredTestFrameworks testFrameworks)
		{
			return new TestTreeView(testFrameworks);
		}
		
		/// <summary>
		/// Gets the currently open solution.
		/// </summary>
		protected virtual Solution GetOpenSolution()
		{
			return ProjectService.OpenSolution;
		}
		
		/// <summary>
		/// Determines whether the parser is currently still loading the
		/// solution.
		/// </summary>
		protected virtual bool IsParserLoadingSolution {
			get { return ParserService.LoadSolutionProjectsThreadRunning; }
		}
		
		/// <summary>
		/// Indicates that an event handler for the ParserService's
		/// LoadSolutionProjectsThreadEnded event has been added
		/// </summary>
		protected virtual void OnAddedLoadSolutionProjectsThreadEndedHandler()
		{
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
				ICompilationUnit[] units = new ICompilationUnit[] {e.OldCompilationUnit, e.NewCompilationUnit};
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
