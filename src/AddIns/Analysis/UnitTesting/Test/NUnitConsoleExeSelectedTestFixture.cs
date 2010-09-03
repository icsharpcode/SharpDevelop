// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using ICSharpCode.UnitTesting;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests
{
	/// <summary>
	/// If the project explicitly targets 32 bit (x86) architecture then nunit-console-x86.exe should be
	/// used. Otherwise the normal nunit-console.exe is used.
	/// </summary>
	[TestFixture]
	public class NUnitConsoleExeSelectedTestFixture
	{
		string oldRootPath;
		
		[TestFixtureSetUpAttribute]
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
		
		[Test]
		public void NothingSpecified()
		{
			MockCSharpProject project = new MockCSharpProject();
			UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
			helper.Initialize(project, null);
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-dotnet2-x86.exe", helper.UnitTestApplication);
		}
		
		
		[Test]
		public void TargetCpuAnyCPUDotnet2()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.ActiveConfiguration = "Debug";
			project.ActivePlatform = "AnyCPU";
			project.SetProperty("PlatformTarget", "AnyCPU");
			project.SetProperty("TargetFrameworkVersion", "v3.5");
			
			UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
			helper.Initialize(project, null);
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-dotnet2.exe", helper.UnitTestApplication);
		}
		
		[Test]
		public void NUnitConsole32BitUsedWhenTargetCpuIs32BitDotnet2()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.ActiveConfiguration = "Debug";
			project.ActivePlatform = "AnyCPU";
			project.SetProperty("PlatformTarget", "x86");
			project.SetProperty("TargetFrameworkVersion", "v3.5");
			
			UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
			helper.Initialize(project, null);
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-dotnet2-x86.exe", helper.UnitTestApplication);
		}
		
		[Test]
		public void NUnitConsole32BitUsedWhenTargetCpuIs32Bit()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.ActiveConfiguration = "Debug";
			project.ActivePlatform = "AnyCPU";
			project.SetProperty("PlatformTarget", "x86");
			project.SetProperty("TargetFrameworkVersion", "v4.0");
			
			UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
			helper.Initialize(project, null);
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe", helper.UnitTestApplication);
		}
		
		[Test]
		public void NotMSBuildBasedProject()
		{
			MissingProject project = new MissingProject(@"C:\Projects\Test.proj", "Test");
			UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
			helper.Initialize(project, null);
			
			Assert.AreEqual(project.GetType().BaseType, typeof(AbstractProject), "MissingProject should be derived from AbstractProject.");
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console.exe", helper.UnitTestApplication);
		}
	}
}
