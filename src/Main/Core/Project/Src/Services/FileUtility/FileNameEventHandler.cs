/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 23.11.2004
 * Time: 12:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
