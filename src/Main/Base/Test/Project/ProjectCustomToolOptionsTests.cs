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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Project
{
	[TestFixture]
	public class ProjectCustomToolOptionsTests
	{
		IProject project;
		Properties projectSpecificProperties;
		ProjectCustomToolOptions projectCustomToolOptions;
		Properties properties;
		
		void CreateProject()
		{
			projectSpecificProperties = new Properties();
			project = MockRepository.GenerateStub<IProject>();
			project.Stub(p => p.Preferences).Return(projectSpecificProperties);
		}
		
		void CreateProjectWithExistingCustomToolProperties(string fileNames)
		{
			CreateProjectWithExistingCustomToolProperties(false, fileNames);
		}
		
		void CreateProjectWithExistingCustomToolProperties(bool runOnBuild, string fileNames = "")
		{
			CreateProject();
			properties = new Properties();
			properties.Set("runOnBuild", runOnBuild);
			properties.Set("fileNames", fileNames);
			projectSpecificProperties.SetNestedProperties("customTool", properties);
		}
		
		void CreateProjectCustomToolsOptions()
		{
			projectCustomToolOptions = new ProjectCustomToolOptions(project);
		}
		
		[Test]
		public void RunCustomToolOnBuild_ProjectHasNoExistingProjectCustomToolProperties_ReturnsFalse()
		{
			CreateProject();
			CreateProjectCustomToolsOptions();
			
			bool run = projectCustomToolOptions.RunCustomToolOnBuild;
			
			Assert.IsFalse(run);
		}
		
		[Test]
		public void FileNames_ProjectHasNoExistingProjectCustomToolProperties_ReturnsEmptyString()
		{
			CreateProject();
			CreateProjectCustomToolsOptions();
			
			string fileNames = projectCustomToolOptions.FileNames;
			
			Assert.AreEqual(String.Empty, fileNames);
		}
		
		[Test]
		public void RunCustomToolOnBuild_ProjectPropertyRunCustomToolOnBuildIsTrue_ReturnsTrue()
		{
			CreateProjectWithExistingCustomToolProperties(runOnBuild: true);
			CreateProjectCustomToolsOptions();
			
			bool run = projectCustomToolOptions.RunCustomToolOnBuild;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void FileNames_ProjectPropertyFileNamesIsNotEmptyString_ReturnsFileName()
		{
			CreateProjectWithExistingCustomToolProperties(fileNames: "T4MVC.tt");
			CreateProjectCustomToolsOptions();
			
			string fileNames = projectCustomToolOptions.FileNames;
			
			Assert.AreEqual("T4MVC.tt", fileNames);
		}
		
		[Test]
		public void RunCustomToolOnBuild_ChangeRunCustomToolOnBuildToTrue_StoredInProjectProperties()
		{
			CreateProjectWithExistingCustomToolProperties(runOnBuild: false);
			CreateProjectCustomToolsOptions();
			
			projectCustomToolOptions.RunCustomToolOnBuild = true;
			
			CreateProjectCustomToolsOptions();
			bool run = projectCustomToolOptions.RunCustomToolOnBuild;
			Assert.IsTrue(run);
		}
		
		[Test]
		public void RunCustomToolOnBuild_ChangeRunCustomToolOnBuildToFalse_StoredInProjectProperties()
		{
			CreateProjectWithExistingCustomToolProperties(runOnBuild: true);
			CreateProjectCustomToolsOptions();
			
			projectCustomToolOptions.RunCustomToolOnBuild = false;
			
			CreateProjectCustomToolsOptions();
			bool run = projectCustomToolOptions.RunCustomToolOnBuild;
			Assert.IsFalse(run);
		}
		
		[Test]
		public void FileNames_ChangeFileNamesFromEmptyStringToFileName_StoredInProjectProperties()
		{
			CreateProjectWithExistingCustomToolProperties(fileNames: String.Empty);
			CreateProjectCustomToolsOptions();
			
			projectCustomToolOptions.FileNames = "abc.tt";
			
			CreateProjectCustomToolsOptions();
			string fileNames = projectCustomToolOptions.FileNames;
			Assert.AreEqual("abc.tt", fileNames);
		}
		
		[Test]
		public void SplitFileNames_FileNamesIsSemiColonSeparatedOfTwoFiles_ReturnsTwoFiles()
		{
			CreateProjectWithExistingCustomToolProperties("a.t4;b.t4");
			CreateProjectCustomToolsOptions();
			
			IList<string> fileNames = projectCustomToolOptions.SplitFileNames();
			
			string[] expectedFileNames = new string[] { 
				"a.t4",
				"b.t4"
			};
			CollectionAssert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void SplitFileNames_FileNamesIsCommaSeparatedOfTwoFiles_ReturnsTwoFiles()
		{
			CreateProjectWithExistingCustomToolProperties("a.t4,b.t4");
			CreateProjectCustomToolsOptions();
			
			IList<string> fileNames = projectCustomToolOptions.SplitFileNames();
			
			string[] expectedFileNames = new string[] { 
				"a.t4",
				"b.t4"
			};
			CollectionAssert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void SplitFileNames_FileNamesIsSemiColonSeparatedOfTwoFilesWithWhitespace_ReturnsTwoFilesWithWhitespaceRemoved()
		{
			CreateProjectWithExistingCustomToolProperties("   a.t4  ;  b.t4  ");
			CreateProjectCustomToolsOptions();
			
			IList<string> fileNames = projectCustomToolOptions.SplitFileNames();
			
			string[] expectedFileNames = new string[] { 
				"a.t4",
				"b.t4"
			};
			CollectionAssert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void SplitFileNames_FileNamesIsTwoFilesEachOnSeparateLine_ReturnsTwoFiles()
		{
			string text =
				"a.t4\r\n" +
				"b.t4";
			CreateProjectWithExistingCustomToolProperties(text);
			CreateProjectCustomToolsOptions();
			
			IList<string> fileNames = projectCustomToolOptions.SplitFileNames();
			
			string[] expectedFileNames = new string[] { 
				"a.t4",
				"b.t4"
			};
			CollectionAssert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void SplitFileNames_FileNamesIsTwoFilesEachOnSeparateLineWithEmptyLineBetweenThemAndOneAtEnd_ReturnsTwoFiles()
		{
			string text =
				"a.t4\r\n" +
				"\r\n" +
				"b.t4\r\n";
			CreateProjectWithExistingCustomToolProperties(text);
			CreateProjectCustomToolsOptions();
			
			IList<string> fileNames = projectCustomToolOptions.SplitFileNames();
			
			string[] expectedFileNames = new string[] { 
				"a.t4",
				"b.t4"
			};
			CollectionAssert.AreEqual(expectedFileNames, fileNames);
		}
	}
}
