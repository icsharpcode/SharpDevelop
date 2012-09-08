// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class RegisteredTestFrameworks : IRegisteredTestFrameworks
	{
		IReadOnlyList<TestFrameworkDescriptor> testFrameworkDescriptors;
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
		
		public bool IsTestMember(IMember member)
		{
			ITestFramework testFramework = GetTestFramework(member);
			if (testFramework != null) {
				return testFramework.IsTestMember(member);
			}
			return false;
		}
		
		ITestFramework GetTestFramework(IEntity entity)
		{
			if (entity != null) {
				return GetTestFrameworkForProject(entity.ParentAssembly.GetProject());
			}
			return null;
		}
		
		public bool IsTestClass(ITypeDefinition c)
		{
			ITestFramework testFramework = GetTestFramework(c);
			if (testFramework != null) {
				return testFramework.IsTestClass(c);
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
