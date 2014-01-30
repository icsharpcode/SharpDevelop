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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementFileService : IPackageManagementFileService
	{
		public void RemoveFile(string path)
		{
			InvokeIfRequired(() => FileService.RemoveFile(path, false));
		}
		
		void InvokeIfRequired(Action action)
		{
			SD.MainThread.InvokeIfRequired(action);
		}
		
		T InvokeIfRequired<T>(Func<T> callback)
		{
			return SD.MainThread.InvokeIfRequired(callback);
		}
		
		public void RemoveDirectory(string path)
		{
			InvokeIfRequired(() => FileService.RemoveFile(path, true));
		}
		
		public void OpenFile(string fileName)
		{
			InvokeIfRequired(() => FileService.OpenFile(fileName));
		}
		
		public IViewContent GetOpenFile(string fileName)
		{
			return InvokeIfRequired(() => FileService.GetOpenFile(fileName));
		}
		
		public void CopyFile(string oldFileName, string newFileName)
		{
			InvokeIfRequired(() => FileService.CopyFile(oldFileName, newFileName, isDirectory: false, overwrite: false));
		}
		
		public void SaveFile(IViewContent view)
		{
			if (SD.MainThread.InvokeRequired) {
				SD.MainThread.InvokeIfRequired(() => SaveFile(view));
			} else {
				if (view.IsDirty) {
					view.Files.ForEach(ICSharpCode.SharpDevelop.Commands.SaveFile.Save);
				}
			}
		}
		
		public bool FileExists(string fileName)
		{
			return InvokeIfRequired(() => File.Exists(fileName));
		}
		
		public string[] GetFiles(string path)
		{
			return InvokeIfRequired(() => Directory.GetFiles(path));
		}
		
		public string[] GetDirectories(string path)
		{
			return InvokeIfRequired(() => Directory.GetDirectories(path));
		}
		
		public void ParseFile(string fileName)
		{
			SD.ParserService.ParseFile(new FileName(fileName));
		}
		
		public ICompilation GetCompilationUnit(string fileName)
		{
			return SD.ParserService.GetCompilationForFile(new FileName(fileName));
		}
	}
}
