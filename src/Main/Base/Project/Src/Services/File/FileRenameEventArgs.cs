// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop
{
	public class FileRenamingEventArgs : FileRenameEventArgs
	{
		bool cancel;

		public bool Cancel {
			get {
				return cancel;
			}
			set {
				cancel = value;
			}
		}
		
		bool operationAlreadyDone;

		public bool OperationAlreadyDone {
			get {
				return operationAlreadyDone;
			}
			set {
				operationAlreadyDone = value;
			}
		}
		
		public FileRenamingEventArgs(string sourceFile, string targetFile, bool isDirectory)
			: base(sourceFile, targetFile, isDirectory)
		{
		}
	}
	
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
