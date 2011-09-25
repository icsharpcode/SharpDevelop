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

		public TestableTextTemplatingHost(string applicationBase)
			: this(
				new FakeTextTemplatingAppDomainFactory(), 
				new FakeTextTemplatingAssemblyResolver(), 
				new FakeTextTemplatingVariables(),
				applicationBase)
		{
		}
		
		public TestableTextTemplatingHost(
			FakeTextTemplatingAppDomainFactory appDomainFactory,
			FakeTextTemplatingAssemblyResolver assemblyResolver,
			FakeTextTemplatingVariables textTemplatingVariables,
			string applicationBase)
			: base(appDomainFactory, assemblyResolver, textTemplatingVariables, applicationBase)
		{
			FakeTextTemplatingAppDomainFactory = appDomainFactory;
			FakeTextTemplatingAssemblyResolver = assemblyResolver;
			FakeTextTemplatingVariables = textTemplatingVariables;
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
