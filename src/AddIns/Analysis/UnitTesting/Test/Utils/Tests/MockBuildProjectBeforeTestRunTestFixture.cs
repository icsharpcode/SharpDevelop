// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			
			createdBuildProject = factory.CreateBuildProjectBeforeTestRun(new[] { project }) as MockBuildProjectBeforeTestRun;
		}
		
		[Test]
		public void CreateBuildProjectBeforeTestRunReturnsObjectPassedToAddBuildProjectBeforeTestRunMethod()
		{
			Assert.AreEqual(buildProject, createdBuildProject);
		}
		
		[Test]
		public void ProjectPassedToBuildProjectObject()
		{
			Assert.AreEqual(new[] { project }, buildProject.Projects);
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
