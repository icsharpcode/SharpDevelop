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
		TestableTextTemplatingHost host;
		FakeTextTemplatingAppDomainFactory fakeTextTemplatingAppDomainFactory;
		FakeTextTemplatingAppDomain fakeTextTemplatingAppDomain;
		FakeTextTemplatingAssemblyResolver fakeAssemblyResolver;
		FakeTextTemplatingVariables fakeTextTemplatingVariables;
		FakeServiceProvider fakeServiceProvider;
		TextTemplatingHostContext hostContext;
		
		void CreateHost()
		{
			CreateHost(String.Empty);
		}
		
		void CreateHost(string applicationBase)
		{
			host = new TestableTextTemplatingHost(applicationBase);
			fakeTextTemplatingAppDomainFactory = host.FakeTextTemplatingAppDomainFactory;
			fakeTextTemplatingAppDomain = fakeTextTemplatingAppDomainFactory.FakeTextTemplatingAppDomain;
			fakeAssemblyResolver = host.FakeTextTemplatingAssemblyResolver;
			fakeTextTemplatingVariables = host.FakeTextTemplatingVariables;
			fakeServiceProvider = host.FakeServiceProvider;
			hostContext = host.HostContext;
		}
		
		void AddTemplateVariableValue(string variableName, string variableValue)
		{
			fakeTextTemplatingVariables.AddVariable(variableName, variableValue);
		}
		
		[Test]
		public void ProvideTemplatingAppDomain_PassedContentName_ReturnsDomainFromTextTemplatingAppDomainFactory()
		{
			CreateHost();
			AppDomain expectedAppDomain = AppDomain.CreateDomain("TextTemplatingHostTests");
			fakeTextTemplatingAppDomain.AppDomain = expectedAppDomain;
			
			AppDomain actualAppDomain = host.ProvideTemplatingAppDomain("test");
			
			Assert.AreEqual(expectedAppDomain, actualAppDomain);
		}
		
		[Test]
		public void Dispose_DisposingHostAfterProvideTemplatingAppDomainCalled_DisposesTemplatingAppDomain()
		{
			CreateHost();
			host.ProvideTemplatingAppDomain("test");
			host.Dispose();
			
			Assert.IsTrue(fakeTextTemplatingAppDomain.IsDisposeCalled);
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
			
			Assert.AreEqual(1, fakeTextTemplatingAppDomainFactory.CreateTextTemplatingAppDomainCallCount);
		}
		
		[Test]
		public void Dispose_DisposeCalledTwiceHostAfterProvideTemplatingAppDomainCalled_DisposesTemplatingAppDomainOnce()
		{
			CreateHost();
			host.ProvideTemplatingAppDomain("test");
			host.Dispose();
			
			fakeTextTemplatingAppDomain.IsDisposeCalled = false;
			host.Dispose();
			
			Assert.IsFalse(fakeTextTemplatingAppDomain.IsDisposeCalled);
		}
		
		[Test]
		public void ProvideTemplatingAppDomain_PassedContentName_HostApplicationBaseIsUsedAsAppDomainSetupApplicationBase()
		{
			string applicationBase = @"d:\sharpdevelop\addins\texttemplating";
			CreateHost(applicationBase);
			host.ProvideTemplatingAppDomain("test");
			
			string actualApplicationBase = fakeTextTemplatingAppDomainFactory.ApplicationBasePassedToCreateTextTemplatingAppDomain;
			Assert.AreEqual(applicationBase, actualApplicationBase);
		}
		
		[Test]
		public void ResolveAssemblyReference_PassedMyAssemblyReference_CallsTextTemplatingAssemblyResolver()
		{
			CreateHost();
			host.CallResolveAssemblyReference("MyReference");
			
			Assert.AreEqual("MyReference", fakeAssemblyResolver.AssembyReferencePassedToResolvePath);
		}
		
		[Test]
		public void ResolveAssemblyReference_PassedMyAssemblyReference_ReturnsFileNameReturnedFromAssemblyResolverResolveMethod()
		{
			CreateHost();
			fakeAssemblyResolver.ResolvePathReturnValue = @"d:\projects\references\MyReference.dll";
			string result = host.CallResolveAssemblyReference("MyReference");
			
			Assert.AreEqual(@"d:\projects\references\MyReference.dll", result);
		}
		
		[Test]
		public void ResolvePath_PathContainsSolutionDirProperty_SolutionDirExpanded()
		{
			CreateHost();
			AddTemplateVariableValue("SolutionDir", @"d:\projects\MySolution\");
			string path = host.CallResolvePath("$(SolutionDir)");
			
			Assert.AreEqual(@"d:\projects\MySolution\", path);
		}
		
		[Test]
		public void GetService_HostPassedFakeServiceProvider_ReturnsServiceFromFakeServiceProvider()
		{
			CreateHost();
			var expectedService = new StringWriter();
			fakeServiceProvider.AddService(typeof(StringWriter), expectedService);
			
			var hostServiceProvider = host as IServiceProvider;
			StringWriter service = hostServiceProvider.GetService(typeof(StringWriter)) as StringWriter;
			
			Assert.AreEqual(expectedService, service);
		}
		
		[Test]
		public void Dispose_DisposeCalledAfterProvideTemplatingAppDomainCalled_DisposesAssemblyResolver()
		{
			CreateHost();
			host.ProvideTemplatingAppDomain("test");
			host.Dispose();
			
			Assert.IsTrue(fakeAssemblyResolver.IsDisposeCalled);
		}
	}
}
