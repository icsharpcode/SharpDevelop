// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHostAppDomainFactory : IMvcTextTemplateHostAppDomainFactory
	{
		public IMvcTextTemplateHostAppDomain CreateAppDomain()
		{
			string applicationBase = GetAssemblyBaseLocation();
			return new MvcTextTemplateHostAppDomain(applicationBase);
		}
		
		string GetAssemblyBaseLocation()
		{
			string location = GetType().Assembly.Location;
			return Path.GetDirectoryName(location);
		}
	}
}
