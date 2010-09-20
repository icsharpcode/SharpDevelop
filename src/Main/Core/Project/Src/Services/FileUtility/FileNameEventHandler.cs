// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
