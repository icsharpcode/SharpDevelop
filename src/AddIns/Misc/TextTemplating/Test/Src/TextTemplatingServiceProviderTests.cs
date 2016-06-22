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
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using Rhino.Mocks;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingServiceProviderTests
	{
		TextTemplatingServiceProvider textTemplatingServiceProvider;
		
		void CreateTextTemplatingServiceProvider()
		{
			IServiceProvider serviceProvider = CreateFakeServiceProvider();
			CreateTextTemplatingServiceProvider(serviceProvider);
		}
		
		void CreateTextTemplatingServiceProvider(IServiceProvider serviceProvider)
		{
			textTemplatingServiceProvider = new TextTemplatingServiceProvider(serviceProvider);
		}
		
		IServiceProvider CreateFakeServiceProvider()
		{
			return MockRepository.GenerateStub<IServiceProvider>();
		}
		
		IServiceProvider CreateFakeServiceProvider(Type serviceType, object service)
		{
			IServiceProvider serviceProvider = CreateFakeServiceProvider();
			AddServiceToFakeServiceProvider(serviceProvider, serviceType, service);
			return serviceProvider;
		}
		
		void AddServiceToFakeServiceProvider(IServiceProvider serviceProvider, Type serviceType, object service)
		{
			serviceProvider
				.Stub(provider => provider.GetService(serviceType))
				.Return(service);
		}
		
		[Test]
		public void GetService_TypeOfTextTemplatingServiceProviderTests_ReturnsTextTemplatingServiceProviderTests()
		{
			CreateTextTemplatingServiceProvider();
			
			var service = textTemplatingServiceProvider.GetService(typeof(TextTemplatingServiceProviderTests)) as TextTemplatingServiceProviderTests;
			
			Assert.IsNotNull(service);
		}
		
		[Test]
		public void GetService_ServiceDefinedByServiceProviderUsedInConstructor_ReturnsServiceFromServiceProvider()
		{
			var expectedService = new object();
			IServiceProvider fakeServiceProvider = 
				CreateFakeServiceProvider(typeof(TextTemplatingServiceProviderTests), expectedService);
			CreateTextTemplatingServiceProvider(fakeServiceProvider);
			
			object service = textTemplatingServiceProvider.GetService(typeof(TextTemplatingServiceProviderTests));
			
			Assert.AreEqual(expectedService, service);
		}
	}
}
