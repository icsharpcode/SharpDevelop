// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		void CreateGenerator()
		{
			options = new ServiceReferenceGeneratorOptions();
			fakeProject = MockRepository.GenerateStub<IProjectWithServiceReferences>();
			fakeProxyGenerator = MockRepository.GenerateStub<IServiceReferenceProxyGenerator>();
			fakeProxyGenerator
				.Stub(p => p.Options)
				.Return(options);
			fakeReferenceMapGenerator = MockRepository.GenerateStub<IServiceReferenceMapGenerator>();
			fileGenerator = new ServiceReferenceFileGenerator(fakeProxyGenerator, fakeReferenceMapGenerator);
			fakeFileSystem = MockRepository.GenerateStub<IFileSystem>();
			
			generator = new ServiceReferenceGenerator(fakeProject, fileGenerator, fakeFileSystem);
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
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProxyFileIsGenerated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.Namespace = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProxyGenerator.AssertWasCalled(p => p.GenerateProxyFile());
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProxyFileNameTakenFromProject()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.Namespace = "MyServiceRef";
			
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
			generator.Options.Namespace = "MyService1";
			
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
			generator.Options.Namespace = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceProxyFile(expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProjectIsSaved()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Options.Namespace = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.Save());
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ReferenceServiceMapFileIsCreated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Options.Namespace = "MyServiceRef";
			
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
			generator.Options.Namespace = "MyServiceRef";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceMapFile(expectedMapFileName));
		}
		
		[Test]
		public void AddServiceReference_ProjectDoesNotHaveSystemServiceModelReference_SystemServiceModelReferenceAddedToProject()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.Namespace = "MyService";
			
			generator.AddServiceReference();
			
			fakeProject.AssertWasCalled(p => p.AddAssemblyReference("System.ServiceModel"));
		}
		
		[Test]
		public void AddServiceReference_ProjectDoesNotHaveSystemServiceModelReference_ProjectIsSavedAfterReferenceIsAdded()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.Namespace = "MyService";
			
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
			ServiceReferenceMapFileName expectedMapFileName = 
				AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Options.Namespace = "MyServiceRef";
			
			generator.AddServiceReference();
			
			Assert.AreEqual("MyServiceRef", fakeProxyGenerator.Options.Namespace);
		}
		
		[Test]
		public void AddServiceReference_CSharpProject_CSharpProxyGenerated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyService");
			AddMapFileNameForServiceName("MyService");
			generator.Options.Namespace = "MyService";
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
			generator.Options.Namespace = "MyService";
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
			generator.Options.Namespace = "MyService";
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
			generator.Options.Namespace = "MyService";
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
	}
}
