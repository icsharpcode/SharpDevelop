// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTestFrameworksWithNUnitFrameworkSupport : NUnitTestFramework, IRegisteredTestFrameworks
	{
		public ITestFramework GetTestFrameworkForProject(IProject project)
		{
			throw new NotImplementedException();
		}
		
		public ITestRunner CreateTestRunner(IProject project)
		{
			throw new NotImplementedException();
		}
		
		public ITestRunner CreateTestDebugger(IProject project)
		{
			throw new NotImplementedException();
		}
		
		public bool IsBuildNeededBeforeTestRunForProject(IProject project)
		{
			throw new NotImplementedException();
		}
	}
}
