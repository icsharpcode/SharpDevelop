// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;
using NUnit.Framework;

namespace TextTemplating.Tests.Helpers
{
	[TestFixture]
	public class TextTemplatingHostContextTests
	{
		TextTemplatingHostContext hostContext;
		FakeTextTemplatingAssemblyResolver fakeAssemblyResolver;
		
		void CreateHostContext()
		{
			var testableHost = new TestableTextTemplatingHost("Test");
			hostContext = testableHost.HostContext;
			fakeAssemblyResolver = testableHost.FakeTextTemplatingAssemblyResolver;
		}
		
		[Test]
		public void Dispose_DisposeCalled_DisposesAssemblyResolver()
		{
			CreateHostContext();
			hostContext.Dispose();
			
			Assert.IsTrue(fakeAssemblyResolver.IsDisposeCalled);
		}
	}
}
