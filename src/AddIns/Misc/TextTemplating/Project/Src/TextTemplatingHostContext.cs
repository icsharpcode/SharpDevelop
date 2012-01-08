// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingHostContext : IDisposable
	{
		ITextTemplatingAppDomainFactory appDomainFactory;
		ITextTemplatingAssemblyResolver assemblyResolver;
		ITextTemplatingVariables templatingVariables;
		IServiceProvider serviceProvider;
		
		public TextTemplatingHostContext(IProject project)
			: this(
				new TextTemplatingAppDomainFactory(),
				new TextTemplatingAssemblyResolver(project),
				new TextTemplatingVariables(),
				new TextTemplatingServiceProvider())
		{
		}
		
		public TextTemplatingHostContext(
			ITextTemplatingAppDomainFactory appDomainFactory,
			ITextTemplatingAssemblyResolver assemblyResolver,
			ITextTemplatingVariables templatingVariables,
			IServiceProvider serviceProvider)
		{
			this.appDomainFactory = appDomainFactory;
			this.assemblyResolver = assemblyResolver;
			this.templatingVariables = templatingVariables;
			this.serviceProvider = serviceProvider;
		}
		
		public object GetService(Type serviceType)
		{
			return serviceProvider.GetService(serviceType);
		}
		
		public string ExpandTemplateVariables(string name)
		{
			return templatingVariables.ExpandVariables(name);
		}
		
		public ITextTemplatingAppDomain CreateTextTemplatingAppDomain(string applicationBase)
		{
			return appDomainFactory.CreateTextTemplatingAppDomain(applicationBase);
		}
		
		public string ResolveAssemblyReference(string assemblyReference)
		{
			return assemblyResolver.ResolvePath(assemblyReference);
		}
		
		public void Dispose()
		{
			assemblyResolver.Dispose();
		}
	}
}
