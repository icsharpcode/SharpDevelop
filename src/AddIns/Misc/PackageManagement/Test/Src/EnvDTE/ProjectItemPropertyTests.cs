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
	public class ProjectItemPropertyTests
	{
		ProjectItem projectItem;
		SD.FileProjectItem msbuildFileProjectItem;
		TestableDTEProject project;
		TestableProject msbuildProject;
		Properties properties;
		
		void CreateProjectItemProperties()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			msbuildFileProjectItem = new SD.FileProjectItem(msbuildProject, SD.ItemType.Compile);
			projectItem = new ProjectItem(project, msbuildFileProjectItem);
			properties = projectItem.Properties;
		}
		
		[Test]
		public void Value_GetCopyToOutputDirectoryPropertyValue_ReturnsCopyToOutputDirectoryValueFromMSBuildProject()
		{
			CreateProjectItemProperties();
			msbuildFileProjectItem.CopyToOutputDirectory = SD.CopyToOutputDirectory.PreserveNewest;
			
			var propertyObject = properties.Item("CopyToOutputDirectory").Value;
			var propertyValue = (UInt32)propertyObject;
			var propertyValueAsEnum = (SD.CopyToOutputDirectory)propertyValue;
			
			Assert.AreEqual(SD.CopyToOutputDirectory.PreserveNewest, propertyValueAsEnum);
		}
		
		[Test]
		public void Name_ProjectItemNameIsTest_ReturnsTest()
		{
			CreateProjectItemProperties();
			var property = new ProjectItemProperty(projectItem, "Test");
			
			string name = property.Name;
			
			Assert.AreEqual("Test", name);
		}
		
		[Test]
		public void Value_SetCopyToOutputDirectoryPropertyValueToAlways_SetsCopyToOutputDirectoryValueInMSBuildProject()
		{
			CreateProjectItemProperties();
			
			UInt32 copyToOutputDirectoryAlways = 1;
			properties.Item("CopyToOutputDirectory").Value = copyToOutputDirectoryAlways;
			
			var copyToOutputDirectory = msbuildFileProjectItem.CopyToOutputDirectory;
			
			Assert.AreEqual(SD.CopyToOutputDirectory.Always, copyToOutputDirectory);
		}
		
		[Test]
		public void Value_SetCopyToOutputDirectoryPropertyValueToAlways_ProjectIsSaved()
		{
			CreateProjectItemProperties();
			
			UInt32 copyToOutputDirectoryAlways = 1;
			properties.Item("CopyToOutputDirectory").Value = copyToOutputDirectoryAlways;
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
	}
}
