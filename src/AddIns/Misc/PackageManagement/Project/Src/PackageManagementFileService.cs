// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public IViewContent GetOpenFile(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => GetOpenFile(fileName));
			} else {
				return FileService.GetOpenFile(fileName);
			}
		}
		
		public void CopyFile(string oldFileName, string newFileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<string, string> action = CopyFile;
				WorkbenchSingleton.SafeThreadAsyncCall<string, string>(action, oldFileName, newFileName);
			} else {
				FileService.CopyFile(oldFileName, newFileName, isDirectory: false, overwrite: false);
			}
		}
		
		public bool FileExists(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => FileExists(fileName));
			} else {
				return File.Exists(fileName);
			}
		}
		
		public string[] GetFiles(string path)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => GetFiles(path));
			} else {
				return Directory.GetFiles(path);
			}
		}
		
		public string[] GetDirectories(string path)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction(() => GetDirectories(path));
			} else {
				return Directory.GetDirectories(path);
			}
		}
		
		public void ParseFile(string fileName)
		{
			ParserService.ParseFile(fileName);
		}
		
		public ICompilationUnit GetCompilationUnit(string fileName)
		{
			return ParserService.GetExistingParseInformation(fileName).CompilationUnit;
		}
	}
}
