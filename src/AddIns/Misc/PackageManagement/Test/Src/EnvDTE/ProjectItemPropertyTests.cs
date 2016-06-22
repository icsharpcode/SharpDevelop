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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;
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
		ICSharpCode.PackageManagement.EnvDTE.Properties properties;
		
		void CreateProjectItemProperties()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			msbuildFileProjectItem = new SD.FileProjectItem(msbuildProject, SD.ItemType.Compile);
			projectItem = new ProjectItem(project, msbuildFileProjectItem);
			properties = (ICSharpCode.PackageManagement.EnvDTE.Properties)projectItem.Properties;
			
			IWorkbench workbench = MockRepository.GenerateStub<IWorkbench>();
			ICSharpCode.SharpDevelop.SD.Services.AddService(typeof(IWorkbench), workbench);
		}
		
		void AssertContainsProperty(string propertyName, IEnumerable items)
		{
			var itemsList = new List<Property>();
			foreach (Property property in items) {
				itemsList.Add(property);
			}
			var matchedProperty = itemsList.Find(p => p.Name == propertyName);
			
			Assert.AreEqual(propertyName, matchedProperty.Name);
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
		
		[Test]
		public void Value_GetCustomTool_ReturnsProjectItemCustomTool()
		{
			CreateProjectItemProperties();
			msbuildFileProjectItem.CustomTool = "Test";
			
			string customTool = properties.Item("CustomTool").Value as string;
			
			Assert.AreEqual("Test", customTool);
		}
		
		[Test]
		public void Value_SetCustomTool_UpdatesProjectItemCustomTool()
		{
			CreateProjectItemProperties();
			
			properties.Item("CustomTool").Value = "MyCustomTool";
			
			var customTool = msbuildFileProjectItem.CustomTool;
			
			Assert.AreEqual("MyCustomTool", customTool);
		}
		
		[Test]
		public void GetEnumerator_FindCopyToOutputDirectoryPropertyInAllProperties_ReturnsPropertyWithCopyToOutputDirectoryName()
		{
			CreateProjectItemProperties();
			
			AssertContainsProperty("CopyToOutputDirectory", properties);
		}
		
		[Test]
		public void GetEnumerator_FindCustomToolInAllProperties_ReturnsCustomToolProperty()
		{
			CreateProjectItemProperties();
			
			AssertContainsProperty("CustomTool", properties);
		}
		
		[Test]
		public void Value_GetFullPath_ReturnsProjectItemFullFileName()
		{
			CreateProjectItemProperties();
			msbuildFileProjectItem.FileName = ICSharpCode.Core.FileName.Create(@"d:\projects\test.cs");
			
			string path = properties.Item("FullPath").Value as string;
			
			Assert.AreEqual(@"d:\projects\test.cs", path);
		}
		
		[Test]
		public void GetEnumerator_FindFullPathPropertyInAllProperties_ReturnsPropertyWithFullPathName()
		{
			CreateProjectItemProperties();
			
			AssertContainsProperty("FullPath", properties);
		}
		
		[Test]
		public void Value_GetLocalPath_ReturnsProjectItemFullFileName()
		{
			CreateProjectItemProperties();
			msbuildFileProjectItem.FileName = FileName.Create(@"d:\projects\test.cs");
			
			string path = properties.Item("LocalPath").Value as string;
			
			Assert.AreEqual(@"d:\projects\test.cs", path);
		}
		
		[Test]
		public void GetEnumerator_FindLocalPathPropertyInAllProperties_ReturnsPropertyWithFullPathName()
		{
			CreateProjectItemProperties();
			
			AssertContainsProperty("LocalPath", properties);
		}
	}
}
