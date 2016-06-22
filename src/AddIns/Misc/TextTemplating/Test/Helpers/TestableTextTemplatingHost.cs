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

namespace TextTemplating.Tests.Helpers
{
	public class TestableTextTemplatingHost : TextTemplatingHost
	{
		public FakeTextTemplatingAppDomainFactory FakeTextTemplatingAppDomainFactory;
		public FakeTextTemplatingAssemblyResolver FakeTextTemplatingAssemblyResolver;
		public FakeTextTemplatingVariables FakeTextTemplatingVariables;
		public FakeServiceProvider FakeServiceProvider;
		public TextTemplatingHostContext HostContext;

		public TestableTextTemplatingHost(string applicationBase)
			: this(
				new FakeTextTemplatingAppDomainFactory(),
				new FakeTextTemplatingAssemblyResolver(),
				new FakeTextTemplatingVariables(),
				new FakeServiceProvider(),
				applicationBase)
		{
		}
		
		public TestableTextTemplatingHost(
			FakeTextTemplatingAppDomainFactory appDomainFactory,
			FakeTextTemplatingAssemblyResolver assemblyResolver,
			FakeTextTemplatingVariables textTemplatingVariables,
			FakeServiceProvider fakeServiceProvider,
			string applicationBase)
			: this(
				new TextTemplatingHostContext(
					appDomainFactory,
					assemblyResolver,
					textTemplatingVariables,
					fakeServiceProvider),
				applicationBase)
		{
			FakeTextTemplatingAppDomainFactory = appDomainFactory;
			FakeTextTemplatingAssemblyResolver = assemblyResolver;
			FakeTextTemplatingVariables = textTemplatingVariables;
			FakeServiceProvider = fakeServiceProvider;
		}
		
		public TestableTextTemplatingHost(TextTemplatingHostContext context, string applicationBase)
			: base(context, applicationBase)
		{
			HostContext = context;
		}

		public string CallResolveAssemblyReference(string assemblyReference)
		{
			return base.ResolveAssemblyReference(assemblyReference);
		}
		
		public string CallResolvePath(string path)
		{
			return base.ResolvePath(path);
		}
	}
}
