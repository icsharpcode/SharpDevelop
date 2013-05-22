// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockFileService : IFileSystem
	{
		public string FileNamePassedToFileExists;
		public bool FileExistsReturnValue = true;
		
		public bool FileExists(string fileName)
		{
			FileNamePassedToFileExists = fileName;
			return FileExistsReturnValue;
		}
	}
}
