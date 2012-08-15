// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class SolutionTests
	{
		SolutionHelper solutionHelper;
		Solution solution;
		
		void CreateSolution()
		{
			solutionHelper = new SolutionHelper();
			solution = solutionHelper.Solution;
		}
		
		SD.Solution OpenDifferentSolution()
		{
			return solutionHelper.OpenDifferentSolution();
		}
		
		void NoOpenSolution()
		{
			solutionHelper.CloseSolution();
		}
		
		void AddProjectToSolution(string projectName)
		{
			solutionHelper.AddProjectToSolution(projectName);
		}
		
		[Test]
		public void IsOpen_NoOpenSolution_ReturnsFalse()
		{
			CreateSolution();
			NoOpenSolution();
			
			bool open = solution.IsOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void IsOpen_SolutionOpenInSharpDevelop_ReturnsTrue()
		{
			CreateSolution();
			
			bool open = solution.IsOpen;
			
			Assert.IsTrue(open);
		}

		[Test]
		public void IsOpen_DifferentSolutionOpenInSharpDevelop_ReturnsFalse()
		{
			CreateSolution();
			OpenDifferentSolution();
			
			bool open = solution.IsOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void Projects_SolutionHasNoProjects_NoProjectsInCollection()
		{
			CreateSolution();
			
			int count = solution.Projects.ToList().Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void Projects_SolutionHasOnProject_OneProjectInCollection()
		{
			CreateSolution();
			AddProjectToSolution("MyProject");
			
			Project project = solution.Projects.First();
			
			Assert.AreEqual("MyProject", project.Name);
		}
	}
}
