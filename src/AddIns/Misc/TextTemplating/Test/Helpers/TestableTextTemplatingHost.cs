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

		public TestableTextTemplatingHost(string applicationBase)
			: this(
				new FakeTextTemplatingAppDomainFactory(), 
				new FakeTextTemplatingAssemblyResolver(), 
				applicationBase)
		{
		}
		
		public TestableTextTemplatingHost(
			FakeTextTemplatingAppDomainFactory appDomainFactory,
			FakeTextTemplatingAssemblyResolver assemblyResolver,
			string applicationBase)
			: base(appDomainFactory, assemblyResolver, applicationBase)
		{
			FakeTextTemplatingAppDomainFactory = appDomainFactory;
			FakeTextTemplatingAssemblyResolver = assemblyResolver;
		}

		public string CallResolveAssemblyReference(string assemblyReference)
		{
			return base.ResolveAssemblyReference(assemblyReference);
		}
	}
}
