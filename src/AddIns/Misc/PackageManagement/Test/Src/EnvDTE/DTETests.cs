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
			fakeProjectService.OpenSolution = new SD.Solution();
			fakeFileService = new FakeFileService(null);
			dte = new DTE(fakeProjectService, fakeFileService);
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
			fakeProjectService.OpenSolution = null;
			
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
	}
}
