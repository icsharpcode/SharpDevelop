// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingServiceProviderTests
	{
		TextTemplatingServiceProvider serviceProvider;
		
		void CreateServiceProvider()
		{
			serviceProvider = new TextTemplatingServiceProvider();
		}
		
		[Test]
		public void GetService_TypeOfFakeServiceProvider_ReturnsNewFakeServiceProvider()
		{
			CreateServiceProvider();
			FakeServiceProvider service = serviceProvider.GetService(typeof(FakeServiceProvider)) as FakeServiceProvider;
			
			Assert.IsNotNull(service);
		}
	}
}
