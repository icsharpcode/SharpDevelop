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
		
		ServiceReferenceProjectItem GetFirstWCFMetadataStorageItemInMSBuildProject()
		{
			return msbuildProject.GetItemsOfType(ItemType.ServiceReference).SingleOrDefault() as ServiceReferenceProjectItem;
		}
		
		FileProjectItem GetFileFromMSBuildProject(string fileName)
		{
			return msbuildProject.Items.Single(item => item.FileName == fileName) as FileProjectItem;
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
		public void CodeDomProvider_UnderlyingProjectUsesCSharpCodeDomProvider_ProjectUsesCSharpCodeDomProvider()
		{
			CreateProject();
			SetProjectCodeDomProvider(LanguageProperties.CSharp);
			
			ICodeDomProvider codeDomProvider = project.CodeDomProvider;
			string fileExtension = codeDomProvider.FileExtension;
			
			Assert.AreEqual("cs", fileExtension);
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
	}
}
