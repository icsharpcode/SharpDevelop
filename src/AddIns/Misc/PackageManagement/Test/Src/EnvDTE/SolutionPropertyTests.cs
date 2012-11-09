// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using SD = ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class SolutionPropertyTests
	{
		Properties properties;
		Solution solution;
		SD.Solution msbuildSolution;
		SolutionHelper solutionHelper;
		
		void CreateProperties()
		{
			solutionHelper = new SolutionHelper();
			solution = solutionHelper.Solution;
			msbuildSolution = solutionHelper.MSBuildSolution;
			properties = (Properties)solution.Properties;
		}
		
		void AddStartupProject(string name, string fileName)
		{
			TestableProject project = solutionHelper.AddProjectToSolutionWithFileName(name, fileName);
			solutionHelper.SetStartupProject(project);
		}
		
		[Test]
		public void Value_GetPathProperty_ReturnsSolutionFileName()
		{
			CreateProperties();
			msbuildSolution.FileName = @"d:\projects\MyProject\MySolution.sln";
			
			global::EnvDTE.Property property = properties.Item("Path");
			object path = property.Value;
			
			Assert.AreEqual(@"d:\projects\MyProject\MySolution.sln", path);
		}
		
		[Test]
		public void GetEnumerator_GetPathProperty_ReturnsSolutionFileName()
		{
			CreateProperties();
			msbuildSolution.FileName = @"d:\projects\MyProject\MySolution.sln";
			
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, "Path");
			object path = property.Value;
			
			Assert.AreEqual(@"d:\projects\MyProject\MySolution.sln", path);
		}
		
		[Test]
		public void GetEnumerator_GetStartupProjectPropertyWhenSolutionHasOneStartableProject_ReturnsStartupProjectName()
		{
			CreateProperties();
			msbuildSolution.FileName = @"d:\projects\MyProject\MySolution.sln";
			AddStartupProject("MyProject", @"d:\projects\MyProject\MyProject.csproj");
			
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, "StartupProject");
			object projectName = property.Value;
			
			Assert.AreEqual("MyProject", projectName);
		}
		
		[Test]
		public void GetEnumerator_GetStartupProjectPropertyWhenSolutionHasNoProjects_ReturnsPropertyWithEmptyStringAsValue()
		{
			CreateProperties();
			msbuildSolution.FileName = @"d:\projects\MyProject\MySolution.sln";
			
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, "StartupProject");
			object projectName = property.Value;
			
			Assert.AreEqual(String.Empty, projectName);
		}
	}
}
