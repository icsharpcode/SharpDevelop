// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestsPad : AbstractPadContent
	{
		SharpTreeView treeView;
		bool disposed;
		DockPanel panel;
		ToolBar toolBar;
		List<Tuple<IParsedFile, IParsedFile>> pending = new List<Tuple<IParsedFile, IParsedFile>>();
		static UnitTestsPad instance;

		public UnitTestsPad()
			: this(TestService.RegisteredTestFrameworks)
		{
		}
		
		public UnitTestsPad(IRegisteredTestFrameworks testFrameworks)
		{
			instance = this;
			
			panel = new DockPanel();

			toolBar = CreateToolBar("/SharpDevelop/Pads/UnitTestsPad/Toolbar");
			panel.Children.Add(toolBar);
			DockPanel.SetDock(toolBar, Dock.Top);
			
			treeView = new SharpTreeView();
			treeView.MouseDoubleClick += TestTreeViewDoubleClick;
//			treeView.KeyDown += TestTreeViewKeyPress;
			panel.Children.Add(treeView);
			
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
			
			treeView.ContextMenu = CreateContextMenu("/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
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

				ProjectService.ProjectItemRemoved -= ProjectItemRemoved;
				ProjectService.ProjectItemAdded -= ProjectItemAdded;
				ProjectService.ProjectAdded -= ProjectAdded;
				ProjectService.SolutionFolderRemoved -= SolutionFolderRemoved;
				ProjectService.SolutionClosed -= SolutionClosed;
				ParserService.ParseInformationUpdated -= ParseInformationUpdated;
				ParserService.LoadSolutionProjectsThreadEnded -= LoadSolutionProjectsThreadEnded;
			}
		}
		
//		public TestTreeView TestTreeView {
//			get { return treeView; }
//		}
		
//		public void ResetTestResults()
//		{
//			treeView.ResetTestResults();
//		}
//
//		public IProject[] GetProjects()
//		{
//			return treeView.GetProjects();
//		}
		
//		public TestProject GetTestProject(IProject project)
//		{
//			return treeView.GetTestProject(project);
//		}
		
		/// <summary>
		/// Updates the state of the buttons on the Unit Tests pad's
		/// toolbar.
		/// </summary>
		public void UpdateToolbar()
		{
//			ToolbarService.UpdateToolbar(toolBar);
		}
		
		/// <summary>
		/// Collapses all nodes.
		/// </summary>
//		public void CollapseAll()
//		{
//			if (treeView == null || treeView.Nodes == null || treeView.Nodes.Count == 0)
//				return;
//
//			treeView.CollapseAll();
//		}
		
		/// <summary>
		/// Called when a solution has been loaded.
		/// </summary>
		protected void SolutionLoaded(Solution solution)
		{
			// SolutionLoaded will be invoked from another thread.
			// The UnitTestsPad might be disposed by the time the event is processed by the main thread.
			if (treeView != null) {
				if (solution != null) {
					treeView.Root = new RootUnitTestNode(solution);
				} else {
					treeView.Root = null;
				}
			}
		}
		
		/// <summary>
		/// Called when a solution has been closed.
		/// </summary>
//		protected void SolutionClosed()
//		{
//			treeView.Clear();
//		}
//
//		protected void SolutionFolderRemoved(ISolutionFolder solutionFolder)
//		{
//			treeView.RemoveSolutionFolder(solutionFolder);
//		}
//
		/// <summary>
		/// The project is added to the tree view only if it has a
		/// reference to a unit testing framework.
		/// </summary>
		protected void ProjectAdded(IProject project)
		{
			SolutionLoaded(GetOpenSolution());
		}
		
		/// <summary>
		/// If the project item removed is a reference to a unit
		/// test framework then the project will be removed from the
		/// test tree.
		/// </summary>
//		protected void ProjectItemRemoved(ProjectItem projectItem)
//		{
//			treeView.ProjectItemRemoved(projectItem);
//		}
//
//		protected void ProjectItemAdded(ProjectItem projectItem)
//		{
//			treeView.ProjectItemAdded(projectItem);
//		}
		
		/// <summary>
		/// Protected method so we can test this method.
		/// </summary>
		protected void UpdateParseInfo(IParsedFile oldFile, IParsedFile newFile)
		{
			RootUnitTestNode root = (RootUnitTestNode)treeView.Root;
			if (root == null) {
				SolutionLoaded(GetOpenSolution());
				root = (RootUnitTestNode)treeView.Root;
				if (root == null) return;
			}
			var solution = GetOpenSolution();
			if (solution == null)
				return;
			var project = solution.FindProjectContainingFile((oldFile ?? newFile).FileName);
			if (project == null)
				return;
			var projectNode = root.Children.OfType<ProjectUnitTestNode>().FirstOrDefault(node => node.Project == project);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ToolBar when testing.
		/// </summary>
		protected virtual ToolBar CreateToolBar(string name)
		{
			return ToolBarService.CreateToolBar(treeView, treeView, name);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ContextMenu when testing.
		/// </summary>
		protected virtual ContextMenu CreateContextMenu(string name)
		{
			return MenuService.CreateContextMenu(treeView, name);
		}
		
//		/// <summary>
//		/// Virtual method so we can override this method and return
//		/// a dummy TestTreeView when testing.
//		/// </summary>
//		protected virtual SharpTreeView CreateTestTreeView(IRegisteredTestFrameworks testFrameworks)
//		{
//			return new SharpTreeView();
//		}
		
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
//			SolutionClosed();
			UpdateToolbar();
		}
		
		void SolutionFolderRemoved(object source, SolutionFolderEventArgs e)
		{
//			SolutionFolderRemoved(e.SolutionFolder);
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
//			WorkbenchSingleton.SafeThreadAsyncCall(SolutionLoaded, solution);
		}
		
		void ParseInformationUpdated(object source, ParseInformationEventArgs e)
		{
			lock (pending) {
				var files = Tuple.Create(e.OldParsedFile, e.NewParsedFile);
				pending.Add(files);
			}
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateParseInfo);
		}
		
		void UpdateParseInfo()
		{
			lock (pending) {
				foreach (var files in pending) {
					UpdateParseInfo(files.Item1, files.Item2);
				}
				pending.Clear();
			}
		}
		
		void TestTreeViewDoubleClick(object source, EventArgs e)
		{
			GotoDefinition();
		}
		
//		void TestTreeViewKeyPress(object source, KeyPressEventArgs e)
//		{
//			if (e.KeyChar == '\r') {
//				e.Handled = true;
//				GotoDefinition();
//			} else if (e.KeyChar == ' ') {
//				e.Handled = true;
//				RunTests();
//			}
//		}
		
		void GotoDefinition()
		{
//			RunCommand(new GotoDefinitionCommand());
		}
		
		void RunTests()
		{
//			RunCommand(new RunTestInPadCommand());
		}
		
		void RunCommand(ICommand command)
		{
			command.Owner = treeView;
			command.Run();
		}
		
		void ProjectItemAdded(object source, ProjectItemEventArgs e)
		{
//			ProjectItemAdded(e.ProjectItem);
		}
		
		void ProjectItemRemoved(object source, ProjectItemEventArgs e)
		{
//			ProjectItemRemoved(e.ProjectItem);
		}
	}
	

}
