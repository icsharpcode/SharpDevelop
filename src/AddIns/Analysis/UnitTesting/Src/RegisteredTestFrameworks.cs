// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public bool IsTestMember(IMember member)
		{
			ITestFramework testFramework = GetTestFramework(member);
			if (testFramework != null) {
				return testFramework.IsTestMember(member);
			}
			return false;
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass @class) {
			ITestFramework testFramework = GetTestFramework(@class);
			if (testFramework != null)
				return testFramework.GetTestMembersFor(@class);
			return new TestMember[0];
		}
		
		ITestFramework GetTestFramework(IMember member)
		{
			if (member != null) {
				return GetTestFramework(member.DeclaringType);
			}
			return null;
		}
		
		ITestFramework GetTestFramework(IClass c)
		{
			IProject project = GetProject(c);
			return GetTestFrameworkForProject(project);
		}

		IProject GetProject(IClass c)
		{
			if (c != null) {
				return c.ProjectContent.Project as IProject;
			}
			return null;
		}
		
		public bool IsTestClass(IClass c)
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
