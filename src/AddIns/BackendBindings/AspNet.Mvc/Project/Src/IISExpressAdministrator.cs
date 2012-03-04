// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AspNet.Mvc
{
	public class IISExpressAdministrator : IISAdministrator
	{
		public IISExpressAdministrator(WebProjectProperties properties)
			: base(properties)
		{
		}
		
		public override bool IsIISInstalled()
		{
			return WebProjectService.IsIISExpressInstalled;
		}
		
		public override void CreateVirtualDirectory(WebProject project)
		{
			dynamic manager = CreateServerManager();
			dynamic site = manager.Sites.Add(project.Name, project.Directory, Properties.DevelopmentServerPort);
			string bindingInformation = String.Format("*:{0}:localhost", Properties.DevelopmentServerPort);
			site.Bindings[0].BindingInformation = bindingInformation;
			manager.CommitChanges();
			manager.Dispose();
		}
		
		protected override bool IsServerManagementVersionRequired(DomAssemblyName assemblyName)
		{
			return (assemblyName.Version.Major == 7) && (assemblyName.Version.Minor == 9);
		}
	}
}
