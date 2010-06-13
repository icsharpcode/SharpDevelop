// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
