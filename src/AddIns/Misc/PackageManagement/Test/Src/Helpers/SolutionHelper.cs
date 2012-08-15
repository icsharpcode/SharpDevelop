// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class SolutionHelper
	{
		public SolutionHelper()
		{
			OpenSolution();
		}
		
		public Solution Solution;
		public FakePackageManagementProjectService FakeProjectService;
		public SD.Solution MSBuildSolution;
		public SD.ProjectSection ExtensibilityGlobalsSection;
		
		void OpenSolution()
		{
			FakeProjectService = new FakePackageManagementProjectService();
			MSBuildSolution = CreateSharpDevelopSolution();
			FakeProjectService.OpenSolution = MSBuildSolution;
			Solution = new Solution(FakeProjectService);
		}
		
		SD.Solution CreateSharpDevelopSolution()
		{
			return new SD.Solution(new SD.MockProjectChangeWatcher());
		}
		
		public SD.Solution OpenDifferentSolution()
		{
			SD.Solution solution = CreateSharpDevelopSolution();
			FakeProjectService.OpenSolution = solution;
			return solution;
		}
		
		public void CloseSolution()
		{
			FakeProjectService.OpenSolution = null;
		}
		
		public void AddProjectToSolution(string projectName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(projectName);
			FakeProjectService.AddFakeProject(project);
		}
		
		public void AddExtensibilityGlobalsSection()
		{
			ExtensibilityGlobalsSection = new SD.ProjectSection("ExtensibilityGlobals", "postSolution");
			MSBuildSolution.Sections.Add(ExtensibilityGlobalsSection);
		}
		
		public void AddVariableToExtensibilityGlobals(string name, string value)
		{
			var solutionItem = new SD.SolutionItem(name, value);
			ExtensibilityGlobalsSection.Items.Add(solutionItem);
		}
	}
}
