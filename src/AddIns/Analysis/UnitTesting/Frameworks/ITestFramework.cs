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
		/// <summary>
		/// Gets whether this test framework supports the specified project.
		/// </summary>
		bool IsTestProject(IProject project);
		
		/// <summary>
		/// Creates a new test project based on the specified project.
		/// </summary>
		ITestProject CreateTestProject(ITestSolution parentSolution, IProject project);
		
		/*
		bool IsTestMember(IMember member);
		bool IsTestClass(ITypeDefinition testClass);
		
		IEnumerable<TestMember> GetTestMembersFor(ITypeDefinition typeDefinition);
		
		ITestRunner CreateTestRunner();
		ITestRunner CreateTestDebugger();
		
		bool IsBuildNeededBeforeTestRun { get; }*/
	}
}
