// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
