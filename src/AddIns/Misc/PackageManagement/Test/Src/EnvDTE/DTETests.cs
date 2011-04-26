// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using SD = ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class DTETests
	{
		DTE dte;
		FakePackageManagementProjectService fakeProjectService;
		
		void CreateDTE()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			fakeProjectService.OpenSolution = new SD.Solution();
			dte = new DTE(fakeProjectService);
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
		public void Solution_NoOpenSolution_ReturnsNull()
		{
			CreateDTE();
			fakeProjectService.OpenSolution = null;
			
			var solution = dte.Solution;
			
			Assert.IsNull(solution);
		}
	}
}
