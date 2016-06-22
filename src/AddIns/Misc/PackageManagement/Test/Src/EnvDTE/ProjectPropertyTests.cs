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
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ProjectPropertyTests
	{
		ICSharpCode.PackageManagement.EnvDTE.Properties properties;
		TestableDTEProject project;
		TestableProject msbuildProject;
		
		void CreateProperties()
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			var factory = new ProjectPropertyFactory(project);
			properties = new ICSharpCode.PackageManagement.EnvDTE.Properties(factory);
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
		public void Value_SetPostBuildEvent_MSBuildProjectIsSaved()
		{
			CreateProperties();
			properties.Item("PostBuildEvent").Value = "test";
			
			bool saved = msbuildProject.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void Value_GetTargetFrameworkMoniker_ReturnsNet40ClientProfile()
		{
			CreateProperties();
			msbuildProject.SetProperty("TargetFrameworkVersion", "4.0");
			msbuildProject.SetProperty("TargetFrameworkProfile", "Client");
			
			string targetFrameworkMoniker = properties.Item("TargetFrameworkMoniker").Value as string;
			
			string expectedTargetFrameworkMoniker = ".NETFramework,Version=v4.0,Profile=Client";
			
			Assert.AreEqual(expectedTargetFrameworkMoniker, targetFrameworkMoniker);
		}
		
		[Test]
		public void Value_GetTargetFrameworkMonikerUsingIncorrectCaseAndFrameworkIdentifierIsSilverlight_ReturnsNet35Silverlight()
		{
			CreateProperties();
			msbuildProject.SetProperty("TargetFrameworkIdentifier", "Silverlight");
			msbuildProject.SetProperty("TargetFrameworkVersion", "3.5");
			msbuildProject.SetProperty("TargetFrameworkProfile", "Full");
			
			string targetFrameworkMoniker = properties.Item("targetframeworkmoniker").Value as string;
			
			string expectedTargetFrameworkMoniker = "Silverlight,Version=v3.5,Profile=Full";
			
			Assert.AreEqual(expectedTargetFrameworkMoniker, targetFrameworkMoniker);
		}
		
		[Test]
		public void GetEnumerator_TargetFrameworkVersionSetTo40_TargetFrameworkVersionPropertyReturned()
		{
			CreateProperties();
			msbuildProject.SetProperty("TargetFrameworkVersion", "4.0");
			
			global::EnvDTE.Property targetFrameworkVersionProperty = PropertiesHelper.FindProperty(project.Properties, "TargetFrameworkVersion");
			string targetFrameworkVersion = targetFrameworkVersionProperty.Value as string;
			
			Assert.AreEqual("4.0", targetFrameworkVersion);
		}
		
		[Test]
		public void Value_GetFullPathProperty_ReturnsProjectDirectory()
		{
			CreateProperties();
			msbuildProject.FileName = new ICSharpCode.Core.FileName(@"d:\projects\MyProject\MyProject.csproj");
			
			global::EnvDTE.Property fullPathProperty = project.Properties.Item("FullPath");
			string fullPath = fullPathProperty.Value as string;
			
			string expectedFullPath = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedFullPath, fullPath);
		}
		
		[Test]
		public void Value_GetFullPathPropertyWithUpperCaseCharacters_ReturnsProjectDirectory()
		{
			CreateProperties();
			msbuildProject.FileName = new ICSharpCode.Core.FileName(@"d:\projects\MyProject\MyProject.csproj");
			
			global::EnvDTE.Property fullPathProperty = project.Properties.Item("FULLPATH");
			string fullPath = fullPathProperty.Value as string;
			
			string expectedFullPath = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedFullPath, fullPath);
		}
		
		[Test]
		public void Value_GetOutputFileNameProperty_ReturnsOutputAssemblyFileNameWithoutPath()
		{
			CreateProperties();
			msbuildProject.AssemblyName = "MyProject";
			msbuildProject.SetProperty("OutputType", "Exe");
			
			string fileName = (string)project.Properties.Item("OutputFileName").Value;
			
			Assert.AreEqual(@"MyProject.exe", fileName);
		}
		
		[Test]
		public void Properties_GetOutputFileNamePropertyInLowerCase_ReturnsOutputAssemblyFileNameWithoutPath()
		{
			CreateProperties();
			msbuildProject.AssemblyName = "MyProject";
			msbuildProject.SetProperty("OutputType", "Library");
			
			string fileName = (string)project.Properties.Item("outputfilename").Value;
			
			Assert.AreEqual(@"MyProject.dll", fileName);
		}
		
		[Test]
		public void Properties_GetOutputFileNamePropertyWhenOutputTypeIsMissing_ReturnsOutputAssemblyFileNameWithExeFileExtension()
		{
			CreateProperties();
			msbuildProject.AssemblyName = "MyProject";
			
			string fileName = (string)project.Properties.Item("outputfilename").Value;
			
			Assert.AreEqual(@"MyProject.exe", fileName);
		}
		
		[Test]
		public void Properties_GetOutputFileNamePropertyWhenOutputTypeValueIsInLowerCase_ReturnsOutputAssemblyFileNameWithoutPath()
		{
			CreateProperties();
			msbuildProject.AssemblyName = "MyProject";
			msbuildProject.SetProperty("OutputType", "winexe");
			
			string fileName = (string)project.Properties.Item("OutputFileName").Value;
			
			Assert.AreEqual(@"MyProject.exe", fileName);
		}
		
		[Test]
		public void Properties_GetOutputFileNamePropertyWhenOutputTypeValueIsInvalid_ReturnsOutputAssemblyFileNameWithExeFileExtension()
		{
			CreateProperties();
			msbuildProject.AssemblyName = "MyProject";
			msbuildProject.SetProperty("OutputType", "invalid");
			
			string fileName = (string)project.Properties.Item("OutputFileName").Value;
			
			Assert.AreEqual("MyProject.exe", fileName);
		}
		
		[Test]
		public void Properties_GetDefaultNamespaceProperty_ReturnsRootNamespaceForProject()
		{
			CreateProperties();
			msbuildProject.RootNamespace = "MyProjectRootNamespace";
			
			string defaultNamespace = (string)project.Properties.Item("DefaultNamespace").Value;
			
			Assert.AreEqual("MyProjectRootNamespace", defaultNamespace);
		}
		
		[Test]
		public void Properties_GetDefaultNamespacePropertyUsingUppercaseName_ReturnsRootNamespaceForProject()
		{
			CreateProperties();
			msbuildProject.RootNamespace = "MyProjectRootNamespace";
			
			string defaultNamespace = (string)project.Properties.Item("DEFAULTNAMESPACE").Value;
			
			Assert.AreEqual("MyProjectRootNamespace", defaultNamespace);
		}
		
		[Test]
		public void Value_GetLocalPathProperty_ReturnsProjectDirectory()
		{
			CreateProperties();
			msbuildProject.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			
			global::EnvDTE.Property localPathProperty = project.Properties.Item("LocalPath");
			string localPath = localPathProperty.Value as string;
			
			string expectedLocalPath = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedLocalPath, localPath);
		}
		
		[Test]
		public void GetEnumerator_LocalPathProperty_ExistsInEnumeratedProperties()
		{
			CreateProperties();
			msbuildProject.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			
			var enumerable = project.Properties as IEnumerable;
			
			AssertContainsProperty("LocalPath", enumerable);
		}
		
		[Test]
		public void Value_GetLocalPathPropertyWithUpperCaseCharacters_ReturnsProjectDirectory()
		{
			CreateProperties();
			msbuildProject.FileName = FileName.Create(@"d:\projects\MyProject\MyProject.csproj");
			
			global::EnvDTE.Property fullPathProperty = project.Properties.Item("LOCALPATH");
			string fullPath = fullPathProperty.Value as string;
			
			string expectedFullPath = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedFullPath, fullPath);
		}
	}
}
