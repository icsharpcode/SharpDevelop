/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 27.07.2006
 * Time: 21:05
 */

using System;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// EventArgs for the <see cref="SharpDevelopHost.FileLoaded">SharpDevelopHost.FileLoaded</see>
	/// and <see cref="SharpDevelopHost.FileSaved">SharpDevelopHost.FileSaved</see> events.
	/// </summary>
	[Serializable]
	public class FileEventArgs : EventArgs
	{
		string fileName;
		
		/// <summary>
		/// Gets the file name.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// Creates a new instance of the FileEventArgs class.
		/// </summary>
		public FileEventArgs(string fileName)
		{
			this.fileName = fileName;
		}
	}
}
