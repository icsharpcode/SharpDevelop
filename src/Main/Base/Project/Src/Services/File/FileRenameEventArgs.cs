// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public delegate void FileRenameEventHandler(object sender, FileRenameEventArgs e);
	
	public class FileRenameEventArgs : EventArgs
	{
		bool   isDirectory;
		
		string sourceFile = null;
		string targetFile = null;
		
		public string SourceFile {
			get {
				return sourceFile;
			}
		}
		
		public string TargetFile {
			get {
				return targetFile;
			}
		}
		
		
		public bool IsDirectory {
			get {
				return isDirectory;
			}
		}
		
		public FileRenameEventArgs(string sourceFile, string targetFile, bool isDirectory)
		{
			this.sourceFile = sourceFile;
			this.targetFile = targetFile;
			this.isDirectory = isDirectory;
		}
	}
}
