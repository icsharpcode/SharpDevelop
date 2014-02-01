// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
