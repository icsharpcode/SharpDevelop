// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class NUnitConsoleProcessStartInfoTestFixture : SDTestFixtureBase
	{
		ProcessStartInfo info;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			FileUtility.ApplicationRootPath = @"C:\SharpDevelop";
		}
		
		[SetUp]
		public void Init()
		{
			MockCSharpProject project = new MockCSharpProject();
			NUnitTestProject testProject = new NUnitTestProject(project);
			NUnitConsoleApplication app = new NUnitConsoleApplication(new[] { testProject });
			
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
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\" /noxml";
			
			Assert.AreEqual(expectedCommandLine, info.Arguments);
		}
	}
}
