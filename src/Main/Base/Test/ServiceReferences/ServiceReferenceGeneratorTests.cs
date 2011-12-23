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
		ServiceReferenceGenerator generator;
		IFileSystem fakeFileSystem;
		MetadataSet metadata;
		
		void CreateGenerator()
		{
			metadata = new MetadataSet();
			
			fakeProject = MockRepository.GenerateStub<IProjectWithServiceReferences>();
			fakeProxyGenerator = MockRepository.GenerateStub<IServiceReferenceProxyGenerator>();
			fakeFileSystem = MockRepository.GenerateStub<IFileSystem>();
			generator = new ServiceReferenceGenerator(fakeProject, fakeProxyGenerator, fakeFileSystem);
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
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_MetadataPassedToProxyGenerator()
		{
			CreateGenerator();
			ServiceReferenceFileName proxyFileName = 
				AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyServiceRef");
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			string expectedProxyFileName = @"d:\projects\MyProject\Service References\MyServiceRef\Reference.cs";
			
			fakeProxyGenerator.AssertWasCalled(p => p.GenerateProxy(metadata, expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_ServiceReferenceDoesNotExist_ServiceReferenceFolderCreated()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName(@"d:\projects\MyProject\Service References", "MyService1");
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
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceProxyFile(expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProjectIsSaved()
		{
			CreateGenerator();
			AddProxyFileNameForServiceName("MyServiceRef");
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.Save());
		}
	}
}
