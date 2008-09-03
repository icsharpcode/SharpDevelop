// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			Assert.AreEqual(@"D:\SharpDevelop\bin\Tools\NUnit\nunit-console.exe", helper.UnitTestApplication);
		}
		
		[Test]
		public void NUnitConsole32BitUsedWhenTargetCpuIs32Bit()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.ActiveConfiguration = "Debug";
			project.ActivePlatform = "AnyCPU";
			project.SetProperty("PlatformTarget", "x86");
				
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
