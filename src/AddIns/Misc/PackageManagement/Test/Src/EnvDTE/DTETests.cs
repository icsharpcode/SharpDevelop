// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class DTETests
	{
		DTE dte;
		FakePackageManagementProjectService fakeProjectService;
		FakeFileService fakeFileService;
		
		void CreateDTE()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			fakeProjectService.OpenSolution = new SD.Solution(new SD.MockProjectChangeWatcher());
			fakeFileService = new FakeFileService(null);
			dte = new DTE(fakeProjectService, fakeFileService);
		}
		
		void NoOpenSolution()
		{
			fakeProjectService.OpenSolution = null;
		}
		
		TestableProject AddProjectToSolution(string projectName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(projectName);
			fakeProjectService.AddFakeProject(project);
			return project;
		}
		
		[Test]
		public void SolutionFullName_SolutionIsOpen_ReturnsSolutionFileName()
		{
			CreateDTE();
			string fileName = @"d:\projects\myproject\myproject.sln";
			fakeProjectService.OpenSolution.FileName = fileName;
			
			string fullName = dte.Solution.FullName;
			
			Assert.AreEqual(fileName, fullName);
		}
		
		[Test]
		public void SolutionFileName_SolutionIsOpen_ReturnsSolutionFileName()
		{
			CreateDTE();
			string expectedFileName = @"d:\projects\myproject\myproject.sln";
			fakeProjectService.OpenSolution.FileName = expectedFileName;
			
			string fileName = dte.Solution.FileName;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void Solution_NoOpenSolution_ReturnsNull()
		{
			CreateDTE();
			NoOpenSolution();
			
			Solution solution = dte.Solution;
			
			Assert.IsNull(solution);
		}
		
		[Test]
		public void Properties_LookForFontsAndColorsCategoryInTextEditorPage_ReturnsProperties()
		{
			CreateDTE();
			
			Properties properties = dte.Properties("FontsAndColors", "TextEditor");
			
			Assert.IsNotNull(properties);
		}
		
		[Test]
		public void Properties_LookForUnknownCategoryAndPage_ReturnsNull()
		{
			CreateDTE();
			
			Properties properties = dte.Properties("UnknownCategory", "UnknownPage");
			
			Assert.IsNull(properties);
		}
		
		[Test]
		public void Properties_LookForFontsAndColorsCategoryInTextEditorPageInUpperCase_ReturnsTextEditorFontsAndColorsProperties()
		{
			CreateDTE();
			
			Properties properties = dte.Properties("FONTSANDCOLORS", "TEXTEDITOR");
			
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
	}
}
