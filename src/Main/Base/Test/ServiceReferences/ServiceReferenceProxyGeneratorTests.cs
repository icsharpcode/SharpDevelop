// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ServiceModel.Description;
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceProxyGeneratorTests
	{
		ServiceReferenceProxyGenerator proxyGenerator;
		IProjectWithServiceReferences fakeProject;
		IServiceReferenceCodeDomBuilder fakeCodeDomBuilder;
		ICodeDomProvider fakeCodeDomProvider;
		MetadataSet metadata;
		
		void CreateProxyGenerator()
		{
			metadata = new MetadataSet();
			
			fakeCodeDomBuilder = MockRepository.GenerateStub<IServiceReferenceCodeDomBuilder>();
			fakeProject = MockRepository.GenerateStub<IProjectWithServiceReferences>();
			fakeCodeDomProvider = MockRepository.GenerateStub<ICodeDomProvider>();
			proxyGenerator = new ServiceReferenceProxyGenerator(fakeCodeDomProvider, fakeCodeDomBuilder);
		}
		
		CodeCompileUnit CreateCompileUnitToReturnFromCodeDomBuilder(MetadataSet metadata)
		{
			var compileUnit = new CodeCompileUnit();
			fakeCodeDomBuilder.Stub(c => c.GenerateCompileUnit(metadata)).Return(compileUnit);
			return compileUnit;
		}
		
		[Test]
		public void GenerateProxyFile_ProxyToBeGeneratedForMetadata_CodeGeneratedFromCodeDomForProxyFileInProjectSubFolder()
		{
			CreateProxyGenerator();
			CodeCompileUnit compileUnit = CreateCompileUnitToReturnFromCodeDomBuilder(metadata);
			proxyGenerator.ServiceReferenceNamespace = "Test";
			string expectedProxyFileName = @"d:\projects\MyProject\Service References\Test\Service1\Reference.cs";
			
			proxyGenerator.GenerateProxyFile(metadata, expectedProxyFileName);
			
			fakeCodeDomProvider.AssertWasCalled(p => p.GenerateCodeFromCompileUnit(compileUnit, expectedProxyFileName));
		}
	}
}
