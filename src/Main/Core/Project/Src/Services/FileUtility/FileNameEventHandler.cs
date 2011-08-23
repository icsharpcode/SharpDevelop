// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// EventArgs with a file name.
	/// </summary>
	public class FileNameEventArgs : System.EventArgs
	{
		FileName fileName;
		
		public FileName FileName {
			get {
				return fileName;
			}
		}
		
		public FileNameEventArgs(FileName fileName)
		{
			this.fileName = fileName;
		}
		
		public FileNameEventArgs(string fileName)
		{
			this.fileName = FileName.Create(fileName);
		}
	}
}
