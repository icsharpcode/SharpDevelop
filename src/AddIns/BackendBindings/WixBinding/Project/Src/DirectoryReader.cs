// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Gets the files and directories in the specified path.
	/// </summary>
	public class DirectoryReader : IDirectoryReader
	{
		public string[] GetFiles(string path)
		{
			return Directory.GetFiles(path);
		}
			
		public string[] GetDirectories(string path)
		{
			return Directory.GetDirectories(path);
		}
		
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}
	}
}
