// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunTestsForNamespaceTestFixture : RunTestCommandTestFixtureBase
	{
		MockTestTreeView treeView;
		MockCSharpProject project;
		
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			project = new MockCSharpProject();
			MockBuildProjectBeforeTestRun buildProject = new MockBuildProjectBeforeTestRun();
			context.MockBuildProjectFactory.AddBuildProjectBeforeTestRun(buildProject);
			
			treeView = new MockTestTreeView();
			treeView.SelectedProject = project;
			treeView.SelectedNamespace = "MyNamespace";
			
			runTestCommand.Owner = treeView;
			runTestCommand.Run();
			buildProject.FireBuildCompleteEvent();
		}
		
		[Test]
		public void TestableConditionGetNamespaceReturnsMyNamespace()
		{
			Assert.AreEqual("MyNamespace", TestableCondition.GetNamespace(treeView));
		}
		
		[Test]
		public void SelectedTestsHasNamespaceFilter()
		{
			SelectedTests tests = runTestCommand.TestRunnersCreated[0].SelectedTestsPassedToStartMethod;
			Assert.AreEqual("MyNamespace", tests.NamespaceFilter);
		}
	}
}
