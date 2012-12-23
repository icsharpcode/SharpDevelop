// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using Rhino.Mocks;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class CustomTextTemplatingServiceProvidersTests
	{
		CustomServiceTextTemplatingServiceProviders customServiceProviders;
		IAddInTree fakeAddInTree;
		List<IServiceProvider> addInTreeServiceProviders;
		
		void CreateFakeAddInTree()
		{
			fakeAddInTree = MockRepository.GenerateStub<IAddInTree>();
			addInTreeServiceProviders = new List<IServiceProvider>();
			fakeAddInTree
				.Stub(tree => tree.BuildServiceProviders(CustomServiceTextTemplatingServiceProviders.AddInPath))
				.Return(addInTreeServiceProviders);
		}
		
		void CreateCustomTextTemplatingServiceProviders()
		{
			customServiceProviders = new CustomServiceTextTemplatingServiceProviders(fakeAddInTree);
		}
		
		void AddServiceObjectToReturnFromServiceProvider(IServiceProvider serviceProvider, Type type, object objectToReturn)
		{
			serviceProvider.Stub(provider => provider.GetService(type)).Return(objectToReturn);
		}
		
		IServiceProvider AddServiceProviderToAddInTree()
		{
			IServiceProvider serviceProvider = MockRepository.GenerateStub<IServiceProvider>();
			addInTreeServiceProviders.Add(serviceProvider);
			return serviceProvider;
		}
		
		[Test]
		public void GetService_AddInTreeDefinesNoServiceProviders_ReturnsNull()
		{
			CreateFakeAddInTree();
			CreateCustomTextTemplatingServiceProviders();
			
			object result = customServiceProviders.GetService(typeof(string));
			
			Assert.IsNull(result);
		}
		
		[Test]
		public void GetService_AddInTreeDefinesOneServiceProvider_TypeReturnedFromServiceProvider()
		{
			CreateFakeAddInTree();
			IServiceProvider serviceProvider = AddServiceProviderToAddInTree();
			object objectToReturnFromServiceProvider = new object();
			AddServiceObjectToReturnFromServiceProvider(serviceProvider, typeof(string), objectToReturnFromServiceProvider);
			CreateCustomTextTemplatingServiceProviders();
			
			object result = customServiceProviders.GetService(typeof(string));
			
			Assert.AreEqual(objectToReturnFromServiceProvider, result);
		}
		
		[Test]
		public void GetService_AddInTreeDefinesTwoServiceProvidersAndSecondDefinesServiceType_TypeReturnedFromSecondServiceProvider()
		{
			CreateFakeAddInTree();
			AddServiceProviderToAddInTree();
			IServiceProvider serviceProvider = AddServiceProviderToAddInTree();
			object objectToReturnFromServiceProvider = new object();
			AddServiceObjectToReturnFromServiceProvider(serviceProvider, typeof(string), objectToReturnFromServiceProvider);
			CreateCustomTextTemplatingServiceProviders();
			
			object result = customServiceProviders.GetService(typeof(string));
			
			Assert.AreEqual(objectToReturnFromServiceProvider, result);
		}
	}
}
