// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class PropertyTests
	{
		Properties properties;
		TestableDTEProject project;
		TestableProject msbuildProject;
		
		void CreateProperties()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			properties = new Properties(project);
		}
		
		[Test]
		public void Value_GetPostBuildEvent_ReturnsProjectsPostBuildEvent()
		{
			CreateProperties();
			msbuildProject.SetProperty("PostBuildEvent", "Test");
			var postBuildEventProperty = properties.Item("PostBuildEvent").Value;
			
			Assert.AreEqual("Test", postBuildEventProperty);
		}
		
		[Test]
		public void Value_GetPostBuildEvent_ReturnsUnevaluatedPostBuildEvent()
		{
			CreateProperties();
			msbuildProject.SetProperty("PostBuildEvent", "$(SolutionDir)", false);
			var postBuildEventProperty = properties.Item("PostBuildEvent").Value;
			
			Assert.AreEqual("$(SolutionDir)", postBuildEventProperty);
		}
		
		[Test]
		public void Value_GetNullProperty_ReturnsEmptyString()
		{
			CreateProperties();
			var property = properties.Item("TestTestTest").Value;
			
			Assert.AreEqual(String.Empty, property);
		}
		
		[Test]
		public void Value_SetPostBuildEvent_UpdatesProjectsPostBuildEvent()
		{
			CreateProperties();
			properties.Item("PostBuildEvent").Value = "Test";
			
			string postBuildEventProperty = msbuildProject.GetEvaluatedProperty("PostBuildEvent");
			
			Assert.AreEqual("Test", postBuildEventProperty);
		}
		
		[Test]
		public void Value_SetPostBuildEvent_DoesNotEscapeText()
		{
			CreateProperties();
			properties.Item("PostBuildEvent").Value = "$(SolutionDir)";
			
			string postBuildEventProperty = msbuildProject.GetUnevalatedProperty("PostBuildEvent");
			
			Assert.AreEqual("$(SolutionDir)", postBuildEventProperty);
		}
		
		[Test]
		public void ItemValue_SetPostBuildEvent_MSBuildProjectIsSaved()
		{
			CreateProperties();
			project.Properties.Item("PostBuildEvent").Value = "test";
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
	}
}
