// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.EnterpriseServices.Internal;

namespace ICSharpCode.AspNet.Mvc
{
	public class IIS6Administrator : IISAdministrator
	{
		const string IIS_WEB_LOCATION = "IIS://localhost/W3SVC/1/Root";

		public IIS6Administrator(WebProjectProperties properties)
			: base(properties)
		{
		}
		
		public override bool IsIISInstalled()
		{
			return WebProjectService.IsIISInstalled;
		}
		
		public override void CreateVirtualDirectory(WebProject project)
		{
			string error = null;
			var virtualRoot = new IISVirtualRoot();
			virtualRoot.Create(IIS_WEB_LOCATION,
				project.Directory,
				project.Name,
				out error);
			if (!String.IsNullOrEmpty(error)) {
				throw new ApplicationException(error);
			}
		}
	}
}
