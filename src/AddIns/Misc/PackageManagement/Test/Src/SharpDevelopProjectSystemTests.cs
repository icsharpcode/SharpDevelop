// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Runtime.Versioning;

using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SharpDevelopProjectSystemTests
	{
		TestableSharpDevelopProjectSystem projectSystem;
		TestableProject project;
		
		void CreateProjectSystem(MSBuildBasedProject project)
		{
			projectSystem = new TestableSharpDevelopProjectSystem(project);
		}
		
		void CreateTestProject()
		{
			project = ProjectHelper.CreateTestProject();
		}
		
		[Test]
		public void Root_NewInstanceCreated_ReturnsProjectDirectory()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
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
			
			FrameworkName expectedName = new FrameworkName(".NETFramework, Profile=Full, Version=v4.0");
			
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
			
			FrameworkName expectedName = new FrameworkName("Silverlight, Profile=Full, Version=v2.0");
			
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
		public void IsSupportedFile_PassedAppConfigFileName_ReturnsFalse()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"d:\temp\app.config";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsSupportedFile_PassedAppConfigFileNameInUpperCase_ReturnsFalse()
		{
			CreateTestProject();
			CreateProjectSystem(project);
			
			string fileName = @"c:\projects\APP.CONFIG";
			bool result = projectSystem.IsSupportedFile(fileName);
			
			Assert.IsFalse(result);
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
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
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
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			projectSystem.AddFile(fileName, null);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = fileName;
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_NewResxFile_AddsFileToProjectWithCorrectItemType()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.EmbeddedResource;
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.resx";
			projectSystem.AddFile(fileName, null);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.EmbeddedResource);
			expectedFileItem.FileName = fileName;
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_RelativeFileNameUsed_AddsFileToProject()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string relativeFileName = @"src\NewFile.cs";
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			projectSystem.AddFile(relativeFileName, null);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = fileName;
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_RelativeFileNameWithNoPathUsed_AddsFileToProject()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string relativeFileName = @"NewFile.cs";
			string fileName = @"d:\projects\MyProject\NewFile.cs";
			projectSystem.AddFile(relativeFileName, null);
			FileProjectItem fileItem = ProjectHelper.GetFile(project, fileName);
			
			FileProjectItem expectedFileItem = new FileProjectItem(project, ItemType.Compile);
			expectedFileItem.FileName = fileName;
			
			FileProjectItemAssert.AreEqual(expectedFileItem, fileItem);
		}
		
		[Test]
		public void AddFile_NewFile_ProjectIsSavedAfterFileAddedToProject()
		{
			CreateTestProject();
			project.IsSaved = false;
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			CreateProjectSystem(project);
			
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			projectSystem.AddFile(fileName, null);
			
			Assert.AreEqual(1, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void AddFile_NewFileToBeAddedInBinFolder_FileIsNotAddedToProject()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			CreateProjectSystem(project);
			
			string fileName = @"bin\NewFile.dll";
			projectSystem.AddFile(fileName, null);
			
			FileProjectItem fileItem = ProjectHelper.GetFileFromInclude(project, fileName);
			
			Assert.IsNull(fileItem);
		}
		
		[Test]
		public void AddFile_NewFileToBeAddedInBinFolderWithBinFolderNameInUpperCase_FileIsNotAddedToProject()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			CreateProjectSystem(project);
			
			string fileName = @"BIN\NewFile.dll";
			projectSystem.AddFile(fileName, null);
			
			FileProjectItem fileItem = ProjectHelper.GetFileFromInclude(project, fileName);
			
			Assert.IsNull(fileItem);
		}
		
		[Test]
		public void AddFile_FileAlreadyExistsInProject_FileIsNotAddedToProject()
		{
			CreateTestProject();
			project.FileName = @"d:\projects\MyProject\MyProject.csproj";
			project.ItemTypeToReturnFromGetDefaultItemType = ItemType.Compile;
			CreateProjectSystem(project);
			
			string relativeFileName = @"src\NewFile.cs";
			string fileName = @"d:\projects\MyProject\src\NewFile.cs";
			ProjectHelper.AddFile(project, fileName);
			
			projectSystem.AddFile(relativeFileName, null);
			
			int projectItemsCount = project.Items.Count;
			Assert.AreEqual(1, projectItemsCount);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromFileSystem_CallsFileServiceRemoveFile()
		{
			CreateTestProject();
			project.FileName = @"d:\temp\MyProject.csproj";
			string fileName = @"d:\temp\test.cs";
			ProjectHelper.AddFile(project, fileName);
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile(fileName);
			
			Assert.AreEqual(fileName, projectSystem.FakeFileService.PathPassedToRemoveFile);
		}
		
		[Test]
		public void DeleteFile_DeletesFileFromFileSystem_ProjectIsSavedAfterFileRemoved()
		{
			CreateTestProject();
			project.FileName = @"d:\temp\MyProject.csproj";
			project.IsSaved = false;
			string fileName = @"d:\temp\test.cs";
			ProjectHelper.AddFile(project, fileName);
			CreateProjectSystem(project);
			
			projectSystem.DeleteFile(fileName);
			
			Assert.AreEqual(0, project.ItemsWhenSaved.Count);
		}
		
		[Test]
		public void DeleteDirectory_DeletesDirectoryFromFileSystem_CallsFileServiceRemoveDirectory()
		{
			CreateTestProject();
			project.FileName = @"d:\temp\MyProject.csproj";
			string fileName = @"d:\temp\test.cs";
			ProjectHelper.AddFile(project, fileName);
			CreateProjectSystem(project);
			string path = @"d:\temp\test";
			
			projectSystem.DeleteDirectory(path);
			
			Assert.AreEqual(path, projectSystem.FakeFileService.PathPassedToRemoveDirectory);
		}
		
		[Test]
		public void DeleteDirectory_DeletesDirectoryFromFileSystem_ProjectIsSavedAfterDirectoryDeleted()
		{
			CreateTestProject();
			project.FileName = @"d:\temp\MyProject.csproj";
			string fileName = @"d:\temp\test.cs";
			ProjectHelper.AddFile(project, fileName);
			project.IsSaved = false;
			CreateProjectSystem(project);
			string path = @"d:\temp";
			
			projectSystem.DeleteDirectory(path);
			
			Assert.AreEqual(0, project.ItemsWhenSaved.Count);
		}
	}
}
