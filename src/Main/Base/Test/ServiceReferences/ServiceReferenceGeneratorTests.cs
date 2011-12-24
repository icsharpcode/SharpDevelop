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
		MetadataSet metadata;
		
		void CreateGenerator()
		{
			metadata = new MetadataSet();
			
			fakeProject = MockRepository.GenerateStub<IProjectWithServiceReferences>();
			fakeProxyGenerator = MockRepository.GenerateStub<IServiceReferenceProxyGenerator>();
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
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_MetadataPassedToProxyGenerator()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			string expectedProxyFileName = @"d:\projects\MyProject\Service References\MyServiceRef\Reference.cs";
			
			fakeProxyGenerator.AssertWasCalled(p => p.GenerateProxyFile(metadata, expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_ServiceReferenceDoesNotExist_ServiceReferenceFolderCreated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyService1");
			AddMapFileNameForServiceName("MyService1");
			generator.Namespace = "MyService1";
			
			generator.AddServiceReference(metadata);
			
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
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceProxyFile(expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProjectIsSaved()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			AddMapFileNameForServiceName("MyServiceRef");
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.Save());
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ReferenceServiceMapFileIsCreated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			AddMapFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
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
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceMapFile(expectedMapFileName));
		}
	}
}
