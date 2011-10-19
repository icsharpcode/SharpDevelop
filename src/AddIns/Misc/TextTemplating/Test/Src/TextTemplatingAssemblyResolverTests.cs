// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingAssemblyResolverTests
	{
		TextTemplatingAssemblyResolver resolver;
		FakeTextTemplatingAssemblyPathResolver fakeAssemblyPathResolver;
		FakeTextTemplatingHostAppDomainAssemblyResolver fakeHostAppDomainAssemblyResolver;
		
		void CreateResolver()
		{
			fakeAssemblyPathResolver = new FakeTextTemplatingAssemblyPathResolver();
			fakeHostAppDomainAssemblyResolver = new FakeTextTemplatingHostAppDomainAssemblyResolver();
			resolver = new TextTemplatingAssemblyResolver(
				fakeAssemblyPathResolver,
				fakeHostAppDomainAssemblyResolver);
		}
		
		[Test]
		public void ResolvePath_PathPassed_PathPassedToAssemblyPathResolver()
		{
			CreateResolver();
			resolver.ResolvePath("Test");
			
			Assert.AreEqual("Test", fakeAssemblyPathResolver.AssemblyReferencePassedToResolvePath);
		}
		
		[Test]
		public void ResolvePath_PathPassed_ReturnAssemblyPathFromAssemblyPathResolver()
		{
			CreateResolver();
			string expectedPath = @"d:\test\MyAssembly.dll";
			fakeAssemblyPathResolver.ResolvePathReturnValue = expectedPath;
			string path = resolver.ResolvePath("Test");
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void Dispose_DisposeCalled_DisposesHostAppDomainAssemblyResolver()
		{
			CreateResolver();
			resolver.Dispose();
			
			Assert.IsTrue(fakeHostAppDomainAssemblyResolver.IsDisposeCalled);
		}
	}
}
