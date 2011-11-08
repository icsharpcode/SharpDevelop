// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class EmptyUnitTestsPadTests
	{
		EmptyUnitTestsPad pad;
		Solution solution;
		
		void CreatePadWithNullSolution()
		{
			CreatePad(null);
		}
		
		void CreatePad(Solution solution)
		{
			pad = new EmptyUnitTestsPad(solution);
		}
		
		void CreatePadWithSolution()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			CreatePad(solution);
		}
		
		IProject AddProjectToSolution()
		{
			var project = new MockCSharpProject(solution, "MyProject");
			solution.AddFolder(project);
			return project;
		}
		
		[Test]
		public void GetProjects_NullSolution_ReturnsNoProjects()
		{
			CreatePadWithNullSolution();
			
			int count = pad.GetProjects().Length;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void GetProjects_SolutionHasOneProject_ReturnsOneProject()
		{
			CreatePadWithSolution();
			IProject project = AddProjectToSolution();
			var expectedProjects = new IProject[] { project };
			
			IProject[] projects = pad.GetProjects();
			
			CollectionAssert.AreEqual(expectedProjects, projects);
		}
	}
}
