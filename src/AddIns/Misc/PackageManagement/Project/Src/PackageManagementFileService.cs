// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementFileService : IPackageManagementFileService
	{
		public void RemoveFile(string path)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<string> action = RemoveFile;
				WorkbenchSingleton.SafeThreadCall<string>(action, path);
			} else {
				FileService.RemoveFile(path, false);
			}
		}
		
		public void RemoveDirectory(string path)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<string> action = RemoveDirectory;
				WorkbenchSingleton.SafeThreadCall<string>(action, path);
			} else {
				FileService.RemoveFile(path, true);
			}
		}
		
		public void OpenFile(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<string> action = OpenFile;
				WorkbenchSingleton.SafeThreadAsyncCall<string>(action, fileName);
			} else {
				FileService.OpenFile(fileName);
			}
		}
	}
}
