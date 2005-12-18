// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	public delegate void FileNameEventHandler(object sender, FileNameEventArgs e);
	
	/// <summary>
	/// Description of FileEventHandler.
	/// </summary>
	public class FileNameEventArgs : System.EventArgs
	{
		string fileName;
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public FileNameEventArgs(string fileName)
		{
			this.fileName = fileName;
		}
	}
}
