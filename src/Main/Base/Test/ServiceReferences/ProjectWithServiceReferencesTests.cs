// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Tests.WebReferences;
using Microsoft.CSharp;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ProjectWithServiceReferencesTests
	{
		IProject fakeProject;
		ProjectWithServiceReferences project;
		List<ProjectItem> projectItemsAddedToProject;
		MSBuildBasedProject msbuildProject;
		
		void CreateProject()
		{
			projectItemsAddedToProject = new List<ProjectItem>();
			fakeProject = MockRepository.GenerateStub<IProject>();
			project = new ProjectWithServiceReferences(fakeProject);
		}
		
		void CreateProjectWithMSBuildProject()
		{
			msbuildProject = WebReferenceTestHelper.CreateTestProject("C#");
			project = new ProjectWithServiceReferences(msbuildProject);
		}
		
		void CreateProjectWithVisualBasicMSBuildProject()
		{
			msbuildProject = WebReferenceTestHelper.CreateTestProject("VBNet");
			project = new ProjectWithServiceReferences(msbuildProject);			
		}
		
		void SetProjectDirectory(string directory)
		{
			fakeProject.Stub(p => p.Directory).Return(directory);			
		}
		
		void SetProjectCodeDomProvider(LanguageProperties languageProperties)
		{
			fakeProject.Stub(p => p.LanguageProperties).Return(languageProperties);
		}
		
		ProjectItem GetFirstServiceReferenceFileInMSBuildProject(ServiceReferenceFileName fileName)
		{
			return msbuildProject.Items.SingleOrDefault(item => item.FileName == fileName.Path);
		}
		
		ServiceReferencesProjectItem GetFirstWCFMetadataItemInMSBuildProject()
		{
			return msbuildProject.GetItemsOfType(ItemType.ServiceReferences).SingleOrDefault() as ServiceReferencesProjectItem;
		}
		
		int GetHowManyWCFMetadataItemsInMSBuildProject()
		{
			return msbuildProject.GetItemsOfType(ItemType.ServiceReferences).Count();
		}
		
		ProjectItem GetFileProjectItemInMSBuildProject(string fileName)
		{
			return msbuildProject.Items.SingleOrDefault(item => item.FileName == fileName);
		}
		
		ServiceReferenceProjectItem GetFirstWCFMetadataStorageItemInMSBuildProject()
		{
			return msbuildProject.GetItemsOfType(ItemType.ServiceReference).SingleOrDefault() as ServiceReferenceProjectItem;
		}
		
		FileProjectItem GetFileFromMSBuildProject(string fileName)
		{
			return msbuildProject.Items.Single(item => item.FileName == fileName) as FileProjectItem;
		}
		
		ReferenceProjectItem GetReferenceFromMSBuildProject(string name)
		{
			return msbuildProject
				.GetItemsOfType(ItemType.Reference)
				.SingleOrDefault(item => item.Include == name) as ReferenceProjectItem;
		}
		
		void AddAssemblyReferenceToMSBuildProject(string name)
		{
			var item = new ReferenceProjectItem(msbuildProject, name);
			ProjectService.AddProjectItem(msbuildProject, item);
		}
		
		void AddFileToMSBuildProject(string include)
		{
			var fileItem = new FileProjectItem(msbuildProject, ItemType.None, include);
			ProjectService.AddProjectItem(msbuildProject, fileItem);
		}
		
		ReferenceProjectItem AddGacReferenceToProject(string name)
		{
			var referenceItem = new ReferenceProjectItem(msbuildProject, name);
			ProjectService.AddProjectItem(msbuildProject, referenceItem);
			return referenceItem;
		}
		
		int CountAssemblyReferencesInMSBuildProject()
		{
			return msbuildProject.GetItemsOfType(ItemType.Reference).Count();
		}
		
		[Test]
		public void ServiceReferencesFolder_ProjectHasNoServiceReferences_ReturnsServiceReferencesFolderAsProjectSubFolder()
		{
			CreateProject();
			SetProjectDirectory(@"d:\projects\MyProject");
			
			string folder = project.ServiceReferencesFolder;
			string expectedFolder = @"d:\projects\MyProject\Service References";
			
			Assert.AreEqual(expectedFolder, folder);
		}
		
		[Test]
		public void GetServiceReferenceProxyFileName_ProjectHasNoServiceReferences_ReturnsFileNameInServiceReferencesFolderWithSubFolderNamedAfterServiceReference()
		{
			CreateProject();
			SetProjectDirectory(@"d:\projects\MyProject");
			
			ServiceReferenceFileName fileName = project.GetServiceReferenceFileName("Service1");
			string expectedFileName = @"d:\projects\MyProject\Service References\Service1\Reference.cs";
			
			Assert.AreEqual(expectedFileName, fileName.Path);
		}
		
		[Test]
		public void AddServiceReferenceProxyFile_ProjectHasNoServiceReferences_ProxyFileAddedToProjectAsFileToCompile()
		{
			CreateProjectWithMSBuildProject();
			
			var proxyFileName = new ServiceReferenceFileName() {
				ServiceReferencesFolder = @"d:\projects\MyProject\Service References",
				ServiceName = "Service1"
			};
			project.AddServiceReferenceProxyFile(proxyFileName);
			
			ProjectItem item = GetFirstServiceReferenceFileInMSBuildProject(proxyFileName);
			
			string dependentUpon = item.GetMetadata("DependentUpon");
			
			Assert.AreEqual(ItemType.Compile, item.ItemType);
			Assert.AreEqual("Reference.svcmap", dependentUpon);
		}
		
		[Test]
		public void Save_SaveProjectChanges_UnderlyingProjectIsSaved()
		{
			CreateProject();
			
			project.Save();
			
			fakeProject.AssertWasCalled(p => p.Save());
		}
		
		[Test]
		public void AddServiceReferenceProxyFile_ProjectHasNoServiceReferences_WCFMetadataItemAddedToProjectForServiceReferencesFolder()
		{
			CreateProjectWithMSBuildProject();
			
			var proxyFileName = new ServiceReferenceFileName() { ServiceName = "Service1" };
			project.AddServiceReferenceProxyFile(proxyFileName);
			
			ServiceReferencesProjectItem item = GetFirstWCFMetadataItemInMSBuildProject();
			
			Assert.AreEqual("Service References", item.Include);
		}
		
		[Test]
		public void AddServiceReferenceProxyFile_ProjectHasServiceReferences_WCFMetadataItemNotAddedToProjectForServiceReferencesRootFolder()
		{
			CreateProjectWithMSBuildProject();
			var proxyFileName = new ServiceReferenceFileName() { ServiceName = "Service1" };
			project.AddServiceReferenceProxyFile(proxyFileName);
			proxyFileName = new ServiceReferenceFileName() { ServiceName = "Service2" };
			
			project.AddServiceReferenceProxyFile(proxyFileName);
			
			int count = GetHowManyWCFMetadataItemsInMSBuildProject();
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void AddServiceReferenceProxyFile_ProjectHasNoServiceReferences_WCFMetadataStorageItemAddedToProjectForServiceReferencesFolder()
		{
			CreateProjectWithMSBuildProject();
			
			var proxyFileName = new ServiceReferenceFileName() { ServiceName = "Service1" };
			project.AddServiceReferenceProxyFile(proxyFileName);
			
			ProjectItem item = GetFirstWCFMetadataStorageItemInMSBuildProject();
			
			Assert.AreEqual(@"Service References\Service1", item.Include);
		}
		
		[Test]
		public void GetServiceReferenceMapFileName_ProjectHasNoServiceReferences_ReturnsMapFileNameInServiceReferencesFolderWithSubFolderNamedAfterServiceReference()
		{
			CreateProject();
			SetProjectDirectory(@"d:\projects\MyProject");
			
			ServiceReferenceMapFileName fileName = project.GetServiceReferenceMapFileName("Service1");
			string expectedFileName = @"d:\projects\MyProject\Service References\Service1\Reference.svcmap";
			
			Assert.AreEqual(expectedFileName, fileName.Path);
		}
		
		[Test]
		public void AddServiceReferenceMapFile_ProjectHasNoServiceReferences_ServiceReferenceMapAddedToProject()
		{
			CreateProjectWithMSBuildProject();
			
			var mapFileName = new ServiceReferenceMapFileName(@"d:\projects\MyProject\Service References", "Service1");
			project.AddServiceReferenceMapFile(mapFileName);
			
			string fileName = @"d:\projects\MyProject\Service References\Service1\Reference.svcmap";
			FileProjectItem item = GetFileFromMSBuildProject(fileName);
			
			string lastGenOutput = item.GetMetadata("LastGenOutput");
			string generator = item.GetMetadata("Generator");
			
			Assert.AreEqual(ItemType.None, item.ItemType);
			Assert.AreEqual("Reference.cs", lastGenOutput);
			Assert.AreEqual("WCF Proxy Generator", generator);
		}
		
		[Test]
		public void AddAssemblyReference_SystemServiceModelAddedToProjectWithoutReference_AssemblyReferenceAdded()
		{
			CreateProjectWithMSBuildProject();
			
			project.AddAssemblyReference("System.ServiceModel");
			
			ReferenceProjectItem item = GetReferenceFromMSBuildProject("System.ServiceModel");
			
			Assert.IsNotNull(item);
		}
		
		[Test]
		public void AddAssemblyReference_SystemServiceModelAddedToProjectThatHasSystemServiceModelReference_AssemblyReferenceIsNotAdded()
		{
			CreateProjectWithMSBuildProject();
			AddAssemblyReferenceToMSBuildProject("System.ServiceModel");
			
			project.AddAssemblyReference("System.ServiceModel");
			
			int count = CountAssemblyReferencesInMSBuildProject();
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void AddAssemblyReference_SystemServiceModelAddedToProjectThatHasOneReferenceToSystemXml_ProjectHasTwoReferences()
		{
			CreateProjectWithMSBuildProject();
			AddAssemblyReferenceToMSBuildProject("System.Xml");
			
			project.AddAssemblyReference("System.ServiceModel");
			
			int count = CountAssemblyReferencesInMSBuildProject();
			
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void AddAssemblyReference_SystemServiceModelAddedToProjectThatHasSystemServiceModelReferenceInDifferentCase_AssemblyReferenceIsNotAdded()
		{
			CreateProjectWithMSBuildProject();
			AddAssemblyReferenceToMSBuildProject("system.serviceModel");
			
			project.AddAssemblyReference("System.ServiceModel");
			
			int count = CountAssemblyReferencesInMSBuildProject();
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void AddAssemblyReference_SystemServiceModelAddedToProjectThatHasSystemServiceModelReferenceUsingFullAssemblyName_AssemblyReferenceIsNotAdded()
		{
			CreateProjectWithMSBuildProject();
			AddAssemblyReferenceToMSBuildProject("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			
			project.AddAssemblyReference("System.ServiceModel");
			
			int count = CountAssemblyReferencesInMSBuildProject();
			
			Assert.AreEqual(1, count);
		}
		
		[Test]
		public void Language_CSharpProject_CSharpLanguageReturned()
		{
			CreateProjectWithMSBuildProject();
			
			Assert.AreEqual("C#", project.Language);
		}
		
		[Test]
		public void Language_VisualBasicProject_VisualBasicLanguageReturned()
		{
			CreateProjectWithVisualBasicMSBuildProject();
			
			Assert.AreEqual("VBNet", project.Language);
		}
		
		[Test]
		public void RootNamespace_MSBuildProjectHasRootNamespace_RootNamespaceReturned()
		{
			CreateProjectWithMSBuildProject();
			msbuildProject.RootNamespace = "Test";
			
			Assert.AreEqual("Test", project.RootNamespace);
		}
		
		[Test]
		public void RootNamespace_MSBuildProjectHasNullRootNamespace_EmptyStringReturned()
		{
			CreateProjectWithMSBuildProject();
			msbuildProject.Name = null;
			
			Assert.AreEqual(String.Empty, project.RootNamespace);
		}
		
		[Test]
		public void AddAppConfigFile_ProjectHasNoAppConfig_ProjectItemAddedToProjectForAppConfig()
		{
			CreateProjectWithMSBuildProject();
			msbuildProject.FileName = @"d:\projects\MyProject\myproject.csproj";
			
			project.AddAppConfigFile();
			
			ProjectItem item = GetFileFromMSBuildProject(@"d:\projects\MyProject\app.config");
			
			Assert.IsNotNull(item);
			Assert.AreEqual(ItemType.None, item.ItemType);
		}
		
		[Test]
		public void HasAppConfigFile_ProjectHasNoAppConfig_ReturnsFalse()
		{
			CreateProjectWithMSBuildProject();
			bool result = project.HasAppConfigFile();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasAppConfigFile_ProjectHasAppConfig_ReturnsTrue()
		{
			CreateProjectWithMSBuildProject();
			project.AddAppConfigFile();
			
			bool result = project.HasAppConfigFile();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasAppConfigFile_ProjectHasAppConfigInSubFolder_ReturnsTrue()
		{
			CreateProjectWithMSBuildProject();
			AddFileToMSBuildProject(@"SubFolder\app.config");
			
			bool result = project.HasAppConfigFile();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void HasAppConfigFile_ProjectHasAppConfigInUpperCase_ReturnsTrue()
		{
			CreateProjectWithMSBuildProject();
			AddFileToMSBuildProject(@"APP.CONFIG");
			
			bool result = project.HasAppConfigFile();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void GetAppConfigFileName_ProjectHasNoAppConfig_DefaultAppConfigFileNameReturned()
		{
			CreateProjectWithMSBuildProject();
			msbuildProject.FileName = @"d:\projects\MyProject\myproject.csproj";
			
			string fileName = project.GetAppConfigFileName();
			
			Assert.AreEqual(@"d:\projects\MyProject\app.config", fileName);
		}
		
		[Test]
		public void GetAppConfigFileName_ProjectHasAppConfigInSubFolder_AppConfigFileNameReturned()
		{
			CreateProjectWithMSBuildProject();
			msbuildProject.FileName = @"d:\projects\MyProject\myproject.csproj";
			AddFileToMSBuildProject(@"SubFolder\app.config");
			string fileName = project.GetAppConfigFileName();
			
			Assert.AreEqual(@"d:\projects\MyProject\SubFolder\app.config", fileName);
		}
		
		[Test]
		public void GetReferences_ProjectHasGacReference_ReturnsGacReference()
		{
			CreateProjectWithMSBuildProject();
			ReferenceProjectItem refItem = AddGacReferenceToProject("System.Xml");
			IEnumerable<ReferenceProjectItem> references = project.GetReferences();
			
			var expectedReferences = new ReferenceProjectItem[] {
				refItem
			};
			CollectionAssert.AreEqual(expectedReferences, references);
		}
	}
}
