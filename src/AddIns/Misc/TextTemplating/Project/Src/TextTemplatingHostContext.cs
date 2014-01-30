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
