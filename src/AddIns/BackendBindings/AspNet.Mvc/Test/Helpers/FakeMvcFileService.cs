// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcFileService : IMvcFileService
	{
		public string FileNamePassedToOpenFile;
		
		public void OpenFile(string fileName)
		{
			FileNamePassedToOpenFile = fileName;
		}
	}
}
