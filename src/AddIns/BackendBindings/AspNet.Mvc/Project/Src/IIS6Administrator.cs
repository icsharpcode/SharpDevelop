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
