// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceFileSystem : IFileSystem
	{
		public void CreateDirectoryIfMissing(string path)
		{
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}
	}
}
