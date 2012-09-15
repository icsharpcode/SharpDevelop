// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestFileService : IFileSystem
	{
		public bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}
	}
}
