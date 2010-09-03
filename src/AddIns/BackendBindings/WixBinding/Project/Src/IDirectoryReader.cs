// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
