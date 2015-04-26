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
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Testing
{
	[TestFixture]
	public class OpenCoverApplicationTests : SDTestFixtureBase
	{
		NUnitConsoleApplication nunitConsoleApp;
		UnitTestingOptions options;
		OpenCoverApplication openCoverApp;
		OpenCoverSettings openCoverSettings;
		MockCSharpProject project;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddStrictMockService<IProjectService>();
			SD.ProjectService.Stub(p => p.TargetFrameworks).Return(new[] { TargetFramework.Net40Client, TargetFramework.Net40 });
		}
		
		[Test]
		public void FileNameWhenOpenCoverApplicationConstructedWithFileNameParameterMatchesFileNameParameter()
		{
			string expectedFileName = @"d:\projects\OpenCover.exe";
			CreateOpenCoverApplication(expectedFileName);
			Assert.AreEqual(expectedFileName, openCoverApp.FileName);
		}
		
		void CreateOpenCoverApplication(string fileName)
		{
			CreateNUnitConsoleApplication();
			openCoverSettings = new OpenCoverSettings();
			openCoverApp = new OpenCoverApplication(
				fileName,
				nunitConsoleApp.GetProcessStartInfo(),
				openCoverSettings,
				project);
		}
		
		void CreateNUnitConsoleApplication()
		{
			project = new MockCSharpProject();
			project.FileName = FileName.Create(@"c:\projects\MyTests\MyTests.csproj");
			
			var testProject = new NUnitTestProject(project);
			options = new UnitTestingOptions(new Properties());
			nunitConsoleApp = new NUnitConsoleApplication(new [] { testProject }, options);
		}
		
		[Test]
		public void FileNameWhenOpenCoverApplicationConstructedWithNoParametersIsDeterminedFromFileUtilityAppRootPath()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreateOpenCoverApplicationWithoutFileName();
			string expectedPath = @"d:\sharpdevelop\bin\Tools\OpenCover\OpenCover.Console.exe";
			Assert.AreEqual(expectedPath, openCoverApp.FileName);
		}
		
		void CreateOpenCoverApplicationWithoutFileName()
		{
			CreateNUnitConsoleApplication();
			openCoverApp = new OpenCoverApplication(nunitConsoleApp.GetProcessStartInfo(), new OpenCoverSettings(), project);
		}
		
		[Test]
		public void FileNameWhenTakenFromFileUtilityAppRootPathRemovesDotDotCharacters()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop\..\sharpdevelop";
			CreateOpenCoverApplicationWithoutFileName();
			string expectedPath = @"d:\sharpdevelop\bin\Tools\OpenCover\OpenCover.Console.exe";
			Assert.AreEqual(expectedPath, openCoverApp.FileName);
		}
		
		[Test]
		public void TargetIsNUnitConsoleApplicationFileName()
		{
			CreateOpenCoverApplication();
			Assert.AreEqual(nunitConsoleApp.FileName, openCoverApp.Target);
		}
		
		void CreateOpenCoverApplication()
		{
			string fileName = @"d:\openCover\OpenCover.exe";
			CreateOpenCoverApplication(fileName);
		}
		
		[Test]
		public void GetTargetArgumentsReturnsNUnitConsoleApplicationCommandLineArguments()
		{
			CreateOpenCoverApplication();
			Assert.AreEqual(nunitConsoleApp.GetArguments(), openCoverApp.GetTargetArguments());
		}
		
		[Test]
		public void GetTargetWorkingDirectoryReturnsWorkingDirectorySpecifiedByNUnitConsoleApplication()
		{
			CreateOpenCoverApplication();
			string expectedTargetWorkingDirectory = @"c:\projects\MyTests\bin\Debug";
			Assert.AreEqual(expectedTargetWorkingDirectory, openCoverApp.GetTargetWorkingDirectory());
		}
		
		[Test]
		public void CodeCoverageResultsFileNameReturnsCoverageXmlFileInsideOpenCoverDirectoryInsideProjectDirectory()
		{
			CreateOpenCoverApplication();
			string expectedOutputDirectory = 
				@"c:\projects\MyTests\OpenCover\coverage.xml";
			
			Assert.AreEqual(expectedOutputDirectory, openCoverApp.CodeCoverageResultsFileName);
		}
		
		[Test]
		public void SettingsReturnsOpenCoverSettingsPassedToConstructor()
		{
			CreateOpenCoverApplication();
			Assert.AreEqual(openCoverSettings, openCoverApp.Settings);
		}
		
		[Test]
		public void GetProcessStartInfoReturnsStartInfoWhereFileNameIsOpenCoverAppFileName()
		{
			string openCoverAppFileName = @"d:\projects\OpenCover.exe";
			CreateOpenCoverApplication(openCoverAppFileName);
			ProcessStartInfo processStartInfo = openCoverApp.GetProcessStartInfo();
			
			Assert.AreEqual(openCoverAppFileName, processStartInfo.FileName);
		}
		
		[Test]
		public void GetProcessStartInfoWhenNoIncludedItemsReturnsCommandLineWithIncludeForAllAssemblies()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreateOpenCoverApplication();
			ProcessStartInfo processStartInfo = openCoverApp.GetProcessStartInfo();
			
			string expectedCommandLine =
				"-register:user -mergebyhash -target:\"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"-targetdir:\"c:\\projects\\MyTests\\bin\\Debug\" " +
				"-targetargs:\"\\\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\\\" /noxml\" " + 
				"-output:\"c:\\projects\\MyTests\\OpenCover\\coverage.xml\" " +
				"-filter:\"+[*]* \"";

			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
		
		[Test]
		public void GetProcessStartInfoWhenHaveIncludedAndExcludedItemsReturnsCommandLineWithIncludeAndExcludeCommandLineArgs()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreateOpenCoverApplication();
			
			openCoverSettings.Include.Add("[MyTests]*");
			openCoverSettings.Include.Add("[MoreTests]*");
			
			openCoverSettings.Exclude.Add("[NUnit.Framework]*");
			openCoverSettings.Exclude.Add("[MyProject]*");
			
			ProcessStartInfo processStartInfo = openCoverApp.GetProcessStartInfo();
			
			string expectedCommandLine =
				"-register:user -mergebyhash -target:\"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"-targetdir:\"c:\\projects\\MyTests\\bin\\Debug\" " +
				"-targetargs:\"\\\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\\\" /noxml\" " + 
				"-output:\"c:\\projects\\MyTests\\OpenCover\\coverage.xml\" " +
				"-filter:\"+[MyTests]* " +
				"+[MoreTests]* " +
				"-[NUnit.Framework]* " +
				"-[MyProject]*\"";

			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
	}
}
