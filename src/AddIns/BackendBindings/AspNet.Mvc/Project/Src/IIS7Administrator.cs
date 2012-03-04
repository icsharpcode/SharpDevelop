// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class IIS7Administrator : IISAdministrator
	{
		public IIS7Administrator(WebProjectProperties properties)
			: base(properties)
		{
		}
		
		public override bool IsIISInstalled()
		{
			return WebProjectService.IsIISInstalled;
		}
		
		public override void CreateVirtualDirectory(WebProject project)
		{
			string name = "/" + project.Name;
			
			dynamic manager = CreateServerManager();
			
			if (manager.Sites[DEFAULT_WEB_SITE] != null) {
				if (manager.Sites[DEFAULT_WEB_SITE].Applications[name] == null) {
					manager.Sites[DEFAULT_WEB_SITE].Applications.Add(name, project.Directory);
					manager.CommitChanges();
				} else {
					ThrowApplicationExistsException();
				}
			} else {
				if (manager.Sites[0].Applications[name] == null) {
					manager.Sites[0].Applications.Add(name, project.Directory);
					manager.CommitChanges();
				} else {
					ThrowApplicationExistsException();
				}
			}
			manager.Dispose();
		}
	}
}
