// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface ITestFramework
	{
		bool IsTestMember(IMember member);
		bool IsTestClass(ITypeDefinition testClass);
		bool IsTestProject(IProject project);
		
		IEnumerable<TestMember> GetTestMembersFor(TestProject project, ITypeDefinition typeDefinition);
		
		ITestRunner CreateTestRunner();
		ITestRunner CreateTestDebugger();
		
		bool IsBuildNeededBeforeTestRun { get; }
	}
}
