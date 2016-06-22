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
