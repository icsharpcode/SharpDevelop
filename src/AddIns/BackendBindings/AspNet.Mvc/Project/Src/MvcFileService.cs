// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcFileService : IMvcFileService
	{
		public void OpenFile(string fileName)
		{
			FileService.OpenFile(fileName);
		}
	}
}
