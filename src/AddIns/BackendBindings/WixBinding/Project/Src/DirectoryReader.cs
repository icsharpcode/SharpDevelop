// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
