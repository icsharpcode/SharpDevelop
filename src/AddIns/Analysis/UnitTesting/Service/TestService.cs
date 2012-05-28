// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public static class TestService
	{
		static IRegisteredTestFrameworks testFrameworks;
		static MessageViewCategory unitTestMessageView;
		
		public static IRegisteredTestFrameworks RegisteredTestFrameworks {
			get {
				CreateRegisteredTestFrameworks();
				return testFrameworks;
			}
			set { testFrameworks = value; }
		}
		
		static void CreateRegisteredTestFrameworks()
		{
			if (testFrameworks == null) {
				UnitTestAddInTree addInTree = new UnitTestAddInTree();
				testFrameworks = new RegisteredTestFrameworks(addInTree);
			}
		}
		
		public static MessageViewCategory UnitTestMessageView {
			get {
				if (unitTestMessageView == null) {
					CreateUnitTestCategory();
				}
				return unitTestMessageView;
			}
		}
		
		static void CreateUnitTestCategory()
		{
			MessageViewCategory.Create(ref unitTestMessageView,
			                           "UnitTesting",
			                           "${res:ICSharpCode.NUnitPad.NUnitPadContent.PadName}");
		}
		
		static readonly ObservableCollection<TestProject> testableProjects = new ObservableCollection<TestProject>();
		
		public static ObservableCollection<TestProject> TestableProjects {
			get { return testableProjects; }
		}
		
		static TestService()
		{
			ProjectService.SolutionCreated += ProjectService_SolutionCreated;
			ProjectService.SolutionLoaded += ProjectService_SolutionLoaded;
			ProjectService.SolutionClosed += ProjectService_SolutionClosed;
			ProjectService.ProjectCreated += ProjectService_ProjectCreated;
			ProjectService.ProjectAdded += ProjectService_ProjectAdded;
			ProjectService.ProjectRemoved += ProjectService_ProjectRemoved;
			SD.ParserService.LoadSolutionProjectsThread.Finished += SD_ParserService_LoadSolutionProjectsThread_Finished;
		}

		static void SD_ParserService_LoadSolutionProjectsThread_Finished(object sender, EventArgs e)
		{
			testableProjects.Clear();
			testableProjects.AddRange(GetTestableProjects());
		}
		
		static void ProjectService_ProjectCreated(object sender, ProjectEventArgs e)
		{
			if (RegisteredTestFrameworks.IsTestProject(e.Project))
				testableProjects.Add(new TestProject(e.Project));
		}

		static void ProjectService_ProjectAdded(object sender, ProjectEventArgs e)
		{
			if (RegisteredTestFrameworks.IsTestProject(e.Project))
				testableProjects.Add(new TestProject(e.Project));
		}

		static void ProjectService_ProjectRemoved(object sender, ProjectEventArgs e)
		{
			testableProjects.RemoveWhere(test => test.Project == e.Project);
		}

		static void ProjectService_SolutionCreated(object sender, SolutionEventArgs e)
		{
			testableProjects.Clear();
			testableProjects.AddRange(GetTestableProjects());
		}

		static void ProjectService_SolutionClosed(object sender, EventArgs e)
		{
			testableProjects.Clear();
		}

		static void ProjectService_SolutionLoaded(object sender, SolutionEventArgs e)
		{
			testableProjects.Clear();
			testableProjects.AddRange(GetTestableProjects());
		}
		
		static IEnumerable<TestProject> GetTestableProjects()
		{
			if (ProjectService.OpenSolution == null)
				return Enumerable.Empty<TestProject>();
			return ProjectService.OpenSolution.Projects.Where(p => RegisteredTestFrameworks.IsTestProject(p)).Select(p => new TestProject(p));
		}
		
		public static TestProject GetTestProject(IProject currentProject)
		{
			return testableProjects.FirstOrDefault(p => p.Project == currentProject);
		}
	}
}
