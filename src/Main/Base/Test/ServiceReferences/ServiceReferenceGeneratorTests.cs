// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceGeneratorTests
	{
		IProjectWithServiceReferences fakeProject;
		IServiceReferenceProxyGenerator fakeProxyGenerator;
		IServiceReferenceMapGenerator fakeReferenceMapGenerator;
		ServiceReferenceGenerator generator;
		ServiceReferenceFileGenerator fileGenerator;
		IFileSystem fakeFileSystem;
		ServiceReferenceGeneratorOptions options;
		List<ReferenceProjectItem> projectReferences;
		
		void CreateGenerator()
		{
			options = new ServiceReferenceGeneratorOptions();
			fakeProject = MockRepository.GenerateStub<IProjectWithServiceReferences>();
			projectReferences = new List<ReferenceProjectItem>();
			fakeProject.Stub(p => p.GetReferences()).Return(projectReferences);
			fakeProxyGenerator = MockRepository.GenerateStub<IServiceReferenceProxyGenerator>();
			fakeProxyGenerator.Options = options;
			fakeReferenceMapGenerator = MockRepository.GenerateStub<IServiceReferenceMapGenerator>();
			fileGenerator = new ServiceReferenceFileGenerator(fakeProxyGenerator, fakeReferenceMapGenerator);
			fakeFileSystem = MockRepository.GenerateStub<IFileSystem>();
			
			generator = new ServiceReferenceGenerator(fakeProject, fileGenerator, fakeFileSystem);
		}
		
		void SetProjectRootNamespace(string rootNamespace)
		{
			fakeProject.Stub(p => p.RootNamespace).Return(rootNamespace);
		}
		
		void SetServiceReferenceFileName(string serviceReferenceName, ServiceReferenceFileName fileName)
		{
			fakeProject.Stub(p => p.GetServiceReferenceFileName(serviceReferenceName)).Return(fileName);
		}
		
		ServiceReferenceFileName CreateProxyFileName(string serviceReferencesFolder, string serviceName)
		{
			return new ServiceReferenceFileName(serviceReferencesFolder, serviceName);
		}
		
		ServiceReferenceFileName AddProxyFileNameForServiceName(string serviceName)
		{
			return AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", serviceName);
		}
		
		ServiceReferenceFileName AddProxyFileNameForServiceName(string serviceReferencesFolder, string serviceName)
		{
			ServiceReferenceFileName proxyFileName = CreateProxyFileName(serviceReferencesFolder, serviceName);
			SetServiceReferenceFileName(serviceName, proxyFileName);
			return proxyFileName;
		}
		
		ServiceReferenceMapFileName AddMapFileNameForServiceName(string serviceName)
		{
			return AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", serviceName);
		}
		
		ServiceReferenceMapFileName AddMapFileNameForServiceName(string serviceReferencesFolder, string serviceName)
		{
			var fileName = new ServiceReferenceMapFileName(serviceReferencesFolder, serviceName);
			SetServiceReferenceMapFileName(serviceName, fileName);
			return fileName;
		}
		
		void SetServiceReferenceMapFileName(string serviceName, ServiceReferenceMapFileName fileName)
		{
			fakeProject.Stub(p => p.GetServiceReferenceMapFileName(serviceName)).Return(fileName);
		}
		
		void UseCSharpProject()
		{
			SetProjectLanguage("C#");
		}
		
		void UseVisualBasicProject()
		{
			SetProjectLanguage("VBNet");
		}
		
		void SetProjectLanguage(string language)
		{
			fakeProject.Stub(p => p.Language).Return(language);
		}
		
		void SetProjectAppConfigFileName(string fileName)
		{
			fakeProject.Stub(p => p.GetAppConfigFileName()).Return(fileName);
		}
		
		void ProjectDoesNotHaveAppConfigFile()
		{
			ProjectHasAppConfigFile(false);
		}
		
		void ProjectHasAppConfigFile()
		{
			ProjectHasAppConfigFile(true);
		}
		
		void ProjectHasAppConfigFile(bool hasAppConfigFile)
		{
			fakeProject.Stub(p => p.HasAppConfigFile()).Return(hasAppConfigFile);
		}
		
		ReferenceProjectItem AddReferenceToProject(string reference)
		{
			return AddReferenceToProject(reference, reference);
		}
		
		ReferenceProjectItem AddReferenceToProject(string reference, string fileName)
		{
			IProject dummyProject = MockRepository.GenerateStub<IProject>();
			dummyProject.Stub(p => p.SyncRoot).Return(new object());
			var projectItem = new ReferenceProjectItem(dummyProject, reference);
			projectItem.FileName = fileName;
			projectReferences.Add(projectItem);
			return projectItem;
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProxyFileIsGenerated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProxyGenerator.AssertWasCalled(p => p.GenerateProxyFile());
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProxyFileNameTakenFromProject()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			
			generator.AddServiceReference();
			string expectedProxyFileName = @"d:\projects\MyProject\Service References\MyServiceRef\Reference.cs";
			
			Assert.AreEqual(expectedProxyFileName, fakeProxyGenerator.Options.OutputFileName);
		}
		
		[Test]
		public void AddServiceReference_ServiceReferenceDoesNotExist_ServiceReferenceFolderCreated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyService1");
			AddMapFileNameForServiceName("MyService1");
			generator.Options.ServiceName = "MyService1";
			
			generator.AddServiceReference();
			
			string expectedDirectory = @"d:\projects\MyProject\Service References\MyService1";
			
			fakeFileSystem.AssertWasCalled(f => f.CreateDirectoryIfMissing(expectedDirectory));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ServiceReferenceProxyFileAddedToProject()
		{
			CreateGenerator();
			ServiceReferenceFileName expectedProxyFileName = 
				AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceProxyFile(expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProjectIsSaved()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.Save());
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ReferenceServiceMapFileIsCreated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			
			generator.AddServiceReference();
			
			var expectedMapFile = new ServiceReferenceMapFile() {
				FileName = @"d:\projects\MyProject\Service References\MyServiceRef\Reference.svcmap"
			};
			
			fakeReferenceMapGenerator.AssertWasCalled(gen => gen.GenerateServiceReferenceMapFile(expectedMapFile));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ReferenceServiceMapFileIsAddedToProject()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			ServiceReferenceMapFileName expectedMapFileName = 
				AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceMapFile(expectedMapFileName));
		}
		
		[Test]
		public void AddServiceReference_ProjectDoesNotHaveSystemServiceModelReference_SystemServiceModelReferenceAddedToProject()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.AddAssemblyReference("System.ServiceModel"));
		}
		
		[Test]
		public void AddServiceReference_ProjectDoesNotHaveSystemServiceModelReference_ProjectIsSavedAfterReferenceIsAdded()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			
			fakeProject
				.Stub(p => p.Save())
				.WhenCalled(new Action<MethodInvocation>(
					mi => fakeProject.AssertWasCalled(p => p.AddAssemblyReference("System.ServiceModel"))));
			
			generator.AddServiceReference();
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_NamespaceSetOnProxyGenerator()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			SetProjectRootNamespace("Test");
			
			generator.AddServiceReference();
			
			Assert.AreEqual("Test.MyServiceRef", fakeProxyGenerator.Options.Namespace);
		}
		
		[Test]
		public void AddServiceReference_ProjectHasNoRootNamespace_NamespaceSetOnProxyGeneratorMatchesServiceName()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Options.ServiceName = "MyServiceRef";
			SetProjectRootNamespace(String.Empty);
			
			generator.AddServiceReference();
			
			Assert.AreEqual("MyServiceRef", fakeProxyGenerator.Options.Namespace);
		}
		
		[Test]
		public void AddServiceReference_CSharpProject_CSharpProxyGenerated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			UseCSharpProject();
			
			generator.AddServiceReference();
			
			Assert.AreEqual("CS", fakeProxyGenerator.Options.Language);
		}
		
		[Test]
		public void AddServiceReference_VisualBasicProject_VisualBasicProxyGenerated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			UseVisualBasicProject();
			
			generator.AddServiceReference();
			
			Assert.AreEqual("VB", fakeProxyGenerator.Options.Language);
		}
		
		[Test]
		public void AddServiceReference_ProjectHasNoAppConfig_AppConfigFileNamePassedToGeneratorButNoFileMergeRequested()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			UseVisualBasicProject();
			string expectedAppConfigFileName = @"d:\projects\MyProject\app.config";
			SetProjectAppConfigFileName(expectedAppConfigFileName);
			ProjectDoesNotHaveAppConfigFile();
			
			generator.AddServiceReference();
			
			Assert.AreEqual(expectedAppConfigFileName, fakeProxyGenerator.Options.AppConfigFileName);
			Assert.IsFalse(fakeProxyGenerator.Options.NoAppConfig);
			Assert.IsFalse(fakeProxyGenerator.Options.MergeAppConfig);
			fakeProject.AssertWasCalled(p => p.AddAppConfigFile());
		}
		
		[Test]
		public void AddServiceReference_ProjectHasAppConfig_MergeAppConfigFileRequested()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			UseVisualBasicProject();
			string expectedAppConfigFileName = @"d:\projects\MyProject\app.config";
			SetProjectAppConfigFileName(expectedAppConfigFileName);
			ProjectHasAppConfigFile();
			
			generator.AddServiceReference();
			
			Assert.AreEqual(expectedAppConfigFileName, fakeProxyGenerator.Options.AppConfigFileName);
			Assert.IsFalse(fakeProxyGenerator.Options.NoAppConfig);
			Assert.IsTrue(fakeProxyGenerator.Options.MergeAppConfig);
			fakeProject.AssertWasNotCalled(p => p.AddAppConfigFile());
		}
		
		[Test]
		public void AddServiceReference_UseTypesInProjectReferencesIsTrue_ProjectReferencesAddedToOptions()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			generator.Options.UseTypesInProjectReferences = true;
			
			AddReferenceToProject("System.Windows.Forms");
			AddReferenceToProject(@"d:\projects\MyProject\lib\MyLib.dll");
			
			generator.AddServiceReference();
			
			string[] expectedReferences = new string[] {
				"System.Windows.Forms",
				@"d:\projects\MyProject\lib\MyLib.dll"
			};
			
			CollectionAssert.AreEqual(expectedReferences, fakeProxyGenerator.Options.Assemblies);
		}
				
		[Test]
		public void AddServiceReference_UseTypesInProjectReferencesIsFalse_NoProjectReferencesAddedToOptions()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.ServiceName = "MyService";
			generator.Options.UseTypesInProjectReferences = false;
			
			AddReferenceToProject("System.Windows.Forms");
			AddReferenceToProject(@"d:\projects\MyProject\lib\MyLib.dll");
			
			generator.AddServiceReference();
			
			Assert.AreEqual(0, fakeProxyGenerator.Options.Assemblies.Count);
		}
		
		[Test]
		public void GetCheckableAssemblyReferences_ProjectHasOneAssemblyReference_ReturnsOneAssemblyReference()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			AddReferenceToProject("System.Xml");
			
			List<CheckableAssemblyReference> references =
				generator.GetCheckableAssemblyReferences().ToList();
			
			Assert.AreEqual("System.Xml", references[0].Description);
		}
		
		[Test]
		public void GetCheckableAssemblyReferences_ProjectHasReferencesInNonAlphabeticalOrder_ReturnsAssemblyReferencesInAlphabeticalOrder()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			AddReferenceToProject("Aardvark");
			AddReferenceToProject("System.Xml");
			AddReferenceToProject("System.ComponentModel");
			
			List<CheckableAssemblyReference> references =
				generator.GetCheckableAssemblyReferences().ToList();
			
			Assert.AreEqual("Aardvark", references[0].Description);
			Assert.AreEqual("System.ComponentModel", references[1].Description);
			Assert.AreEqual("System.Xml", references[2].Description);
		}
		
		[Test]
		public void UpdateAssemblyReferences_TwoAssemblyReferencesButOnlyOneChecked_OneAssemblyReferenced()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			ReferenceProjectItem checkedReference = AddReferenceToProject("Checked", @"d:\projects\MyProject\Checked.dll");
			ReferenceProjectItem uncheckedReference = AddReferenceToProject("Unchecked");
			
			var references = new List<CheckableAssemblyReference>();
			references.Add(new CheckableAssemblyReference(checkedReference) { ItemChecked = true });
			references.Add(new CheckableAssemblyReference(uncheckedReference) { ItemChecked = false });
			
			generator.UpdateAssemblyReferences(references);
			
			string[] expectedAssemblies = new string[] {
				@"d:\projects\MyProject\Checked.dll"
			};
			
			CollectionAssert.AreEqual(expectedAssemblies, generator.Options.Assemblies);
		}
	}
}
