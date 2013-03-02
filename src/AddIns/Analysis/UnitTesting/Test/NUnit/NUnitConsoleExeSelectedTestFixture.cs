// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
