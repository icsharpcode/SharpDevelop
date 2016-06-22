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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using ICSharpCode.UnitTesting;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;
using ICSharpCode.SharpDevelop;

namespace UnitTesting.Tests.NUnit
{
	/// <summary>
	/// If the project explicitly targets 32 bit (x86) architecture then nunit-console-x86.exe should be
	/// used. Otherwise the normal nunit-console.exe is used.
	/// </summary>
	[TestFixture]
	public class NUnitConsoleExeSelectedTestFixture : SDTestFixtureBase
	{
		MockCSharpProject project;
		NUnitTestProject testProject;
		string oldRootPath;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddStrictMockService<IProjectService>();
			SD.ProjectService.Stub(p => p.TargetFrameworks).Return(new[] { TargetFramework.Net20, TargetFramework.Net30, TargetFramework.Net35, TargetFramework.Net35Client, TargetFramework.Net40Client, TargetFramework.Net40 });
		}
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			oldRootPath = FileUtility.ApplicationRootPath;
			FileUtility.ApplicationRootPath = @"D:\SharpDevelop";
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			FileUtility.ApplicationRootPath = oldRootPath;
		}
		
		[SetUp]
		public void SetUp()
		{
			project = new MockCSharpProject();
			testProject = new NUnitTestProject(project);
		}
		
		[Test]
		public void NothingSpecified()
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe", app.FileName);
		}
		
		[Test]
		public void TargetCpuAnyCPUDotnet2()
		{
			project.ActiveConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			project.SetProperty("PlatformTarget", "AnyCPU");
			project.SetProperty("TargetFrameworkVersion", "v3.5");
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			// We use 32-bit NUnit to test AnyCPU projects because the debugger doesn't support 64-bit
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-dotnet2-x86.exe", app.FileName);
		}
		
		[Test]
		public void TargetCpuAnyCPUDotnet45()
		{
			project.ActiveConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			project.SetProperty("PlatformTarget", "AnyCPU");
			project.SetProperty("TargetFrameworkVersion", "v4.5");
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			// We use 32-bit NUnit to test AnyCPU projects because the debugger doesn't support 64-bit
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe", app.FileName);
		}
		
		[Test]
		public void TargetCpuX64Dotnet2()
		{
			project.ActiveConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			project.SetProperty("PlatformTarget", "x64");
			project.SetProperty("TargetFrameworkVersion", "v3.5");
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-dotnet2.exe", app.FileName);
		}
		
		[Test]
		public void TargetCpuX64Dotnet45()
		{
			project.ActiveConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			project.SetProperty("PlatformTarget", "x64");
			project.SetProperty("TargetFrameworkVersion", "v4.5");
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console.exe", app.FileName);
		}
		
		[Test]
		public void NUnitConsole32BitUsedWhenTargetCpuIs32BitDotnet2()
		{
			project.ActiveConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			project.SetProperty("PlatformTarget", "x86");
			project.SetProperty("TargetFrameworkVersion", "v3.5");
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-dotnet2-x86.exe", app.FileName);
		}
		
		[Test]
		public void NUnitConsole32BitUsedWhenTargetCpuIs32Bit()
		{
			project.ActiveConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			project.SetProperty("PlatformTarget", "x86");
			
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe", app.FileName);
		}
		
		[Test]
		public void NotMSBuildBasedProject()
		{
			ProjectLoadInformation info = new ProjectLoadInformation(MockSolution.Create(), FileName.Create(@"C:\Projects\Test.proj"), "Test");
			
			MissingProject project = new MissingProject(info);
			ITestProject testProject = new NUnitTestProject(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			
			Assert.AreEqual(project.GetType().BaseType, typeof(AbstractProject), "MissingProject should be derived from AbstractProject.");
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console.exe", app.FileName);
		}
	}
}
