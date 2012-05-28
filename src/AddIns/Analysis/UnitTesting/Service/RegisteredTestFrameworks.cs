// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class RegisteredTestFrameworks : IRegisteredTestFrameworks
	{
		List<TestFrameworkDescriptor> testFrameworkDescriptors;
		public const string AddInPath = "/SharpDevelop/UnitTesting/TestFrameworks";
		
		public RegisteredTestFrameworks(IAddInTree addInTree)
		{
			testFrameworkDescriptors = addInTree.BuildItems<TestFrameworkDescriptor>(AddInPath, null);
		}
		
		public ITestFramework GetTestFrameworkForProject(IProject project)
		{
			if (project != null) {
				foreach (TestFrameworkDescriptor descriptor in testFrameworkDescriptors) {
					if (descriptor.IsSupportedProject(project) && descriptor.TestFramework.IsTestProject(project)) {
						return descriptor.TestFramework;
					}
				}
			}
			return null;
		}
		
		public bool IsTestMethod(IMethod method, ICompilation compilation)
		{
			ITestFramework testFramework = GetTestFramework(method, compilation);
			if (testFramework != null) {
				return testFramework.IsTestMethod(method, compilation);
			}
			return false;
		}
		
		public bool IsTestCase(IMethod method, ICompilation compilation)
		{
			ITestFramework testFramework = GetTestFramework(method, compilation);
			if (testFramework != null) {
				return testFramework.IsTestCase(method, compilation);
			}
			return false;
		}
		
		public IEnumerable<IMethod> GetTestMethodsFor(ITypeDefinition type, ICompilation compilation)
		{
			ITestFramework testFramework = GetTestFramework(type, compilation);
			if (testFramework != null)
				return testFramework.GetTestMethodsFor(type);
			return new IMethod[0];
		}
		
		ITestFramework GetTestFramework(IMethod method, ICompilation compilation)
		{
			if (method != null) {
				return GetTestFramework(method.DeclaringTypeDefinition, compilation);
			}
			return null;
		}
		
		ITestFramework GetTestFramework(ITypeDefinition c, ICompilation compilation)
		{
			IProject project = GetProject(c);
			return GetTestFrameworkForProject(project);
		}

		IProject GetProject(ITypeDefinition c)
		{
			if (c != null && ProjectService.OpenSolution != null) {
				return ProjectService.OpenSolution.FindProjectContainingFile(c.Parts[0].ParsedFile.FileName);
			}
			return null;
		}
		
		public bool IsTestClass(ITypeDefinition c, ICompilation compilation)
		{
			ITestFramework testFramework = GetTestFramework(c, compilation);
			if (testFramework != null) {
				return testFramework.IsTestClass(c, compilation);
			}
			return false;
		}
		
		public bool IsTestProject(IProject project)
		{
			ITestFramework testFramework = GetTestFrameworkForProject(project);
			if (testFramework != null) {
				return testFramework.IsTestProject(project);
			}
			return false;
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
