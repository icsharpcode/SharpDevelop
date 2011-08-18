// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHostFactory : IMvcTextTemplateHostFactory
	{
		public IMvcTextTemplateHost CreateMvcTextTemplateHost(IMvcProject mvcProject)
		{
			var appDomainFactory = new TextTemplatingAppDomainFactory();
			string applicationBase = GetAssemblyBaseLocation();
			var assemblyResolver = new TextTemplatingAssemblyResolver(mvcProject.Project);
			return new MvcTextTemplateHost(appDomainFactory, assemblyResolver, applicationBase);
		}
		
		string GetAssemblyBaseLocation()
		{
			string location = GetType().Assembly.Location;
			return Path.GetDirectoryName(location);
		}
	}
}
