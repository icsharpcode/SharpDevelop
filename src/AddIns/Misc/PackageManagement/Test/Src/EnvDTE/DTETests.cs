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
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class DTETests
	{
		DTE dte;
		FakePackageManagementProjectService fakeProjectService;
		FakeFileService fakeFileService;
		ISolution fakeSolution;
		
		void CreateDTE(string fileName = @"d:\projects\MyProject\MyProject.sln")
		{
			ICSharpCode.SharpDevelop.SD.InitializeForUnitTests();
			fakeProjectService = new FakePackageManagementProjectService();
			OpenSolution(fileName);
			fakeFileService = new FakeFileService(null);
			dte = new DTE(fakeProjectService, fakeFileService);
		}
		
		void OpenSolution(string fileName)
		{
			fakeSolution = MockRepository.GenerateStub<ISolution>();
			var sections = new SimpleModelCollection<SolutionSection>(new SolutionSection[0]);
			fakeSolution.Stub(s => s.GlobalSections).Return(sections);
			fakeProjectService.OpenSolution = fakeSolution;
			SetOpenSolutionFileName(fileName);
		}
		
		void NoOpenSolution()
		{
			fakeProjectService.OpenSolution = null;
		}
		
		void SetOpenSolutionFileName(string fileName)
		{
			fakeSolution.Stub(s => s.FileName).Return(new FileName(fileName));
		}
		
		TestableProject AddProjectToSolution(string projectName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(projectName);
			fakeProjectService.AddProject(project);
			return project;
		}
		
		[Test]
		public void SolutionFullName_SolutionIsOpen_ReturnsSolutionFileName()
		{
			string fileName = @"d:\projects\myproject\myproject.sln";
			CreateDTE(fileName);
			
			string fullName = dte.Solution.FullName;
			
			Assert.AreEqual(fileName, fullName);
		}
		
		[Test]
		public void SolutionFileName_SolutionIsOpen_ReturnsSolutionFileName()
		{
			string expectedFileName = @"d:\projects\myproject\myproject.sln";
			CreateDTE(expectedFileName);
			
			string fileName = dte.Solution.FileName;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Solution_NoOpenSolution_ReturnsNull()
		{
			CreateDTE();
			NoOpenSolution();
			
			global::EnvDTE.Solution solution = dte.Solution;
			
			Assert.IsNull(solution);
		}
		
		[Test]
		public void Properties_LookForFontsAndColorsCategoryInTextEditorPage_ReturnsProperties()
		{
			CreateDTE();
			
			global::EnvDTE.Properties properties = dte.Properties("FontsAndColors", "TextEditor");
			
			Assert.IsNotNull(properties);
		}
		
		[Test]
		public void Properties_LookForUnknownCategoryAndPage_ReturnsNull()
		{
			CreateDTE();
			
			global::EnvDTE.Properties properties = dte.Properties("UnknownCategory", "UnknownPage");
			
			Assert.IsNull(properties);
		}
		
		[Test]
		public void Properties_LookForFontsAndColorsCategoryInTextEditorPageInUpperCase_ReturnsTextEditorFontsAndColorsProperties()
		{
			CreateDTE();
			
			global::EnvDTE.Properties properties = dte.Properties("FONTSANDCOLORS", "TEXTEDITOR");
			
			Assert.IsNotNull(properties);
		}
		
		[Test]
		public void ActiveSolutionProjects_NoSolutionOpen_ReturnsEmptyArray()
		{
			CreateDTE();
			NoOpenSolution();
			
			Array projects = dte.ActiveSolutionProjects as Array;
			
			Assert.AreEqual(0, projects.Length);
		}
		
		[Test]
		public void ActiveSolutionProjects_SolutionHasOneProject_ReturnsArrayWithOneItem()
		{
			CreateDTE();
			AddProjectToSolution("ProjectA");
			
			Array projects = dte.ActiveSolutionProjects as Array;
			
			Assert.AreEqual(1, projects.Length);
		}
		
		[Test]
		public void ActiveSolutionProjects_SolutionHasOneProject_ReturnsArrayContainingProject()
		{
			CreateDTE();
			TestableProject expectedProject = AddProjectToSolution("ProjectA");
			
			Array projects = dte.ActiveSolutionProjects as Array;
			
			Project project = projects.GetValue(0) as Project;
			string name = project.Name;
			
			Assert.AreEqual("ProjectA", name);
		}
		
		[Test]
		public void Version_CheckVersion_Returns10()
		{
			CreateDTE();
			
			string version = dte.Version;
			
			Assert.AreEqual("10.0", version);
		}
		
		[Test]
		public void Solution_SetGlobalsVariableValueAndThenAccessSolutionPropertyAgainAndGetSolutionGlobalsVariable_GlobalsVariableValueReturned()
		{
			CreateDTE();
			
			dte.Solution.Globals.set_VariableValue("test", "test-value");
			object variableValue = dte.Solution.Globals.get_VariableValue("test");
			
			Assert.AreEqual("test-value", variableValue);
		}
		
		[Test]
		public void Solution_OpenSolutionChangesAfterSolutionPropertyAccessed_SolutionReturnedForCurrentOpenSolution()
		{
			CreateDTE(@"d:\projects\first\first.sln");
			global::EnvDTE.Solution firstSolution = dte.Solution;
			
			OpenSolution(@"d:\projects\second\second.sln");
			
			string fileName = dte.Solution.FileName;
			Assert.AreEqual(@"d:\projects\second\second.sln", fileName);
		}
	}
}
