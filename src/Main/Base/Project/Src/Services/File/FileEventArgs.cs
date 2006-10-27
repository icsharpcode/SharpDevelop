// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop
{
	public class FileEventArgs : EventArgs
	{
		string fileName   = null;
		
		bool   isDirectory;
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public bool IsDirectory {
			get {
				return isDirectory;
			}
		}
		
		public FileEventArgs(string fileName, bool isDirectory)
		{
			this.fileName = fileName;
			this.isDirectory = isDirectory;
		}
	}
	
	public class FileCancelEventArgs : FileEventArgs
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
		
		public FileCancelEventArgs(string fileName, bool isDirectory) : base(fileName, isDirectory)
		{
		}
	}
}
