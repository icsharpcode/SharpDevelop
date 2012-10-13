// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
