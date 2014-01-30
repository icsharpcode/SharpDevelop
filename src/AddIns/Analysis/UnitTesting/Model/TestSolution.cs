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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
			SD.ProjectService.AllProjects.CollectionChanged += OnProjectsCollectionChanged;
			SD.ParserService.LoadSolutionProjectsThread.Finished += SD_ParserService_LoadSolutionProjectsThread_Finished;
			foreach (var project in SD.ProjectService.AllProjects) {
				AddProject(project);
			}
			SD_ParserService_LoadSolutionProjectsThread_Finished(null, null);
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
					testSolution.NestedTestCollection.Remove(testProject);
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
					testSolution.NestedTestCollection.Remove(testProject);
					testProject = null;
				}
				// Create new testProject
				if (newTestFramework != null) {
					testProject = newTestFramework.CreateTestProject(testSolution, project);
					if (testProject == null)
						throw new InvalidOperationException("CreateTestProject() returned null");
					testSolution.NestedTestCollection.Add(testProject);
				}
				oldTestFramework = newTestFramework;
			}
		}
		
		void OnProjectsCollectionChanged(IReadOnlyCollection<IProject> removedItems, IReadOnlyCollection<IProject> addedItems)
		{
			for (int i = 0; i < changeListeners.Count; i++) {
				if (removedItems.Contains(changeListeners[i].project)) {
					changeListeners[i].Stop();
					changeListeners.RemoveAt(i--);
				}
			}
			foreach (var project in addedItems) {
				AddProject(project);
			}
		}
		
		void AddProject(IProject project)
		{
			ProjectChangeListener listener = new ProjectChangeListener(this, project);
			changeListeners.Add(listener);
			listener.Start();
		}
		
		void SD_ParserService_LoadSolutionProjectsThread_Finished(object sender, EventArgs e)
		{
			for (int i = 0; i < changeListeners.Count; i++) {
				changeListeners[i].CheckTestFramework();
			}
		}
	}
}
