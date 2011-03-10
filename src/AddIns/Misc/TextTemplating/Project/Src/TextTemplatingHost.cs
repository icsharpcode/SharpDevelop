// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Mono.TextTemplating;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingHost : TemplateGenerator, ITextTemplatingHost
	{
		ITextTemplatingAppDomainFactory appDomainFactory;
		ITextTemplatingAppDomain templatingAppDomain;
		ITextTemplatingAssemblyResolver assemblyResolver;
		string applicationBase;
		
		public TextTemplatingHost(
			ITextTemplatingAppDomainFactory appDomainFactory,
			ITextTemplatingAssemblyResolver assemblyResolver,
			string applicationBase)
		{
			this.appDomainFactory = appDomainFactory;
			this.assemblyResolver = assemblyResolver;
			this.applicationBase = applicationBase;
		}
		
		public void Dispose()
		{
			if (templatingAppDomain != null) {
				templatingAppDomain.Dispose();
				templatingAppDomain = null;
			}
		}
		
		public override AppDomain ProvideTemplatingAppDomain(string content)
		{
			if (templatingAppDomain == null) {
				CreateAppDomain();
			}
			return templatingAppDomain.AppDomain;
		}

		void CreateAppDomain()
		{
			templatingAppDomain = appDomainFactory.CreateTextTemplatingAppDomain(applicationBase);
		}
		
		protected override string ResolveAssemblyReference(string assemblyReference)
		{
			return assemblyResolver.Resolve(assemblyReference);
		}
	}
}
