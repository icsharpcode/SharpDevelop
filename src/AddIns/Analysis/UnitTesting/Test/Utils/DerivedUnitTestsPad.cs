// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	/// <summary>
	/// Class that derives from UnitTestsPad so we can access protected 
	/// methods and test it.
	/// </summary>
	public class DerivedUnitTestsPad : UnitTestsPad
	{
		bool getOpenSolutionCalled;
		bool isParserLoadingSolutionCalled;
		MockProjectContent projectContent = new MockProjectContent();
		Solution openSolution;
		bool loadSolutionProjectsThreadEndedHandled;
		bool addedLoadSolutionProjectsThreadEndedHandler;
		DummyParserServiceTestTreeView treeView = new DummyParserServiceTestTreeView();

		public DerivedUnitTestsPad(Solution openSolution)
			: base(new MockTestFrameworksWithNUnitFrameworkSupport())
		{
			this.openSolution = openSolution;
		}
		
		public DerivedUnitTestsPad()
			: this(null)
		{
		}
		
		/// <summary>
		/// Gets the project content to be used when creating the
		/// derived test tree view.
		/// </summary>
		public MockProjectContent ProjectContent {
			get { return projectContent; }
			set {
				projectContent = value;
				treeView.ProjectContentForProject = projectContent;
			}
		}
		
		public bool GetOpenSolutionCalled {
			get { return getOpenSolutionCalled; }
		}
		
		public bool IsParserLoadingSolutionCalled {
			get { return isParserLoadingSolutionCalled; }
		}
		
		/// <summary>
		/// Checks whether the ParserService's LoadSolutionProjectsThreadEnded event
		/// is mapped to an event handler before IsParserLoadingSolution is
		/// called. This ensures we do not miss this event.
		/// </summary>
		public bool LoadSolutionProjectsThreadEndedHandled {
			get { return loadSolutionProjectsThreadEndedHandled; }
		}
		
		public void CallSolutionLoaded(Solution solution)
		{
			base.SolutionLoaded(solution);
		}
		
		public void CallSolutionClosed()
		{
			base.SolutionClosed();
		}
		
		public void CallProjectItemRemoved(ProjectItem item)
		{
			base.ProjectItemRemoved(item);
		}
		
		public void CallProjectItemAdded(ProjectItem item)
		{
			base.ProjectItemAdded(item);
		}
		
		public void CallProjectAdded(IProject project)
		{
			base.ProjectAdded(project);
		}
		
		public void CallSolutionFolderRemoved(ISolutionFolder folder)
		{
			base.SolutionFolderRemoved(folder);
		}
		
		public void CallUpdateParseInfo(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			base.UpdateParseInfo(oldUnit, newUnit);
		}
		
		/// <summary>
		/// Returns a dummy toolstrip so the UnitTestsPad can be
		/// tested. If the default method is called the AddInTree
		/// is referenced which is not available during testing.
		/// </summary>
		protected override ToolStrip CreateToolStrip(string name)
		{
			return new ToolStrip();
		}
		
		/// <summary>
		/// Returns a dummy ContextMenuStrip so the UnitTestsPad can be
		/// tested. If the default method is called the AddInTree
		/// is referenced which is not available during testing.
		/// </summary>
		protected override ContextMenuStrip CreateContextMenu(string name)
		{
			return new ContextMenuStrip();
		}
		
		/// <summary>
		/// Returns a dummy tree view where we can mock the
		/// IProjectContent that will be used by the TestTreeView.
		/// </summary>
		protected override TestTreeView CreateTestTreeView(IRegisteredTestFrameworks testFrameworks)
		{
			return treeView;
		}
		
		protected override Solution GetOpenSolution()
		{
			getOpenSolutionCalled = true;
			return openSolution;
		}
		
		protected override bool IsParserLoadingSolution {
			get {
				loadSolutionProjectsThreadEndedHandled = addedLoadSolutionProjectsThreadEndedHandler;
				isParserLoadingSolutionCalled = true;
				return false;
			}
		}
		
		protected override void OnAddedLoadSolutionProjectsThreadEndedHandler()
		{
			addedLoadSolutionProjectsThreadEndedHandler = true;
		}
	}
}
