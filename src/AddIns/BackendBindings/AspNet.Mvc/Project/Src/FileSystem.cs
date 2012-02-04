// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class FileSystem : IFileSystem
	{
		public string[] GetFiles(string path, string searchPattern)
		{
			return Directory.GetFiles(path, searchPattern);
		}
	}
}
