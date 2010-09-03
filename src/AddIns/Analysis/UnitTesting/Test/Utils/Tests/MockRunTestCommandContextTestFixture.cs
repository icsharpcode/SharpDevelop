// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockRunTestCommandContextTestFixture
	{
		IRunTestCommandContext runTestCommandContext;
		
		[SetUp]
		public void Init()
		{
			runTestCommandContext = new MockRunTestCommandContext();
		}
		
		[Test]
		public void UnitTestingOptionsIsNotNull()
		{
			MockRunTestCommandContext context = new MockRunTestCommandContext();
			Assert.IsNotNull(context.UnitTestingOptions);
		}
		
		[Test]
		public void RegisteredTestFrameworksIsMockRegisteredTestFrameworks()
		{
			Assert.IsNotNull(runTestCommandContext.RegisteredTestFrameworks as MockRegisteredTestFrameworks);
		}
		
		[Test]
		public void TaskServiceIsMockTaskService()
		{
			Assert.IsNotNull(runTestCommandContext.TaskService as MockTaskService);
		}
		
		[Test]
		public void WorkbenchIsMockUnitTestWorkbench()
		{
			Assert.IsNotNull(runTestCommandContext.Workbench as MockUnitTestWorkbench);
		}
		
		[Test]
		public void BuildProjectFactoryIsMockBuildProjectFactory()
		{
			Assert.IsNotNull(runTestCommandContext.BuildProjectFactory as MockBuildProjectFactory);
		}
		
		[Test]
		public void BuildOptionsIsMockBuildOptions()
		{
			Assert.IsNotNull(runTestCommandContext.BuildOptions as MockBuildOptions);
		}
		
		[Test]
		public void UnitTestCategoryIsMessageViewWithCategoryNameUnitTests()
		{
			Assert.AreEqual("Unit Tests", runTestCommandContext.UnitTestCategory.Category);
		}
		
		[Test]
		public void UnitTestsPadIsMockUnitTestsPad()
		{
			Assert.IsNotNull(runTestCommandContext.OpenUnitTestsPad as MockUnitTestsPad);
		}
		
		[Test]
		public void MessageServiceIsMockMessageService()
		{
			Assert.IsNotNull(runTestCommandContext.MessageService as MockMessageService);
		}
	}
}
