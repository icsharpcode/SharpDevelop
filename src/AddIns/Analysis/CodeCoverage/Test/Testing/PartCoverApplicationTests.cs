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
	public class PartCoverApplicationTests
	{
		NUnitConsoleApplication nunitConsoleApp;
		SelectedTests selectedTests;
		UnitTestingOptions options;
		PartCoverApplication partCoverApp;
		PartCoverSettings partCoverSettings;
		
		[Test]
		public void FileNameWhenPartCoverApplicationConstructedWithFileNameParameterMatchesFileNameParameter()
		{
			string expectedFileName = @"d:\projects\PartCover.exe";
			CreatePartCoverApplication(expectedFileName);
			Assert.AreEqual(expectedFileName, partCoverApp.FileName);
		}
		
		void CreatePartCoverApplication(string fileName)
		{
			CreateNUnitConsoleApplication();
			partCoverSettings = new PartCoverSettings();
			partCoverApp = new PartCoverApplication(fileName, nunitConsoleApp, partCoverSettings);
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
			string expectedPath = @"d:\sharpdevelop\bin\Tools\PartCover\PartCover.exe";
			Assert.AreEqual(expectedPath, partCoverApp.FileName);
		}
		
		void CreatePartCoverApplicationWithoutFileName()
		{
			CreateNUnitConsoleApplication();
			partCoverApp = new PartCoverApplication(nunitConsoleApp, new PartCoverSettings());
		}
		
		[Test]
		public void FileNameWhenTakenFromFileUtilityAppRootPathRemovesDotDotCharacters()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop\..\sharpdevelop";
			CreatePartCoverApplicationWithoutFileName();
			string expectedPath = @"d:\sharpdevelop\bin\Tools\PartCover\PartCover.exe";
			Assert.AreEqual(expectedPath, partCoverApp.FileName);
		}
		
		[Test]
		public void TargetIsNUnitConsoleApplicationFileName()
		{
			CreatePartCoverApplication();
			Assert.AreEqual(nunitConsoleApp.FileName, partCoverApp.Target);
		}
		
		void CreatePartCoverApplication()
		{
			string fileName = @"d:\partcover\PartCover.exe";
			CreatePartCoverApplication(fileName);
		}
		
		[Test]
		public void GetTargetArgumentsReturnsNUnitConsoleApplicationCommandLineArguments()
		{
			CreatePartCoverApplication();
			Assert.AreEqual(nunitConsoleApp.GetArguments(), partCoverApp.GetTargetArguments());
		}
		
		[Test]
		public void GetTargetWorkingDirectoryReturnsWorkingDirectoryForProjectOutput()
		{
			CreatePartCoverApplication();
			string expectedTargetWorkingDirectory = @"c:\projects\MyTests\bin\Debug";
			Assert.AreEqual(expectedTargetWorkingDirectory, partCoverApp.GetTargetWorkingDirectory());
		}
		
		[Test]
		public void CodeCoverageResultsFileNameReturnsCoverageXmlFileInsidePartCoverDirectoryInsideProjectDirectory()
		{
			CreatePartCoverApplication();
			string expectedOutputDirectory = 
				@"c:\projects\MyTests\PartCover\coverage.xml";
			
			Assert.AreEqual(expectedOutputDirectory, partCoverApp.CodeCoverageResultsFileName);
		}
		
		[Test]
		public void SettingsReturnsPartCoverSettingsPassedToConstructor()
		{
			CreatePartCoverApplication();
			Assert.AreEqual(partCoverSettings, partCoverApp.Settings);
		}
		
		[Test]
		public void GetProcessStartInfoReturnsStartInfoWhereFileNameIsPartCoverAppFileName()
		{
			string partCoverAppFileName = @"d:\projects\partcover.exe";
			CreatePartCoverApplication(partCoverAppFileName);
			ProcessStartInfo processStartInfo = partCoverApp.GetProcessStartInfo();
			
			Assert.AreEqual(partCoverAppFileName, processStartInfo.FileName);
		}
		
		[Test]
		public void GetProcessStartInfoWhenNoIncludedItemsReturnsCommandLineWithIncludeForAllAssemblies()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreatePartCoverApplication();
			ProcessStartInfo processStartInfo = partCoverApp.GetProcessStartInfo();
			
			string expectedCommandLine =
				"--target \"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"--target-work-dir \"c:\\projects\\MyTests\\bin\\Debug\" " +
				"--target-args \"\\\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\\\" /noxml\" " + 
				"--output \"c:\\projects\\MyTests\\PartCover\\coverage.xml\" " +
				"--include [*]*";

			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
		
		[Test]
		public void GetProcessStartInfoWhenHaveIncludedAndExcludedItemsReturnsCommandLineWithIncludeAndExcludeCommandLineArgs()
		{
			FileUtility.ApplicationRootPath = @"d:\sharpdevelop";
			CreatePartCoverApplication();
			
			partCoverSettings.Include.Add("[MyTests]*");
			partCoverSettings.Include.Add("[MoreTests]*");
			
			partCoverSettings.Exclude.Add("[NUnit.Framework]*");
			partCoverSettings.Exclude.Add("[MyProject]*");
			
			ProcessStartInfo processStartInfo = partCoverApp.GetProcessStartInfo();
			
			string expectedCommandLine =
				"--target \"d:\\sharpdevelop\\bin\\Tools\\NUnit\\nunit-console-x86.exe\" " +
				"--target-work-dir \"c:\\projects\\MyTests\\bin\\Debug\" " +
				"--target-args \"\\\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\\\" /noxml\" " + 
				"--output \"c:\\projects\\MyTests\\PartCover\\coverage.xml\" " +
				"--include [MyTests]* " +
				"--include [MoreTests]* " +
				"--exclude [NUnit.Framework]* " +
				"--exclude [MyProject]*";

			Assert.AreEqual(expectedCommandLine, processStartInfo.Arguments);
		}
	}
}
