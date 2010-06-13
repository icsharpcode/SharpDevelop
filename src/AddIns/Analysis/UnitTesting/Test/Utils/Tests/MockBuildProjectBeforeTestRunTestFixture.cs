// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockBuildProjectBeforeTestRunTestFixture
	{
		MockBuildProjectBeforeTestRun buildProject;
		MockBuildProjectBeforeTestRun createdBuildProject;
		MockCSharpProject project;
		MockBuildProjectFactory factory;
		
		[SetUp]
		public void Init()
		{
			factory = new MockBuildProjectFactory();
			project = new MockCSharpProject();
			buildProject = new MockBuildProjectBeforeTestRun();
			factory.AddBuildProjectBeforeTestRun(buildProject);
			
			createdBuildProject = factory.CreateBuildProjectBeforeTestRun(project) as MockBuildProjectBeforeTestRun;
		}
		
		[Test]
		public void CreateBuildProjectBeforeTestRunReturnsObjectPassedToAddBuildProjectBeforeTestRunMethod()
		{
			Assert.AreEqual(buildProject, createdBuildProject);
		}
		
		[Test]
		public void ProjectPassedToBuildProjectObject()
		{
			Assert.AreEqual(project, buildProject.Project);
		}
		
		[Test]
		public void CallingCreateBuildProjectBeforeTestRunAgainReturnsNull()
		{
			Assert.IsNull(factory.CreateBuildProjectBeforeTestRun(null));
		}
		
		[Test]
		public void FireBuildCompleteEventFiresBuildCompleteEvent()
		{
			bool eventFired = false;
			buildProject.BuildComplete += delegate { eventFired = true; };
			buildProject.FireBuildCompleteEvent();
			
			Assert.IsTrue(eventFired);
		}
		
		[Test]
		public void IsRunMethodCalledReturnsFalseByDefault()
		{
			Assert.IsFalse(buildProject.IsRunMethodCalled);
		}
		
		[Test]
		public void IsRunMethodCalledReturnsTrueAfterRunMethodCalled()
		{
			buildProject.Run();
			Assert.IsTrue(buildProject.IsRunMethodCalled);
		}
		
		[Test]
		public void LastBuildResultsIsNull()
		{
			Assert.IsNull(buildProject.LastBuildResults);
		}
		
		[Test]
		public void LastBuildResultsIsNotNullAfterRun()
		{
			buildProject.Run();
			Assert.IsNotNull(buildProject.LastBuildResults);
		}
	}
}
