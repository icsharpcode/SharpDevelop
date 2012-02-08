// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface ITestFramework
	{
		bool IsTestMember(IMember member);
		bool IsTestClass(IClass c);
		bool IsTestProject(IProject project);
		
		IEnumerable<TestMember> GetTestMembersFor(IClass @class);
		
		ITestRunner CreateTestRunner();
		ITestRunner CreateTestDebugger();
		
		bool IsBuildNeededBeforeTestRun { get; }
	}
}
