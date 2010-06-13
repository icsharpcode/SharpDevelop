// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class NUnitConsoleProcessStartInfoTestFixture
	{
		ProcessStartInfo info;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
		}
		
		[SetUp]
		public void Init()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests selectedTests = new SelectedTests(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests);
			
			info = app.GetProcessStartInfo();
		}
		
		[Test]
		public void WorkingDirectoryIsNUnitConsoleApplicationDirectory()
		{
			string expectedDirectory = @"C:\SharpDevelop\bin\Tools\NUnit";
			Assert.AreEqual(expectedDirectory, info.WorkingDirectory);
		}
		
		[Test]
		public void FileNameIsNUnitConsoleExe()
		{
			string expectedFileName = @"C:\SharpDevelop\bin\Tools\NUnit\nunit-console-x86.exe";
			Assert.AreEqual(expectedFileName, info.FileName);
		}
		
		[Test]
		public void CommandLineArgumentsAreNUnitConsoleExeCommandLineArguments()
		{
			string expectedCommandLine =
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\"";
			
			Assert.AreEqual(expectedCommandLine, info.Arguments);
		}
	}
}
