// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public static class ProjectHelper
	{
		public static TestableProject CreateTestProject()
		{
			Solution solution = new Solution();
			solution.FileName = @"d:\projects\Test\TestSolution.sln";
			
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.Solution = solution;
			createInfo.ProjectName = "TestProject";
			createInfo.SolutionPath = @"d:\projects\Test";
			createInfo.ProjectBasePath = @"d:\projects\Test\TestProject";
			createInfo.OutputProjectFileName = @"d:\projects\Test\TestProject\TestProject.csproj";
			
			var project = new TestableProject(createInfo);
			project.Parent = solution;
			return project;
		}
		
		public static void AddReference(MSBuildBasedProject project, string referenceName)
		{
			var referenceProjectItem = new ReferenceProjectItem(project, referenceName);
			ProjectService.AddProjectItem(project, referenceProjectItem);
		}
		
		public static void AddFile(MSBuildBasedProject project, string fileName)
		{
			var fileProjectItem = new FileProjectItem(project, ItemType.Compile);
			fileProjectItem.FileName = fileName;
			ProjectService.AddProjectItem(project, fileProjectItem);
		}
		
		public static ReferenceProjectItem GetReference(MSBuildBasedProject project, string referenceName)
		{
			foreach (ReferenceProjectItem referenceProjectItem in project.GetItemsOfType(ItemType.Reference)) {
				if (referenceProjectItem.Include == referenceName) {
					return referenceProjectItem;
				}
			}
			return null;
		}
		
		public static FileProjectItem GetFile(MSBuildBasedProject project, string fileName)
		{
			foreach (ProjectItem projectItem in project.Items) {
				FileProjectItem fileItem = projectItem as FileProjectItem;
				if (fileItem != null) {
					if (fileItem.FileName == fileName) {
						return fileItem;
					}
				}
			}
			return null;
		}
		
		public static FileProjectItem GetFileFromInclude(TestableProject project, string include)
		{
			foreach (ProjectItem projectItem in project.Items) {
				FileProjectItem fileItem = projectItem as FileProjectItem;
				if (fileItem != null) {
					if (fileItem.Include == include) {
						return fileItem;
					}
				}
			}
			return null;
		}
	}
}
