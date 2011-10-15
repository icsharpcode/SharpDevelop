// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using Mono.TextTemplating;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingHost : TemplateGenerator, ITextTemplatingHost, IServiceProvider
	{
		ITextTemplatingAppDomain templatingAppDomain;
		TextTemplatingHostContext context;
		string applicationBase;
		
		public TextTemplatingHost(TextTemplatingHostContext context, string applicationBase)
		{
			this.context = context;
			this.applicationBase = applicationBase;
		}
		
		public void Dispose()
		{
			if (templatingAppDomain != null) {
				templatingAppDomain.Dispose();
				templatingAppDomain = null;
			}
			context.Dispose();
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
			templatingAppDomain = context.CreateTextTemplatingAppDomain(applicationBase);
		}
		
		protected override string ResolveAssemblyReference(string assemblyReference)
		{
			return context.ResolveAssemblyReference(assemblyReference);
		}
		
		protected override string ResolvePath(string path)
		{
			path = ExpandPath(path);
			return base.ResolvePath(path);
		}
		
		string ExpandPath(string path)
		{
			return context.ExpandTemplateVariables(path);
		}
		
		object IServiceProvider.GetService(Type serviceType)
		{
			return context.GetService(serviceType);
		}
	}
}
