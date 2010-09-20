// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockRegisteredTestFrameworks : MockTestFramework, IRegisteredTestFrameworks
	{
		Dictionary<IProject, ITestFramework> testFrameworks =
			new Dictionary<IProject, ITestFramework>();
		
		public void AddTestFrameworkForProject(IProject project, ITestFramework testFramework)
		{
			testFrameworks.Add(project, testFramework);
		}
		
		public ITestFramework GetTestFrameworkForProject(IProject project)
		{
			ITestFramework testFramework = null;
			if (testFrameworks.TryGetValue(project, out testFramework)) {
				return testFramework;
			}
			return null;
		}
		
		public ITestRunner CreateTestRunner(IProject project)
		{
			ITestFramework testFramework = GetTestFrameworkForProject(project);
			if (testFramework != null) {
				return testFramework.CreateTestRunner();
			}
			return null;
		}
		
		public ITestRunner CreateTestDebugger(IProject project)
		{
			ITestFramework testFramework = GetTestFrameworkForProject(project);
			if (testFramework != null) {
				return testFramework.CreateTestDebugger();
			}
			return null;
		}
		
		public bool IsBuildNeededBeforeTestRunForProject(IProject project)
		{
			ITestFramework testFramework = GetTestFrameworkForProject(project);
			if (testFramework != null) {
				return testFramework.IsBuildNeededBeforeTestRun;
			}
			return true;
		}
	}
}
