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
using System.Linq;
using System.Runtime.Versioning;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using Microsoft.Build.Construction;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SharpDevelopProjectSystemTests
	{
		TestableSharpDevelopProjectSystem projectSystem;
		TestableProject project;
		
		void CreateProjectSystem(MSBuildBasedProject project)
		{
			SD.Services.AddService(typeof(IWorkbench), MockRepository.GenerateStub<IWorkbench>());
			SD.Services.AddService(typeof(IMessageLoop), MockRepository.GenerateStub<IMessageLoop>());
			projectSystem = new TestableSharpDevelopProjectSystem(project);
		}
		
		void CreateTestProject()
		{
			project = ProjectHelper.CreateTestProject();
		}
		
		void CreateTestWebApplicationProject()
		{
			project = ProjectHelper.CreateTestWebApplicationProject();
		}
		
		void CreateTestWebSiteProject()
		{
			project = ProjectHelper.CreateTestWebSiteProject();
		}
		
		void CreateTestProject(string fileName)
		{
			CreateTestProject();
			project.FileName = new FileName(fileName);
		}
		
		void AddFileToProject(string fileName)
		{
			ProjectHelper.AddFile(project, fileName);
		}
		
		void AddDefaultCustomToolForFileName(string fileName, string customTool)
		{
			projectSystem.FakeProjectService.AddDefaultCustomToolForFileName(fileName, customTool);
		}
		
		void AddFile(string fileName)
		{
			projectSystem.AddFile(fileName, (Stream)null);
		}
		
		void AssertLastMSBuildChildElementHasProjectAttributeValue(string expectedAttributeValue)
		{
			ProjectImportElement import = project.GetLastMSBuildChildElement();
			Assert.AreEqual(expectedAttributeValue, import.Project);
		}
		
		void AssertLastMSBuildChildHasCondition(string expectedCondition)
		{
			ProjectImportElement import = project.GetLastMSBuildChildElement();
			Assert.AreEqual(expectedCondition, import.Condition);
		}
		
		void AssertFirstMSBuildChildElementHasProjectAttributeValue(string expectedAttributeValue)
		{
			ProjectImportElement import = project.GetFirstMSBuildChildElement();
			Assert.AreEqual(expectedAttributeValue, import.Project);
		}
		
		void AssertFirstMSBuildChildHasCondition(string expectedCondition)
		{
			ProjectImportElement import = project.GetFirstMSBuildChildElement();
			Assert.AreEqual(expectedCondition, import.Condition);
		}
		
		[Test]
		public void Root_NewInstanceCreated_ReturnsProjectDirectory()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			
			string expectedRoot = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedRoot, projectSystem.Root);
		}
		
		[Test]
		public void ProjectName_NewInstanceCreated_ReturnsProjectName()
		{
			CreateTestProject();
			project.Name = "MyProjectName";
			CreateProjectSystem(project);
			
			Assert.AreEqual("MyProjectName", projectSystem.ProjectName);
		}
		
		[Test]
		public void GetPropertyValue_PassedDefinedPropertyName_ReturnsExpectedPropertyValue()
		{
			CreateTestProject();
			string expectedPropertyValue = "Test";
			string propertyName = "TestProperty";
			project.SetProperty(propertyName, expectedPropertyValue);
			CreateProjectSystem(project);
			
			string propertyValue = projectSystem.GetPropertyValue(propertyName);
			
			Assert.AreEqual(expectedPropertyValue, propertyValue);
		}
		
		[Test]
		public void TargetFramework_TargetFrameworkVersion40DefinedInProject_ReturnsFullDotNetFramework40()
		{
			CreateTestProject();
			project.SetProperty("TargetFrameworkIdentifier", null);
			project.SetProperty("TargetFrameworkVersion", "v4.0");
			project.SetProperty("TargetFrameworkProfile", null);
			CreateProjectSystem(project);
			
			FrameworkName expectedName = new FrameworkName(".NETFramework, Version=v4.0");
			
			Assert.AreEqual(expectedName, projectSystem.TargetFramework);
		}
		
		[Test]
		public void TargetFramework_TargetFrameworkVersion35ClientProfileDefinedInProject_ReturnsClientProfileDotNetFramework35()
		{
			CreateTestProject();
			project.SetProperty("TargetFrameworkIdentifier", null);
			project.SetProperty("TargetFrameworkVersion", "v3.5");
			project.SetProperty("TargetFrameworkProfile", "Client");
			CreateProjectSystem(project);
			
			FrameworkName expectedName = new FrameworkName(".NETFramework, Profile=Client, Version=v3.5");
			
			Assert.AreEqual(expectedName, projectSystem.TargetFramework);
		}
		
		[Test]
		public void TargetFramework_TargetFrameworkVersionIsSilverlight20DefinedInProject_ReturnsSilverlight()
		{
			CreateTestProject();
			project.SetProperty("TargetFrameworkIdentifier", "Silverlight");
			project.SetProperty("TargetFrameworkVersion", "v2.0");
			project.SetProperty("TargetFrameworkProfile", null);
			CreateProjectSystem(project);
			
			FrameworkName expectedName = new FrameworkName("Silverlight, Version=v2.0");
			
			Assert.AreEqual(expectedName, projectSystem.TargetFramework);
		}
		
		[Test]
		public void IsSupportedFile_PassedCSharpFileName_ReturnsTrue()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\abc.cs";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsWebProjectAndPassedAppConfigFileName_ReturnsFalse()
		{
			CreateTestWebApplicationProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\app.config";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsWebProjectAndPassedAppConfigFileNameInUpperCase_ReturnsFalse()
		{
			CreateTestWebApplicationProject();
			CreateProjectSystem(project);
			
			string fileName = @"c:\projects\APP.CONFIG";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsWebApplicationProjectAndPassedWebConfigFileName_ReturnsTrue()
		{
			CreateTestWebApplicationProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\web.config";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsWebSiteProjectAndPassedWebConfigFileName_ReturnsTrue()
		{
			CreateTestWebSiteProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\web.config";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsCSharpProjectAndPassedWebConfigFileName_ReturnsFalse()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\web.config";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsCSharpProjectAndPassedWebConfigFileNameInUpperCase_ReturnsFalse()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\WEB.CONFIG";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsSupportedFile_ProjectIsCSharpProjectAndPassedAppConfigFileName_ReturnsTrue()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\app.config";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ReferenceExists_ProjectHasReferenceAndFullPathToAssemblyPassedToMethod_ReturnsTrue()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "MyAssembly");
			CreateProjectSystem(project);
			string fileName = @"D:\Projects\Test\MyAssembly.dll";
			
			bool result = projectSystem.ReferenceExists(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ReferenceExists_ProjectHasNoReferences_ReturnsFalse()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			string fileName = @"D:\Projects\Test\MyAssembly.dll";
			
			bool result = projectSystem.ReferenceExists(fileName);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ReferenceExists_ProjectReferenceNameHasDifferentCase_ReturnsTrue()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "myassembly");
			CreateProjectSystem(project);
			string fileName = @"D:\Projects\Test\MYASSEMBLY.dll";
			
			bool result = projectSystem.ReferenceExists(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ReferenceExists_ReferenceNamePassedIsInProjectAndIsReferenceNameWithNoFileExtension_ReturnsTrue()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "System.ComponentModel.Composition");
			CreateProjectSystem(project);
			string referenceName = "System.ComponentModel.Composition";
			
			bool result = projectSystem.ReferenceExists(referenceName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ReferenceExists_ReferenceIsInProjectAndProjectReferenceSearchedForHasExeFileExtension_ReturnsTrue()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "myassembly");
			CreateProjectSystem(project);
			string fileName = @"D:\Projects\Test\myassembly.exe";
			
			bool result = projectSystem.ReferenceExists(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ReferenceExists_ReferenceIsInProjectAndProjectReferenceSearchedForHasExeFileExtensionInUpperCase_ReturnsTrue()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "myassembly");
			CreateProjectSystem(project);
			string fileName = @"D:\Projects\Test\MYASSEMBLY.EXE";
			
			bool result = projectSystem.ReferenceExists(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void AddReference_AddReferenceToNUnitFramework_ProjectIsSavedAfterAddingReference()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			project.IsSaved = false;
			
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			projectSystem.AddReference(fileName, null);
			
			Assert.AreEqual(1, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void AddReference_AddReferenceToNUnitFramework_ReferenceAddedToProject()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			project.IsSaved = false;
			
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			projectSystem.AddReference(fileName, null);
			
			ReferenceProjectItem referenceItem = ProjectHelper.GetReference(project, "nunit.framework");
			
			ReferenceProjectItem expectedReferenceItem = new ReferenceProjectItem(project);
			expectedReferenceItem.Include = "nunit.framework";
			expectedReferenceItem.HintPath = fileName;
			
			ReferenceProjectItemAssert.AreEqual(expectedReferenceItem, referenceItem);
		}
		
		[Test]
		public void AddReference_ReferenceFileNameIsRelativePath_ReferenceAddedToProject()
		{
			CreateTestProject();
			project.FileName = new FileName(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			project.IsSaved = false;
			
			string fileName = @"packages\nunit\nunit.framework.dll";
			projectSystem.AddReference(fileName, null);
			
			ReferenceProjectItem referenceItem = ProjectHelper.GetReference(project, "nunit.framework");
			
			ReferenceProjectItem expectedReferenceItem = new ReferenceProjectItem(project);
			expectedReferenceItem.Include = "nunit.framework";
			expectedReferenceItem.HintPath = fileName;
			
			ReferenceProjectItemAssert.AreEqual(expectedReferenceItem, referenceItem);
		}
		
		[Test]
		public void AddReference_AddReferenceToNUnitFramework_AddingReferenceIsLogged()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			project.Name = "MyTestProject";
			
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			projectSystem.AddReference(fileName, null);
			
			var expectedReferenceAndProjectName = new ReferenceAndProjectName() {
				Reference = "nunit.framework",
				Project = "MyTestProject"
			};
			
			Assert.AreEqual(expectedReferenceAndProjectName, projectSystem.ReferenceAndProjectNamePassedToLogAddedReferenceToProject);
		}
		
		[Test]
		public void RemoveReference_ReferenceBeingRemovedHasFileExtension_ReferenceRemovedFromProject()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "nunit.framework");
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			projectSystem.RemoveReference(fileName);
			
			ReferenceProjectItem referenceItem = ProjectHelper.GetReference(project, "nunit.framework");
			
			Assert.IsNull(referenceItem);
		}
		
		[Test]
		public void RemoveReference_ReferenceCaseAddedToProjectDifferentToReferenceNameBeingRemoved_ReferenceRemovedFromProject()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "nunit.framework");
			CreateProjectSystem(project);
			
			string fileName = @"NUNIT.FRAMEWORK.DLL";
			projectSystem.RemoveReference(fileName);
			
			ReferenceProjectItem referenceItem = ProjectHelper.GetReference(project, "nunit.framework");
			
			Assert.IsNull(referenceItem);
		}

		[Test]
		public void RemoveReference_ProjectHasNoReference_ArgumentNullExceptionNotThrown()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"NUNIT.FRAMEWORK.DLL";
			Assert.DoesNotThrow(() => projectSystem.RemoveReference(fileName));
		}
		
		[Test]
		public void RemoveReference_ReferenceExistsInProject_ProjectIsSavedAfterReferenceRemoved()
		{
			CreateTestProject();
			ProjectHelper.AddReference(project, "nunit.framework");
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			projectSystem.RemoveReference(fileName);
			
			Assert.AreEqual(0, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void RemoveReference_ReferenceBeingRemovedHasFileExtension_ReferenceRemovalIsLogged()
		{
			CreateTestProject();
			project.Name = "MyTestProject";
			ProjectHelper.AddReference(project, "nunit.framework");
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			projectSystem.RemoveReference(fileName);
			
			var expectedReferenceAndProjectName = new ReferenceAndProjectName {
				Reference = "nunit.framework",
				Project = "MyTestProject"
			};
			
			Assert.AreEqual(expectedReferenceAndProjectName, projectSystem.ReferenceAndProjectNamePassedToLogRemovedReferenceFromProject);
		}
		
		[Test]
		public void AddFile_NewFile_AddsFileToFileSystem()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string expectedPath = @"d:\temp\abc.cs";
			Stream expectedStream = new MemoryStream();
			projectSystem.AddFile(expectedPath, expectedStream);
			
			Assert.AreEqual(expectedPath, projectSystem.PathPassedToPhysicalFileSystemAddFile);
			Assert.AreEqual(expectedStream, projectSystem.StreamPassedToPhysicalFileSystemAddFile);
		}
		
		[Test]
		public void AddFile_NewFile_AddsFileToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			AddFile(fileName);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = FileName.Create(fileName);
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_NewResxFile_AddsFileToProjectWithCorrectItemType()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.EmbeddedResource;
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.resx";
			AddFile(fileName);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.EmbeddedResource);
			expectedFileItem.FileName = FileName.Create(fileName);
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_RelativeFileNameUsed_AddsFileToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string relativeFileName = @"src\NewFile.cs";
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			AddFile(relativeFileName);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = FileName.Create(fileName);
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_RelativeFileNameWithNoPathUsed_AddsFileToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string relativeFileName = @"NewFile.cs";
			string fileName = @"d:\projects\MyProject\NewFile.cs";
			AddFile(relativeFileName);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = FileName.Create(fileName);
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_NewFile_ProjectIsSavedAfterFileAddedToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.IsSaved = false;
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			AddFile(fileName);
			
			Assert.AreEqual(1, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void AddFile_NewFileToBeAddedInBinFolder_FileIsNotAddedToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			
			string fileName = @"bin\NewFile.dll";
			AddFile(fileName);
			
			FileProjectItem fileItem = ProjectHelper.GetFileFromInclude(project, fileName);
			
			Assert.IsNull(fileItem);
		}
		
		[Test]
		public void AddFile_NewFileToBeAddedInBinFolderWithBinFolderNameInUpperCase_FileIsNotAddedToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			
			string fileName = @"BIN\NewFile.dll";
			AddFile(fileName);
			
			FileProjectItem fileItem = ProjectHelper.GetFileFromInclude(project, fileName);
			
			Assert.IsNull(fileItem);
		}
		
		[Test]
		public void AddFile_FileAlreadyExistsInProject_FileIsNotAddedToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			AddFileToProject(@"d:\projects\MyProject\src\NewFile.cs");
			
			AddFile(@"src\NewFile.cs");
			
			int projectItemsCount = project.Items.Count;
			Assert.AreEqual(1, projectItemsCount);
		}
		
		[Test]
		public void AddFile_NewFile_FileAddedToProjectIsLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			project.Name = "MyTestProject";
			CreateProjectSystem(project);
			
			AddFile(@"src\files\abc.cs");
			
			var expectedFileNameAndProjectName = new FileNameAndProjectName {
				FileName = @"src\files\abc.cs",
				ProjectName = "MyTestProject"
			};
			
			Assert.AreEqual(expectedFileNameAndProjectName, projectSystem.FileNameAndProjectNamePassedToLogAddedFileToProject);
		}
		
		[Test]
		public void AddFile_NewFileAlreadyExistsInProject_FileIsStillLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			project.Name = "MyTestProject";
			AddFileToProject(@"src\files\abc.cs");
			CreateProjectSystem(project);
			
			AddFile(@"src\files\abc.cs");
			
			var expectedFileNameAndProjectName = new FileNameAndProjectName {
				FileName = @"src\files\abc.cs",
				ProjectName = "MyTestProject"
			};
			
			Assert.AreEqual(expectedFileNameAndProjectName, projectSystem.FileNameAndProjectNamePassedToLogAddedFileToProject);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromFileSystem_CallsFileServiceRemoveFile()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			AddFileToProject(@"d:\temp\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile("test.cs");
			
			Assert.AreEqual(@"d:\temp\test.cs", projectSystem.FakeFileService.PathPassedToRemoveFile);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromFileSystem_ProjectIsSavedAfterFileRemoved()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			project.IsSaved = false;
			AddFileToProject(@"d:\temp\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile("test.cs");
			
			Assert.AreEqual(0, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromFileSystem_FileDeletionLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			AddFileToProject(@"d:\temp\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile("test.cs");
			
			Assert.AreEqual("test.cs", projectSystem.FileNamePassedToLogDeletedFile);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromFileSystem_FolderInformationNotLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			AddFileToProject(@"d:\temp\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile("test.cs");
			
			Assert.IsNull(projectSystem.FileNameAndDirectoryPassedToLogDeletedFileFromDirectory);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromSubFolder_FileDeletionLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			AddFileToProject(@"d:\temp\src\Files\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile(@"src\Files\test.cs");
			
			var expectedFileNameAndFolder = new FileNameAndDirectory() {
				FileName = "test.cs",
				Folder = @"src\Files"
			};
			
			var actualFileNameAndFolder = projectSystem.FileNameAndDirectoryPassedToLogDeletedFileFromDirectory;
			
			Assert.AreEqual(expectedFileNameAndFolder, actualFileNameAndFolder);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromSubFolder_FileDeletionWithoutFolderInformationIsNotLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			AddFileToProject(@"d:\temp\src\Files\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile(@"src\Files\test.cs");
			
			Assert.IsNull(projectSystem.FileNamePassedToLogDeletedFile);
		}

		[Test]
		public void DeleteDirectory_DeletesDirectoryFromFileSystem_CallsFileServiceRemoveDirectory()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			AddFileToProject(@"d:\temp\test\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteDirectory("test");
			
			string path = @"d:\temp\test";
			Assert.AreEqual(path, projectSystem.FakeFileService.PathPassedToRemoveDirectory);
		}
		
		[Test]
		public void DeleteDirectory_DeletesDirectoryFromFileSystem_ProjectIsSavedAfterDirectoryDeleted()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			project.IsSaved = false;
			AddFileToProject(@"d:\temp\test\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteDirectory("test");
			
			Assert.AreEqual(0, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void DeleteDirectory_DeletesDirectoryFromFileSystem_DirectoryIsLogged()
		{
			CreateTestProject(@"d:\temp\MyProject.csproj");
			project.IsSaved = false;
			AddFileToProject(@"d:\temp\test\test.cs");
			CreateProjectSystem(project);
			
			projectSystem.DeleteDirectory("test");
			
			Assert.AreEqual("test", projectSystem.DirectoryPassedToLogDeletedDirectory);
		}
		
		[Test]
		public void AddFrameworkReference_SystemXmlToBeAdded_ReferenceAddedToProject()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			projectSystem.AddFrameworkReference("System.Xml");
			
			ReferenceProjectItem referenceItem = ProjectHelper.GetReference(project, "System.Xml");
			
			ReferenceProjectItem expectedReferenceItem = new ReferenceProjectItem(project);
			expectedReferenceItem.Include = "System.Xml";
			
			ReferenceProjectItemAssert.AreEqual(expectedReferenceItem, referenceItem);
		}
		
		[Test]
		public void AddFrameworkReference_SystemXmlToBeAdded_ProjectIsSaved()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			projectSystem.AddFrameworkReference("System.Xml");
			
			bool saved = project.IsSaved;
			
			Assert.IsTrue(saved);
		}
		
		[Test]
		public void AddFrameworkReference_SystemXmlToBeAdded_AddedReferenceIsLogged()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			project.Name = "MyTestProject";	
			
			projectSystem.AddFrameworkReference("System.Xml");
			
			var expectedReferenceAndProjectName = new ReferenceAndProjectName() {
				Reference = "System.Xml",
				Project = "MyTestProject"
			};
			
			Assert.AreEqual(expectedReferenceAndProjectName, projectSystem.ReferenceAndProjectNamePassedToLogAddedReferenceToProject);
		}
		
		[Test]
		public void ResolvePath_PathPassed_ReturnsPathUnchanged()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string expectedPath = @"d:\temp";
			
			string path = projectSystem.ResolvePath(expectedPath);
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void AddFile_NewTextTemplateFileWithAssociatedDefaultCustomTool_AddsFileToProjectWithDefaultCustomTool()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			string path = @"d:\temp\abc.tt";
			AddDefaultCustomToolForFileName(path, "TextTemplatingFileGenerator");
			Stream stream = new MemoryStream();
			
			projectSystem.AddFile(path, stream);
			
			FileProjectItem fileItem = ProjectHelper.GetFile(project, path);
			string customTool = fileItem.CustomTool;
			Assert.AreEqual("TextTemplatingFileGenerator", customTool);
		}
		
		[Test]
		public void AddFile_NewFileWithNoAssociatedDefaultCustomTool_AddsFileToProjectWithNoDefaultCustomTool()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			string path = @"d:\temp\abc.tt";
			AddDefaultCustomToolForFileName(path, null);
			Stream stream = new MemoryStream();
			
			projectSystem.AddFile(path, stream);
			
			FileProjectItem fileItem = ProjectHelper.GetFile(project, path);
			string customTool = fileItem.CustomTool;
			Assert.AreEqual(String.Empty, customTool);
		}
		
		[Test]
		public void AddImport_FullImportFilePathAndBottomOfProject_PathRelativeToProjectAddedAsLastImportInProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			AssertLastMSBuildChildElementHasProjectAttributeValue(@"..\packages\Foo.0.1\build\Foo.targets");
		}
		
		[Test]
		public void AddImport_AddImportToBottomOfProject_ImportAddedWithConditionThatChecksForExistenceOfTargetsFile()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			AssertLastMSBuildChildHasCondition("Exists('..\\packages\\Foo.0.1\\build\\Foo.targets')");
		}
		
		[Test]
		public void AddImport_AddSameImportTwice_ImportOnlyAddedOnceToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			Assert.AreEqual(1, project.GetImports().Count);
		}
		
		[Test]
		public void AddImport_AddSameImportTwiceButWithDifferentCase_ImportOnlyAddedOnceToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath1 = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			string targetPath2 = @"d:\projects\MyProject\packages\Foo.0.1\BUILD\FOO.TARGETS";
			projectSystem.AddImport(targetPath1, ProjectImportLocation.Bottom);
			
			projectSystem.AddImport(targetPath2, ProjectImportLocation.Bottom);
			
			Assert.AreEqual(1, project.GetImports().Count);
		}
		
		[Test]
		public void AddImport_FullImportFilePathAndBottomOfProject_ProjectIsSaved()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			Assert.IsTrue(project.IsSaved);
		}
		
		[Test]
		public void AddImport_FullImportFilePathAndBottomOfProject_ProjectIsReevaluated()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			Assert.IsTrue(projectSystem.IsReevaluateProjectIfNecessaryCalled);
		}
		
		[Test]
		public void RemoveImport_ImportAlreadyAddedToBottomOfProject_ImportRemoved()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			projectSystem.AddImport(targetPath, ProjectImportLocation.Bottom);
			
			projectSystem.RemoveImport(targetPath);
			
			Assert.AreEqual(0, project.GetImports().Count);
		}
		
		[Test]
		public void RemoveImport_ImportAlreadyWithDifferentCaseAddedToBottomOfProject_ImportRemoved()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath1 = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			projectSystem.AddImport(targetPath1, ProjectImportLocation.Bottom);
			string targetPath2 = @"d:\projects\MyProject\packages\Foo.0.1\BUILD\FOO.TARGETS";
			
			projectSystem.RemoveImport(targetPath2);
			
			Assert.AreEqual(0, project.GetImports().Count);
		}
		
		[Test]
		public void RemoveImport_DifferentImportAdded_ExceptionNotThrown()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath1 = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			projectSystem.AddImport(targetPath1, ProjectImportLocation.Bottom);
			string targetPath2 = @"d:\projects\MyProject\packages\Bar.0.1\build\Bar.targets";
			
			Assert.DoesNotThrow(() => projectSystem.RemoveImport(targetPath2));
			
			Assert.AreEqual(1, project.GetImports().Count);
		}
		
		[Test]
		public void RemoveImport_NoImportsAdded_ProjectIsSaved()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			
			projectSystem.RemoveImport("Unknown.targets");
			
			Assert.IsTrue(project.IsSaved);
		}
		
		[Test]
		public void RemoveImport_NoImportsAdded_ProjectIsReevaluated()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			
			projectSystem.RemoveImport("Unknown.targets");
			
			Assert.IsTrue(projectSystem.IsReevaluateProjectIfNecessaryCalled);
		}
		
		[Test]
		public void AddImport_AddToTopOfProject_ImportAddedAsFirstChildElement()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Top);
			
			AssertFirstMSBuildChildElementHasProjectAttributeValue(@"..\packages\Foo.0.1\build\Foo.targets");
		}
		
		[Test]
		public void AddImport_AddImportToTopOfProject_ImportAddedWithConditionThatChecksForExistenceOfTargetsFile()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Top);
			
			AssertFirstMSBuildChildHasCondition("Exists('..\\packages\\Foo.0.1\\build\\Foo.targets')");
		}
		
		[Test]
		public void AddImport_AddToTopOfProjectTwice_ImportAddedOnlyOnce()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject\MyProject.csproj");
			CreateProjectSystem(project);
			string targetPath = @"d:\projects\MyProject\packages\Foo.0.1\build\Foo.targets";
			projectSystem.AddImport(targetPath, ProjectImportLocation.Top);
			
			projectSystem.AddImport(targetPath, ProjectImportLocation.Top);
			
			Assert.AreEqual(1, project.GetImports().Count);
		}
		
		[Test]
		public void AddFile_NewFileAddedWithAction_AddsFileToFileSystem()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string expectedPath = @"d:\temp\abc.cs";
			Action<Stream> expectedAction = stream => { };
			projectSystem.AddFile(expectedPath, expectedAction);
			
			Assert.AreEqual(expectedPath, projectSystem.PathPassedToPhysicalFileSystemAddFile);
			Assert.AreEqual(expectedAction, projectSystem.ActionPassedToPhysicalFileSystemAddFile);
		}
		
		[Test]
		public void AddFile_NewFileAddedWithAction_AddsFileToProject()
		{
			CreateTestProject(@"d:\projects\MyProject\MyProject.csproj");
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			Action<Stream> action = stream => { };
			projectSystem.AddFile(fileName, action);
			
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = new FileName(fileName);
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void ReferenceExists_ReferenceIsInProjectButIncludesAssemblyVersion_ReturnsTrue()
		{
			CreateTestProject();
			string include = "MyAssembly, Version=0.1.0.0, Culture=neutral, PublicKeyToken=8cc8392e8503e009";
			ProjectHelper.AddReference(project, include);
			CreateProjectSystem(project);
			string fileName = @"D:\Projects\Test\myassembly.dll";
			
			bool result = projectSystem.ReferenceExists(fileName);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void RemoveReference_ReferenceBeingRemovedHasFileExtensionAndProjectHasReferenceIncludingAssemblyVersion_ReferenceRemovedFromProject()
		{
			CreateTestProject();
			string include = "nunit.framework, Version=2.6.2.0, Culture=neutral, PublicKeyToken=8cc8392e8503e009";
			ProjectHelper.AddReference(project, include);
			CreateProjectSystem(project);
			string fileName = @"d:\projects\packages\nunit\nunit.framework.dll";
			
			projectSystem.RemoveReference(fileName);
			
			ReferenceProjectItem referenceItem = ProjectHelper.GetReference(project, "nunit.framework");
			Assert.IsNull(referenceItem);
		}
	}
}
