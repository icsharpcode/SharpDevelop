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
using System.IO;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceFileSystem : IServiceReferenceFileSystem
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
