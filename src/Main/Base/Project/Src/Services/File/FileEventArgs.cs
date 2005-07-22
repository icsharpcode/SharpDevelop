// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	public delegate void FileEventHandler(object sender, FileEventArgs e);
	
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
}
