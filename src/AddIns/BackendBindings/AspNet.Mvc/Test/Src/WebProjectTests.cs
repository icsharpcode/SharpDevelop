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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class WebProjectTests : MvcTestsBase
	{
		MSBuildBasedProject msbuildProject;
		WebProject webProject;
		
		void CreateMSBuildProject()
		{
			msbuildProject = MSBuildProjectHelper.CreateCSharpProject();
		}
		
		void CreateWebProject()
		{
			CreateMSBuildProject();
			CreateWebProject(msbuildProject);
		}
		
		void CreateWebProject(MSBuildBasedProject msbuildProject)
		{
			webProject = new WebProject(msbuildProject);
		}
		
		void CreateWebProjectFromMSBuildProjectWithWebProjectProperties(WebProjectProperties properties)
		{
			CreateWebProject();
			webProject.UpdateWebProjectProperties(properties);
			CreateWebProject(msbuildProject);
		}
		
		void CreateWebProjectFromMSBuildProjectWithWebProjectProperties()
		{
			CreateWebProjectFromMSBuildProjectWithWebProjectProperties(new WebProjectProperties());
		}
		
		string GetMSBuildProjectExtensions()
		{
			var fileContentsBuilder = new StringBuilder();
			var stringWriter = new StringWriter(fileContentsBuilder);
			lock (msbuildProject.SyncRoot)
				msbuildProject.MSBuildProjectFile.Save(stringWriter);
			
			return GetProjectExtensions(fileContentsBuilder.ToString());
		}
		
		string GetProjectExtensions(string text)
		{
			return Regex.Match(
				text,
				"<ProjectExtensions>.*?</ProjectExtensions>",
				RegexOptions.Singleline).Value;
		}
		
		void SetUseIISExpressInMSBuildProjectToTrue()
		{
			msbuildProject.SetProperty("UseIISExpress", "True");
		}
		
		void AddVisualStudioExtensionWithChildElement(string childElementName)
		{
			msbuildProject.SaveProjectExtensions(VisualStudioProjectExtension.ProjectExtensionName, new XElement(childElementName));
		}
		
		[Test]
		public void Regex_GetMSBuildProjectExtensions_ReturnsProjectExtensionsXml()
		{
			string text =
				"<Project>\r\n" +
				"  <ProjectExtensions>\r\n" +
				"    <foo>\r\n" +
				"    </foo>\r\n" +
				"  <ProjectExtensions>\r\n" +
				"</Project>";
			
			string result = GetProjectExtensions(text);
			
			string expectedResult =
				"  <ProjectExtensions>\r\n" +
				"    <foo>\r\n" +
				"    </foo>\r\n" +
				"  <ProjectExtensions>";
			
			Assert.AreNotEqual(expectedResult, result);
		}
		
		[Test]
		public void Regex_GetMSBuildProjectExtensionsFromFullProjectXml_ReturnsProjectExtensionsXml()
		{
			string text =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Project ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <PropertyGroup>
    <ProjectGuid>{20D05950-C385-4093-8EAF-ADF36EAA54BE}</ProjectGuid>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">x86</Platform>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Platform)' == 'x86' "">
    <PlatformTarget>x86</PlatformTarget>
  </PropertyGroup>
  <ProjectExtensions>
    <VisualStudio>
      <FlavorProperties GUID=""{349C5851-65DF-11DA-9384-00065B846F21}"">  <WebProjectProperties>    <UseIIS>True</UseIIS>    <AutoAssignPort>False</AutoAssignPort>    <DevelopmentServerPort>7777</DevelopmentServerPort>    <DevelopmentServerVPath>/</DevelopmentServerVPath>    <IISUrl>http://localhost:7777/test</IISUrl>    <NTLMAuthentication>False</NTLMAuthentication>    <UseCustomServer>False</UseCustomServer>    <CustomServerUrl>    </CustomServerUrl>    <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>  </WebProjectProperties></FlavorProperties>
    </VisualStudio>
  </ProjectExtensions>
</Project>";
			
			string result = GetProjectExtensions(text);
			
			string expectedResult =
@"<ProjectExtensions>
    <VisualStudio>
      <FlavorProperties GUID=""{349C5851-65DF-11DA-9384-00065B846F21}"">  <WebProjectProperties>    <UseIIS>True</UseIIS>    <AutoAssignPort>False</AutoAssignPort>    <DevelopmentServerPort>7777</DevelopmentServerPort>    <DevelopmentServerVPath>/</DevelopmentServerVPath>    <IISUrl>http://localhost:7777/test</IISUrl>    <NTLMAuthentication>False</NTLMAuthentication>    <UseCustomServer>False</UseCustomServer>    <CustomServerUrl>    </CustomServerUrl>    <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>  </WebProjectProperties></FlavorProperties>
    </VisualStudio>
  </ProjectExtensions>";
			
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void GetWebProjectProperties_NoExistingWebProjectPropertiesInMSBuildProject_ReturnsWebProjectPropertiesWithDefaultValues()
		{
			CreateWebProject();
			WebProjectProperties properties = webProject.GetWebProjectProperties();
			
			Assert.AreEqual(WebProject.DefaultProperties, properties);
		}
		
		[Test]
		public void UpdateWebProjectProperties_SaveServerSettingsInUserFileIsFalse_SavesPropertiesToMainProjectFile()
		{
			CreateWebProject();
			WebProjectProperties properties = webProject.GetWebProjectProperties();
			properties.SaveServerSettingsInUserFile = false;
			properties.DevelopmentServerPort = 7777;
			properties.DevelopmentServerVPath = "/";
			properties.IISUrl = "http://localhost:7777/test";
			properties.UseIIS = true;
			
			webProject.UpdateWebProjectProperties(properties);
			
			string msbuildProjectFileContents = GetMSBuildProjectExtensions();
			
			string expectedMSBuildProjectFileContents = 
@"<ProjectExtensions>
    <VisualStudio>
      <FlavorProperties GUID=""{349C5851-65DF-11DA-9384-00065B846F21}"">
        <WebProjectProperties>
          <UseIIS>True</UseIIS>
          <AutoAssignPort>False</AutoAssignPort>
          <DevelopmentServerPort>7777</DevelopmentServerPort>
          <DevelopmentServerVPath>/</DevelopmentServerVPath>
          <IISUrl>http://localhost:7777/test</IISUrl>
          <NTLMAuthentication>False</NTLMAuthentication>
          <UseCustomServer>False</UseCustomServer>
          <CustomServerUrl></CustomServerUrl>
          <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>
        </WebProjectProperties>
      </FlavorProperties>
    </VisualStudio>
  </ProjectExtensions>";
			
			Assert.AreEqual(expectedMSBuildProjectFileContents, msbuildProjectFileContents);
		}
		
		[Test]
		public void GetWebProjectProperties_MSBuildHasWebProjectProperties_ReadsWebProjectPropertiesFromMSBuildProject()
		{
			var expectedProperties = new WebProjectProperties
			{
				DevelopmentServerPort = 8989,
				IISUrl = "http://localhost:8989/test"
			};
			CreateWebProjectFromMSBuildProjectWithWebProjectProperties(expectedProperties);
			
			WebProjectProperties properties = webProject.GetWebProjectProperties();
			
			Assert.AreEqual(expectedProperties, properties);
		}
		
		[Test]
		public void GetWebProjectProperties_MSBuildHasUseIISExpressPropertySetToTrue_UseIISExpressPropertyIsTrue()
		{
			CreateWebProjectFromMSBuildProjectWithWebProjectProperties();
			SetUseIISExpressInMSBuildProjectToTrue();
			
			var properties = webProject.GetWebProjectProperties();
			
			var expectedProperties = new WebProjectProperties { UseIISExpress = true };
			
			Assert.AreEqual(expectedProperties, properties);
		}
		
		[Test]
		public void UpdateWebProjectProperties_UseIISExpressIsTrue_MSBuildProjectIISExpressPropertySetToTrue()
		{
			CreateWebProject();
			var properties = new WebProjectProperties { UseIISExpress = true };
			
			webProject.UpdateWebProjectProperties(properties);
			
			string value = msbuildProject.GetEvaluatedProperty("UseIISExpress");
			
			Assert.AreEqual("True", value);
		}
		
		[Test]
		public void UseIISExpress_MSBuildUseIISExpressIsTrue_ReturnsTrue()
		{
			CreateWebProject();
			msbuildProject.SetProperty("UseIISExpress", "True");
			
			bool result = webProject.UseIISExpress;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void UseIISExpress_MSBuildHasNoUseIISExpressProperty_ReturnsFalse()
		{
			CreateWebProject();
			
			bool result = webProject.UseIISExpress;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasWebProjectProperties_MSBuildHasNoWebProjectProperties_ReturnsFalse()
		{
			CreateWebProject();
			
			bool result = webProject.HasWebProjectProperties();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasWebProjectProperties_MSBuildHasWebProjectProperties_ReturnsTrue()
		{
			CreateWebProjectFromMSBuildProjectWithWebProjectProperties();
			
			bool result = webProject.HasWebProjectProperties();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasWebProjectProperties_MSBuildProjectDoesNotHaveWebProjectPropertiesButHasVisualStudioExtension_ReturnsFalse()
		{
			CreateMSBuildProject();
			AddVisualStudioExtensionWithChildElement("Test");
			CreateWebProject(msbuildProject); 
			
			bool contains = webProject.HasWebProjectProperties();
			
			Assert.IsFalse(contains);
		}
		
		[Test]
		public void Name_MSBuildProjectNameIsTest_ReturnsTest()
		{
			CreateMSBuildProject();
			msbuildProject.Name = "Test";
			CreateWebProject(msbuildProject);
			
			string name = webProject.Name;
			
			Assert.AreEqual("Test", name);
		}
		
		[Test]
		public void Directory_MSBuildProjectDirectoryIsSet_ReturnsMSBuildProjectDirectory()
		{
			CreateMSBuildProject();
			msbuildProject.FileName = new FileName(@"c:\projects\Test\test.csproj");
			CreateWebProject(msbuildProject);
			
			string directory = webProject.Directory;
			
			Assert.AreEqual(@"c:\projects\Test", directory);
		}
	}
}
