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
		SD.ISolution msbuildSolution;
		SolutionHelper solutionHelper;
		
		void CreateProperties(string fileName)
		{
			solutionHelper = new SolutionHelper(fileName);
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
			CreateProperties(@"d:\projects\MyProject\MySolution.sln");
			
			global::EnvDTE.Property property = properties.Item("Path");
			object path = property.Value;
			
			Assert.AreEqual(@"d:\projects\MyProject\MySolution.sln", path);
		}
		
		[Test]
		public void GetEnumerator_GetPathProperty_ReturnsSolutionFileName()
		{
			CreateProperties(@"d:\projects\MyProject\MySolution.sln");
			
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, "Path");
			object path = property.Value;
			
			Assert.AreEqual(@"d:\projects\MyProject\MySolution.sln", path);
		}
		
		[Test]
		public void GetEnumerator_GetStartupProjectPropertyWhenSolutionHasOneStartableProject_ReturnsStartupProjectName()
		{
			CreateProperties(@"d:\projects\MyProject\MySolution.sln");
			AddStartupProject("MyProject", @"d:\projects\MyProject\MyProject.csproj");
			
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, "StartupProject");
			object projectName = property.Value;
			
			Assert.AreEqual("MyProject", projectName);
		}
		
		[Test]
		public void GetEnumerator_GetStartupProjectPropertyWhenSolutionHasNoProjects_ReturnsPropertyWithEmptyStringAsValue()
		{
			CreateProperties(@"d:\projects\MyProject\MySolution.sln");
			
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, "StartupProject");
			object projectName = property.Value;
			
			Assert.AreEqual(String.Empty, projectName);
		}
	}
}
