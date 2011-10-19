// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
