// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTestFramework : ITestFramework
	{
		IMember isTestMemberParameterUsed;
		List<IMember> testMembers = new List<IMember>();
		IClass isTestClassParameterUsed;
		List<IClass> testClasses = new List<IClass>();
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
			return testMembers.Contains(member);
		}
		
		public IEnumerable<TestMember> GetTestMembersFor(IClass @class) {
			return testMembers.Where(member => member.DeclaringType == @class).Select(member => new TestMember(member));
		}
		
		public IMember IsTestMemberParameterUsed {
			get { return isTestMemberParameterUsed; }
		}
		
		public void AddTestMember(IMember member)
		{
			testMembers.Add(member);
		}
		
		public bool IsTestClass(IClass c)
		{
			isTestClassParameterUsed = c;
			return testClasses.Contains(c);
		}
		
		public IClass IsTestClassParameterUsed {
			get { return isTestClassParameterUsed; }
		}
		
		public void AddTestClass(IClass c)
		{
			testClasses.Add(c);
		}
		
		public void RemoveTestClass(IClass c)
		{
			testClasses.Remove(c);
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
