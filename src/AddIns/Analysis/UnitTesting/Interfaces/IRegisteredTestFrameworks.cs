// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public interface IRegisteredTestFrameworks
	{
		ITestFramework GetTestFrameworkForProject(IProject project);
		ITestRunner CreateTestRunner(IProject project);
		ITestRunner CreateTestDebugger(IProject project);
		
		bool IsTestMethod(IMethod method, ICompilation compilation);
		bool IsTestCase(IMethod method, ICompilation compilation);
		bool IsTestClass(ITypeDefinition typeDefinition, ICompilation compilation);
		bool IsTestProject(IProject project);
		
		IEnumerable<IMethod> GetTestMethodsFor(ITypeDefinition typeDefinition, ICompilation compilation);

		bool IsBuildNeededBeforeTestRunForProject(IProject project);
	}
}
