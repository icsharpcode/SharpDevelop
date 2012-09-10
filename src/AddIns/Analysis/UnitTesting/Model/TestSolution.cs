// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Manages the collection of TestProjects.
	/// </summary>
	sealed class TestSolution : TestBase, ITestSolution
	{
		readonly IResourceService resourceService;
		readonly ITestService testService;
		readonly List<ProjectChangeListener> changeListeners = new List<ProjectChangeListener>();
		
		public TestSolution(ITestService testService, IResourceService resourceService)
		{
			if (testService == null)
				throw new ArgumentNullException("testService");
			if (resourceService == null)
				throw new ArgumentNullException("resourceService");
			this.testService = testService;
			this.resourceService = resourceService;
			ProjectService.SolutionLoaded += ProjectService_SolutionLoaded;
			ProjectService.SolutionClosed += ProjectService_SolutionClosed;
			ProjectService.ProjectAdded += ProjectService_ProjectAdded;
			ProjectService.ProjectRemoved += ProjectService_ProjectRemoved;
			SD.ParserService.LoadSolutionProjectsThread.Finished += SD_ParserService_LoadSolutionProjectsThread_Finished;
			if (ProjectService.OpenSolution != null) {
				ProjectService_SolutionLoaded(null, new SolutionEventArgs(ProjectService.OpenSolution));
				SD_ParserService_LoadSolutionProjectsThread_Finished(null, null);
			}
		}
		
		public override string DisplayName {
			get { return resourceService.GetString("ICSharpCode.UnitTesting.AllTestsTreeNode.Text"); }
		}
		
		public override event EventHandler DisplayNameChanged {
			add { resourceService.LanguageChanged += value; }
			remove { resourceService.LanguageChanged -= value; }
		}
		
		public override ITestProject ParentProject {
			get { return null; }
		}
		
		public ITestProject GetTestProject(IProject project)
		{
			for (int i = 0; i < changeListeners.Count; i++) {
				if (changeListeners[i].project == project) {
					return changeListeners[i].testProject;
				}
			}
			return null;
		}
		
		public IEnumerable<ITest> GetTestsForEntity(IEntity entity)
		{
			if (entity == null)
				return Enumerable.Empty<ITest>();
			ITestProject testProject = GetTestProject(entity.ParentAssembly.GetProject());
			if (testProject != null)
				return testProject.GetTestsForEntity(entity);
			else
				return Enumerable.Empty<ITest>();
		}
		
		/// <summary>
		/// Creates a TestProject for an IProject.
		/// This class takes care of changes in the test framework and will recreate the testProject
		/// if the test framework changes.
		/// </summary>
		class ProjectChangeListener
		{
			readonly TestSolution testSolution;
			ITestFramework oldTestFramework;
			internal readonly IProject project;
			internal ITestProject testProject;
			
			public ProjectChangeListener(TestSolution testSolution, IProject project)
			{
				this.testSolution = testSolution;
				this.project = project;
			}
			
			public void Start()
			{
				project.ParseInformationUpdated += project_ParseInformationUpdated;
				CheckTestFramework();
			}
			
			public void Stop()
			{
				project.ParseInformationUpdated -= project_ParseInformationUpdated;
				// Remove old testProject
				if (testProject != null) {
					testSolution.NestedTests.Remove(testProject);
					testProject = null;
				}
			}
			
			void project_ParseInformationUpdated(object sender, ParseInformationEventArgs e)
			{
				if (testProject != null) {
					testProject.NotifyParseInformationChanged(e.OldUnresolvedFile, e.NewUnresolvedFile);
				}
			}
			
			internal void CheckTestFramework()
			{
				ITestFramework newTestFramework = testSolution.testService.GetTestFrameworkForProject(project);
				if (newTestFramework == oldTestFramework)
					return; // test framework is unchanged
				
				// Remove old testProject
				if (testProject != null) {
					testSolution.NestedTests.Remove(testProject);
					testProject = null;
				}
				// Create new testProject
				if (newTestFramework != null) {
					testProject = newTestFramework.CreateTestProject(testSolution, project);
					if (testProject == null)
						throw new InvalidOperationException("CreateTestProject() returned null");
					testSolution.NestedTests.Add(testProject);
				}
				oldTestFramework = newTestFramework;
			}
		}
		
		void ProjectService_ProjectAdded(object sender, ProjectEventArgs e)
		{
			AddProject(e.Project);
		}
		
		void AddProject(IProject project)
		{
			ProjectChangeListener listener = new ProjectChangeListener(this, project);
			changeListeners.Add(listener);
			listener.Start();
		}
		
		void ProjectService_ProjectRemoved(object sender, ProjectEventArgs e)
		{
			for (int i = 0; i < changeListeners.Count; i++) {
				if (changeListeners[i].project == e.Project) {
					changeListeners[i].Stop();
					changeListeners.RemoveAt(i);
					break;
				}
			}
		}
		
		void ProjectService_SolutionClosed(object sender, EventArgs e)
		{
			for (int i = 0; i < changeListeners.Count; i++) {
				changeListeners[i].Stop();
			}
			changeListeners.Clear();
		}

		void ProjectService_SolutionLoaded(object sender, SolutionEventArgs e)
		{
			ProjectService_SolutionClosed(sender, e);
			foreach (var project in e.Solution.Projects) {
				AddProject(project);
			}
		}
		
		void SD_ParserService_LoadSolutionProjectsThread_Finished(object sender, EventArgs e)
		{
			for (int i = 0; i < changeListeners.Count; i++) {
				changeListeners[i].CheckTestFramework();
			}
		}
	}
}
