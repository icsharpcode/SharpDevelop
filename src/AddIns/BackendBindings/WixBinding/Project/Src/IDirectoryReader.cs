// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Gets the files and directories for a specified path. This is used
	/// to hide the file system from the WixPackageFilesEditor class when
	/// testing.
	/// </summary>
	public interface IDirectoryReader
	{
		/// <summary>
		/// Gets the files in the specified path.
		/// </summary>
		string[] GetFiles(string path);
		
		/// <summary>
		/// Gets the directories in the specified path.
		/// </summary>
		string[] GetDirectories(string path);
		
		/// <summary>
		/// Checks whether the specified directory exists.
		/// </summary>
		bool DirectoryExists(string path);
	}
}
