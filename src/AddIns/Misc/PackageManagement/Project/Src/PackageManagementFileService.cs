// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
