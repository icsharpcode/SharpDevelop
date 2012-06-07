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
		
		public string CreateTempFile(string text)
		{
			string folder = Path.GetTempPath();
			string fileName = Path.Combine(folder, "app.config");
			File.WriteAllText(fileName, text);
			return fileName;
		}
		
		public string ReadAllFileText(string fileName)
		{
			return File.ReadAllText(fileName);
		}
		
		public void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}
		
		public void WriteAllText(string fileName, string text)
		{
			File.WriteAllText(fileName, text);
		}
	}
}
