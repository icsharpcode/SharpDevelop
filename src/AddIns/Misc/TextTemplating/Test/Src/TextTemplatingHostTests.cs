// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingHostTests
	{
		TextTemplatingHost host;
		FakeTextTemplatingAppDomainFactory textTemplatingAppDomainFactory;
		FakeTextTemplatingAppDomain textTemplatingAppDomain;
		
		void CreateHost()
		{
			CreateHost(String.Empty);
		}
		
		void CreateHost(string applicationBase)
		{
			textTemplatingAppDomainFactory = new FakeTextTemplatingAppDomainFactory();
			textTemplatingAppDomain = textTemplatingAppDomainFactory.FakeTextTemplatingAppDomain;
			host = new TextTemplatingHost(textTemplatingAppDomainFactory, applicationBase);
		}
		
		[Test]
		public void ProvideTemplatingAppDomain_PassedContentName_ReturnsDomainFromTextTemplatingAppDomainFactory()
		{
			CreateHost();
			AppDomain expectedAppDomain = AppDomain.CreateDomain("TextTemplatingHostTests");
			textTemplatingAppDomain.AppDomain = expectedAppDomain;
			
			AppDomain actualAppDomain = host.ProvideTemplatingAppDomain("test");
			
			Assert.AreEqual(expectedAppDomain, actualAppDomain);
		}
		
		[Test]
		public void Dispose_DisposingHostAfterProvideTemplatingAppDomainCalled_DisposesTemplatingAppDomain()
		{
			CreateHost();
			host.ProvideTemplatingAppDomain("test");
			host.Dispose();
			
			Assert.IsTrue(textTemplatingAppDomain.IsDisposeCalled);
		}
		
		[Test]
		public void Dispose_DisposingHostWhenProvideTemplatingAppDomainIsNotCalled_DoesNotThrowNullReferenceException()
		{
			CreateHost();
			Assert.DoesNotThrow(() => host.Dispose());
		}
		
		[Test]
		public void ProvideTemplatingAppDomain_MethodCalledTwice_AppDomainCreatedOnce()
		{
			CreateHost();
			host.ProvideTemplatingAppDomain("test");
			host.ProvideTemplatingAppDomain("test");
			
			Assert.AreEqual(1, textTemplatingAppDomainFactory.CreateTextTemplatingAppDomainCallCount);
		}
		
		[Test]
		public void Dispose_DisposeCalledTwiceHostAfterProvideTemplatingAppDomainCalled_DisposesTemplatingAppDomainOnce()
		{
			CreateHost();
			host.ProvideTemplatingAppDomain("test");
			host.Dispose();
			
			textTemplatingAppDomain.IsDisposeCalled = false;
			host.Dispose();
			
			Assert.IsFalse(textTemplatingAppDomain.IsDisposeCalled);
		}
		
		[Test]
		public void ProvideTemplatingAppDomain_PassedContentName_HostApplicationBaseIsUsedAsAppDomainSetupApplicationBase()
		{
			string applicationBase = @"d:\sharpdevelop\addins\texttemplating";
			CreateHost(applicationBase);
			host.ProvideTemplatingAppDomain("test");
			
			string actualApplicationBase = textTemplatingAppDomainFactory.ApplicationBasePassedToCreateTextTemplatingAppDomain;
			Assert.AreEqual(applicationBase, actualApplicationBase);
		}
	}
}
