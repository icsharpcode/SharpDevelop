// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTestFramework : ITestFramework
	{
		IMember isTestMemberParameterUsed;
		List<string> testMembers = new List<string>();
		ITypeDefinition isTestClassParameterUsed;
		List<string> testClasses = new List<string>();
		IProject isTestProjectParameterUsed;
		List<IProject> testProjects = new List<IProject>();
		List<MockTestRunner> testRunnersCreated = new List<MockTestRunner>();
		List<MockTestRunner> testDebuggersCreated = new List<MockTestRunner>();
		bool buildNeededBeforeTestRun = true;
		
		public MockTestFramework()
		{
		}
		
		public bool IsTestMember(IMember member)
		{
			isTestMemberParameterUsed = member;
			return testMembers.Contains(member.ReflectionName);
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(ITypeDefinition @class) {
			throw new NotImplementedException();
		}
		
		public IMember IsTestMemberParameterUsed {
			get { return isTestMemberParameterUsed; }
		}
		
		public void AddTestMember(string reflectionName)
		{
			testMembers.Add(reflectionName);
		}
		
		public bool IsTestClass(ITypeDefinition c)
		{
			isTestClassParameterUsed = c;
			return testClasses.Contains(c.ReflectionName);
		}
		
		public ITypeDefinition IsTestClassParameterUsed {
			get { return isTestClassParameterUsed; }
		}
		
		public void AddTestClass(string reflectionName)
		{
			testClasses.Add(reflectionName);
		}
		
		public bool IsTestProject(IProject project)
		{
			isTestProjectParameterUsed = project;
			return testProjects.Contains(project);
		}
		
		public IProject IsTestProjectParameterUsed {
			get { return isTestProjectParameterUsed; }
		}
		
		public void AddTestProject(IProject project)
		{
			testProjects.Add(project);
		}
		
		public ITestRunner CreateTestRunner()
		{
			MockTestRunner testRunner = new MockTestRunner();
			testRunnersCreated.Add(testRunner);
			return testRunner;
		}
		
		public List<MockTestRunner> TestRunnersCreated {
			get { return testRunnersCreated; }
		}
		
		public ITestRunner CreateTestDebugger()
		{
			MockTestRunner testRunner = new MockTestRunner();
			testDebuggersCreated.Add(testRunner);
			return testRunner;
		}
		
		public List<MockTestRunner> TestDebuggersCreated {
			get { return testDebuggersCreated; }
		}
		
		public bool IsBuildNeededBeforeTestRun {
			get { return buildNeededBeforeTestRun; }
			set { buildNeededBeforeTestRun = value; }
		}
	}
}
