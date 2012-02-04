// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface IRegisteredTestFrameworks
	{
		ITestFramework GetTestFrameworkForProject(IProject project);
		ITestRunner CreateTestRunner(IProject project);
		ITestRunner CreateTestDebugger(IProject project);
		
		bool IsTestMember(IMember member);
		bool IsTestClass(IClass c);
		bool IsTestProject(IProject project);
		
		IEnumerable<TestMember> GetTestMembersFor(IClass @class);

		bool IsBuildNeededBeforeTestRunForProject(IProject project);		
	}
}
