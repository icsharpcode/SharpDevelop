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
