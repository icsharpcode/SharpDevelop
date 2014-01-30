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
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHostAppDomain : TextTemplatingAppDomain, IMvcTextTemplateHostAppDomain
	{
		public MvcTextTemplateHostAppDomain(string applicationBase)
			: base(applicationBase)
		{
			this.ApplicationBase = applicationBase;
		}
		
		public string ApplicationBase { get; private set; }
		
		public IMvcTextTemplateHost CreateMvcTextTemplateHost(
			ITextTemplatingAppDomainFactory appDomainFactory,
			ITextTemplatingAssemblyResolver assemblyResolver,
			string applicationBase)
		{
			Type type = typeof(MvcTextTemplateHost);
			var args = new object[] { appDomainFactory, assemblyResolver, applicationBase };
			
			return (MvcTextTemplateHost)AppDomain.CreateInstanceAndUnwrap(
				type.Assembly.FullName,
				type.FullName,
				false,
				BindingFlags.CreateInstance,
				null,
				args,
				null,
				null);
		}
	}
}
