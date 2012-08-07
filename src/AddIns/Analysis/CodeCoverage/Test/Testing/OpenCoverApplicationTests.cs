// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.CodeCoverage.Tests.Testing
{
	[TestFixture]
	public class OpenCoverApplicationTests
	{
		NUnitConsoleApplication nunitConsoleApp;
		SelectedTests selectedTests;
		UnitTestingOptions options;
		OpenCoverApplication openCoverApp;
		OpenCoverSettings openCoverSettings;
		
		[Test]
		public void FileNameWhenPartCoverApplicationConstructedWithFileNameParameterMatchesFileNameParameter()
		{
			string expectedFileName = @"d:\projects\PartCover.exe";
			CreateOpenCoverApplication(expectedFileName);
			Assert.AreEqual(expectedFileName, openCoverApp.FileName);
		}
		
		void CreateOpenCoverApplication(string fileName)
		{
			CreateNUnitConsoleApplication();
			openCoverSettings = new OpenCoverSettings();
			openCoverApp = new OpenCoverApplication(fileName, nunitConsoleApp, openCoverSettings);
		}
		
		void CreateNUnitConsoleApplication()
		{
			MockCSharpProject project = new MockCSharpProject();
			selectedTests = new SelectedTests(project);
			
			options = new UnitTestingOptions(new Properties());
			nunitConsoleApp = new NUnitConsoleApplication(selectedTests, options);
		}
		
		[Test]
		public void FileNameWhenPartCoverApplicationConstructedWithNoParametersIsDeterminedFromFileUtilityAppRootPath()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreatePartCoverApplicationWithoutFileName();
			string expectedPath = @"d:\sharpdevelop\bin\Tools\OpenCover\OpenCover.Console.exe";
			Assert.AreEqual(expectedPath, openCoverApp.FileName);
		}
		
		void CreatePartCoverApplicationWithoutFileName()
		{
			CreateNUnitConsoleApplication();
			openCoverApp = new OpenCoverApplication(nunitConsoleApp, new OpenCoverSettings());
		}
		
		[Test]
		public void FileNameWhenTakenFromFileUtilityAppRootPathRemovesDotDotCharacters()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop\..\sharpdevelop";
			CreatePartCoverApplicationWithoutFileName();
			string expectedPath = @"d:\sharpdevelop\bin\Tools\OpenCover\OpenCover.Console.exe";
			Assert.AreEqual(expectedPath, openCoverApp.FileName);
		}
		
		[Test]
		public void TargetIsNUnitConsoleApplicationFileName()
		{
			CreatePartCoverApplication();
			Assert.AreEqual(nunitConsoleApp.FileName, openCoverApp.Target);
		}
		
		void CreatePartCoverApplication()
		{
			string fileName = @"d:\partcover\PartCover.exe";
			CreateOpenCoverApplication(fileName);
		}
		
		[Test]
		public void GetTargetArgumentsReturnsNUnitConsoleApplicationCommandLineArguments()
		{
			CreatePartCoverApplication();
			Assert.AreEqual(nunitConsoleApp.GetArguments(), openCoverApp.GetTargetArguments());
		}
		
		[Test]
		public void GetTargetWorkingDirectoryReturnsWorkingDirectoryForProjectOutput()
		{
			CreatePartCoverApplication();
			string expectedTargetWorkingDirectory = @"c:\projects\MyTests\bin\Debug";
			Assert.AreEqual(expectedTargetWorkingDirectory, openCoverApp.GetTargetWorkingDirectory());
		}
		
		[Test]
		public void CodeCoverageResultsFileNameReturnsCoverageXmlFileInsidePartCoverDirectoryInsideProjectDirectory()
		{
			CreatePartCoverApplication();
			string expectedOutputDirectory = 
				@"c:\projects\MyTests\OpenCover\coverage.xml";
			
			Assert.AreEqual(expectedOutputDirectory, openCoverApp.CodeCoverageResultsFileName);
		}
		
		[Test]
		public void SettingsReturnsPartCoverSettingsPassedToConstructor()
		{
			CreatePartCoverApplication();
			Assert.AreEqual(openCoverSettings, openCoverApp.Settings);
		}
		
		[Test]
		public void GetProcessStartInfoReturnsStartInfoWhereFileNameIsPartCoverAppFileName()
		{
			string partCoverAppFileName = @"d:\projects\partcover.exe";
			CreateOpenCoverApplication(partCoverAppFileName);
			ProcessStartInfo processStartInfo = openCoverApp.GetProcessStartInfo();
			
			Assert.AreEqual(partCoverAppFileName, processStartInfo.FileName);
		}
		
		[Test]
		public void GetProcessStartInfoWhenNoIncludedItemsReturnsCommandLineWithIncludeForAllAssemblies()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreatePartCoverApplication();
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
			CreatePartCoverApplication();
			
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
