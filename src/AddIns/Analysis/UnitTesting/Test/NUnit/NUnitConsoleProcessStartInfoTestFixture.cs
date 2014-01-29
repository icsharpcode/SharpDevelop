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
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using Rhino.Mocks;
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
			SD.Services.AddStrictMockService<IProjectService>();
			SD.ProjectService.Stub(p => p.TargetFrameworks).Return(new[] { TargetFramework.Net40Client, TargetFramework.Net40 });
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
