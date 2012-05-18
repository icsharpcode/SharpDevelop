// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IFileSystem
	{
		void CreateDirectoryIfMissing(string path);
		string CreateTempFile(string text);
		string ReadAllFileText(string fileName);
		void DeleteFile(string fileName);
		void WriteAllText(string fileName, string text);
	}
}
