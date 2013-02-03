// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
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
				"-register:user -target:\"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
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
				"-register:user -target:\"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
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
