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
		
		void SetServiceReferenceFileName(string serviceReferenceName, string fileName)
		{
			fakeProject.Stub(p => p.GetServiceReferenceFileName(serviceReferenceName)).Return(fileName);
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_MetadataPassedToProxyGenerator()
		{
			CreateGenerator();
			string expectedProxyFileName = @"d:\projects\MyProject\Service References\MyServiceRef\Reference.cs";
			SetServiceReferenceFileName("MyServiceRef", expectedProxyFileName);
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProxyGenerator.AssertWasCalled(p => p.GenerateProxy(metadata, expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_ServiceReferenceDoesNotExist_ServiceReferenceFolderCreated()
		{
			CreateGenerator();
			string proxyFileName = @"d:\projects\MyProject\Service References\MyService1\Reference.cs";
			SetServiceReferenceFileName("MyService1", proxyFileName);
			generator.Namespace = "MyService1";
			
			generator.AddServiceReference(metadata);
			
			string expectedDirectory = @"d:\projects\MyProject\Service References\MyService1";
			
			fakeFileSystem.AssertWasCalled(f => f.CreateDirectoryIfMissing(expectedDirectory));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ServiceReferenceProxyFileAddedToProject()
		{
			CreateGenerator();
			string expectedProxyFileName = @"d:\projects\MyProject\Service References\MyServiceRef\Reference.cs";
			SetServiceReferenceFileName("MyServiceRef", expectedProxyFileName);
			generator.Namespace = "MyServiceRef";
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.AddServiceReferenceProxyFile(expectedProxyFileName));
		}
		
		[Test]
		public void AddServiceReference_GeneratesServiceReference_ProjectIsSaved()
		{
			CreateGenerator();
			
			generator.AddServiceReference(metadata);
			
			fakeProject.AssertWasCalled(p => p.Save());
		}
	}
}
