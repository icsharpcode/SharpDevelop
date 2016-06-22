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
