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
