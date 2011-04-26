// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectTests
	{
		Project project;
		TestableProject msbuildProject;
		
		void CreateProject()
		{
			msbuildProject = ProjectHelper.CreateTestProject();
			project = new Project(msbuildProject);
		}
		
		[Test]
		public void ObjectReferencesAdd_AddGacAssemblyReference_ReferenceAddedToMSBuildProject()
		{
			CreateProject();
			project.Object.References.Add("System.Data");
			
			var reference = msbuildProject.Items[0] as ReferenceProjectItem;
			string referenceName = reference.Name;
			
			Assert.AreEqual("System.Data", referenceName);
		}
		
		[Test]
		public void PropertiesItemValue_GetPostBuildEvent_ReturnsProjectsPostBuildEvent()
		{
			CreateProject();
			msbuildProject.SetProperty("PostBuildEvent", "Test");
			var postBuildEventProperty = project.Properties.Item("PostBuildEvent").Value;
			
			Assert.AreEqual("Test", postBuildEventProperty);
		}
		
		[Test]
		public void PropertiesItemValue_SetPostBuildEvent_UpdatesProjectsPostBuildEvent()
		{
			CreateProject();
			project.Properties.Item("PostBuildEvent").Value = "Test";
			
			string postBuildEventProperty = msbuildProject.GetEvaluatedProperty("PostBuildEvent");
			
			Assert.AreEqual("Test", postBuildEventProperty);
		}
		
		[Test]
		public void PropertiesItemValue_SetPostBuildEvent_DoesNotEscapeText()
		{
			CreateProject();
			project.Properties.Item("PostBuildEvent").Value = "$(SolutionDir)";
			
			string postBuildEventProperty = msbuildProject.GetUnevalatedProperty("PostBuildEvent");
			
			Assert.AreEqual("$(SolutionDir)", postBuildEventProperty);
		}
	}
}
