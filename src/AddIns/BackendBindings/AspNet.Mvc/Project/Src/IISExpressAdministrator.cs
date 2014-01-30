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
using ICSharpCode.SharpDevelop.Parser;

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
