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
		ITextTemplatingVariables templatingVariables;
		string applicationBase;
		
		public TextTemplatingHost(
			ITextTemplatingAppDomainFactory appDomainFactory,
			ITextTemplatingAssemblyResolver assemblyResolver,
			ITextTemplatingVariables templatingVariables,
			string applicationBase)
		{
			this.appDomainFactory = appDomainFactory;
			this.assemblyResolver = assemblyResolver;
			this.templatingVariables = templatingVariables;
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
		
		protected override string ResolvePath(string path)
		{
			path = ExpandPath(path);
			return base.ResolvePath(path);
		}
		
		string ExpandPath(string path)
		{
			return templatingVariables.Expand(path);
		}
	}
}
