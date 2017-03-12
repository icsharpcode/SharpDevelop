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
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class MSBuildProjectPropertiesMergerTests
	{
		MSBuildProjectPropertiesMerger propertiesMerger;
		IPackageManagementProjectService projectService;
		TestableProject sharpDevelopProject;
		Project msbuildProject;
		
		[SetUp]
		public void Init()
		{
			CreateMSBuildProject();
			CreateSharpDevelopProject();
			CreateProjectPropertiesMerger();
		}
		
		void CreateMSBuildProject()
		{
			msbuildProject = new Project();
		}
		
		void CreateSharpDevelopProject()
		{
			sharpDevelopProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateProjectPropertiesMerger()
		{
			projectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			propertiesMerger = new MSBuildProjectPropertiesMerger(msbuildProject, sharpDevelopProject, projectService);
		}
		
		void Merge()
		{
			propertiesMerger.Merge();
		}
		
		void AddPropertyToSharpDevelopProject(string name, string value)
		{
			sharpDevelopProject.SetProperty(name, value);
		}
		
		void AddPropertyToSharpDevelopProjectWithCondition(string name, string value, string condition)
		{
			lock (sharpDevelopProject.SyncRoot)
				AddPropertyWithCondition(sharpDevelopProject.MSBuildProjectFile, name, value, condition);
		}
		
		void AddPropertyWithCondition(ProjectRootElement projectRoot, string name, string value, string condition)
		{
			ProjectPropertyGroupElement groupProperty = projectRoot.CreatePropertyGroupElement();
			groupProperty.Condition = condition;
			projectRoot.AppendChild(groupProperty);
			
			ProjectPropertyElement property = projectRoot.CreatePropertyElement(name);
			groupProperty.AppendChild(property);
			property.Value = value;
			property.Condition = condition;
		}
		
		void AddPropertyToMSBuildProject(string name, string value)
		{
			msbuildProject.SetProperty(name, value);
		}
		
		void AddPropertyToMSBuildProjectWithCondition(string name, string value, string condition)
		{
			AddPropertyWithCondition(msbuildProject.Xml, name, value, condition);
		}
		
		void AssertSharpDevelopProjectContainsProperty(string propertyName, string expectedValue)
		{
			string actualValue = sharpDevelopProject.GetEvaluatedProperty(propertyName);
			
			Assert.AreEqual(expectedValue, actualValue);
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNewPropertyAdded_PropertyAddedToSharpDevelopProject()
		{
			AddPropertyToMSBuildProject("Test", "test-value");
			
			Merge();
			
			AssertSharpDevelopProjectContainsProperty("Test", "test-value");
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNewPropertyAdded_SharpDevelopProjectIsSaved()
		{
			AddPropertyToMSBuildProject("Test", "test-value");
			
			Merge();
			
			projectService.AssertWasCalled(service => service.Save(sharpDevelopProject));
		}
		
		[Test]
		public void Merge_MSBuildProjectHasTwoNewPropertiesAdded_BothPropertiesAddedToSharpDevelopProject()
		{
			AddPropertyToMSBuildProject("Test1", "test-value1");
			AddPropertyToMSBuildProject("Test2", "test-value2");
			
			Merge();
			
			AssertSharpDevelopProjectContainsProperty("Test1", "test-value1");
			AssertSharpDevelopProjectContainsProperty("Test2", "test-value2");
		}
		
		[Test]
		public void Merge_MSBuildProjectHasTwoNewPropertiesAdded_MergeResultHasBothPropertiesAdded()
		{
			AddPropertyToMSBuildProject("Test1", "test-value2");
			AddPropertyToMSBuildProject("Test2", "test-value2");
			
			Merge();
			
			var expected = new string[] { "Test1", "Test2" };
			string expectedToString = "Properties added: Test1,\r\nTest2\r\nProperties updated: ";
			CollectionAssert.AreEqual(expected, propertiesMerger.Result.PropertiesAdded);
			Assert.AreEqual(expectedToString, propertiesMerger.Result.ToString());
		}
		
		[Test]
		public void Merge_MSBuildProjectPropertyUpdated_SharpDevelopProjectPropertyIsUpdated()
		{
			AddPropertyToSharpDevelopProject("Test", "old-value");
			AddPropertyToMSBuildProject("Test", "new-value");
			
			Merge();
			
			AssertSharpDevelopProjectContainsProperty("Test", "new-value");
		}
		
		[Test]
		public void Merge_TwoMSBuildProjectsPropertiesUpdated_BothPropertiesUpdatedInSharpDevelopProject()
		{
			AddPropertyToSharpDevelopProject("Test1", "old-value");
			AddPropertyToSharpDevelopProject("Test2", "old-value");
			AddPropertyToMSBuildProject("Test1", "new-value1");
			AddPropertyToMSBuildProject("Test2", "new-value2");
			
			Merge();
			
			AssertSharpDevelopProjectContainsProperty("Test1", "new-value1");
			AssertSharpDevelopProjectContainsProperty("Test2", "new-value2");
		}
		
		[Test]
		public void Merge_TwoMSBuildProjectsPropertiesUpdated_MergeResultShowsUpdatedProperty()
		{
			AddPropertyToSharpDevelopProject("Test1", "old-value");
			AddPropertyToSharpDevelopProject("Test2", "old-value");
			AddPropertyToMSBuildProject("Test1", "new-value");
			AddPropertyToMSBuildProject("Test2", "new-value");
			
			Merge();
			
			var expected = new string[] { "Test1", "Test2" };
			string expectedToString = "Properties added: \r\nProperties updated: Test1,\r\nTest2";
			CollectionAssert.AreEqual(expected, propertiesMerger.Result.PropertiesUpdated);
			Assert.AreEqual(expectedToString, propertiesMerger.Result.ToString());
		}
		
		[Test]
		public void Merge_TwoMSBuildProjectsNotChanged_MergeResultShowNoUpdatedProperties()
		{
			AddPropertyToSharpDevelopProject("Test1", "old-value");
			AddPropertyToSharpDevelopProject("Test2", "old-value");
			AddPropertyToMSBuildProject("Test1", "old-value");
			AddPropertyToMSBuildProject("Test2", "old-value");
			
			Merge();
			
			var expected = new string[0];
			CollectionAssert.AreEqual(expected, propertiesMerger.Result.PropertiesUpdated);
			CollectionAssert.AreEqual(expected, propertiesMerger.Result.PropertiesAdded);
			Assert.IsFalse(propertiesMerger.Result.AnyPropertiesChanged());
		}
		
		[Test]
		public void SetProperty_UseDifferentCaseForMSBuildPropertyName_WhatHappens()
		{
			AddPropertyToMSBuildProject("test", "value");
			
			string propertyValue = msbuildProject.GetPropertyValue("TEST");
			
			Assert.AreEqual("value", propertyValue);
		}
		
		[Test]
		public void Merge_MSBuildProjectPropertyUpdatedButDifferentCaseUsedForName_SharpDevelopProjectPropertyIsStillUpdated()
		{
			AddPropertyToSharpDevelopProject("TEST", "old-value");
			AddPropertyToMSBuildProject("Test", "new-value");
			
			Merge();
			
			AssertSharpDevelopProjectContainsProperty("TEST", "new-value");
		}
		
		[Test]
		public void Merge_MSBuildProjectPropertyUpdatedButDifferentCaseUsedForName_MergeResultShowsPropertyHasBeenUpdated()
		{
			AddPropertyToSharpDevelopProject("TEST", "old-value");
			AddPropertyToMSBuildProject("Test", "new-value");
			
			Merge();
			
			var expected = new string[] { "Test" };
			CollectionAssert.AreEqual(expected, propertiesMerger.Result.PropertiesUpdated);
			Assert.AreEqual(0, propertiesMerger.Result.PropertiesAdded.Count());
		}
		
		/// <summary>
		/// Ignore any properties that are duplicated in the project file, such as
		/// OutputPath which exists twice due to Debug and Release configurations.
		/// </summary>
		[Test]
		public void Merge_OutputPathPropertyInDebugAndReleaseConfigurations_SharpDevelopProjectPropertiesNotChanged()
		{
			string debugConfiguration = "'$(Configuration)' == 'Debug'";
			string releaseConfiguration = "'$(Configuration)' == 'Release'";
			AddPropertyToSharpDevelopProjectWithCondition("OutputPath", @"bin\Release", releaseConfiguration);
			AddPropertyToSharpDevelopProjectWithCondition("OutputPath", @"bin\Debug", debugConfiguration);
			AddPropertyToMSBuildProjectWithCondition("OutputPath", @"bin\Release", releaseConfiguration);
			AddPropertyToMSBuildProjectWithCondition("OutputPath", @"bin\Debug", debugConfiguration);
			
			Merge();
			
			Assert.IsFalse(propertiesMerger.Result.AnyPropertiesChanged());
		}
		
		[Test]
		public void Merge_MSBuildProjectHasNewPropertyAddedWithEncodableCharacters_PropertyAddedToSharpDevelopProjectWithoutEncodingCharacters()
		{
			var propertyGroup = msbuildProject.Xml.AddPropertyGroup();
			propertyGroup.SetProperty("Test", "$(Value)");
			
			Merge();
			
			string value = sharpDevelopProject.GetUnevalatedProperty("Test");
			Assert.AreEqual("$(Value)", value);
		}
	}
}
