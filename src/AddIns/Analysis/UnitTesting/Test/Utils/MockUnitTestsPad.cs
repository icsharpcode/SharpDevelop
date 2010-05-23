// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockUnitTestsPad : IUnitTestsPad
	{
		bool updateToolbarMethodCalled;
		bool bringToFrontMethodCalled;
		bool resetTestResultsMethodCalled;
		List<IProject> projects = new List<IProject>();
		List<TestProject> testProjects = new List<TestProject>();
		
		public bool IsUpdateToolbarMethodCalled {
			get { return updateToolbarMethodCalled; }
			set { updateToolbarMethodCalled = value; }
		}
		
		public void UpdateToolbar()
		{
			updateToolbarMethodCalled = true;
		}
		
		public bool IsBringToFrontMethodCalled {
			get { return bringToFrontMethodCalled; }
		}
		
		public void BringToFront()
		{
			bringToFrontMethodCalled = true;
		}
		
		public bool IsResetTestResultsMethodCalled {
			get { return resetTestResultsMethodCalled; }
		}
		
		public void ResetTestResults()
		{
			resetTestResultsMethodCalled = true;
		}
		
		public void AddProject(IProject project)
		{
			projects.Add(project);
		}
		
		public IProject[] GetProjects()
		{
			return projects.ToArray();
		}
		
		public TestProject GetTestProject(IProject project)
		{
			foreach (TestProject testProject in testProjects) {
				if (testProject.Project == project) {
					return testProject;
				}
			}
			return null;
		}
		
		public void AddTestProject(TestProject testProject)
		{
			testProjects.Add(testProject);
		}
	}
}
