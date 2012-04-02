// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestFileService : IUnitTestFileService
	{
		public void OpenFile(string fileName)
		{
			FileService.OpenFile(fileName);
		}
		
		public void JumpToFilePosition(string fileName, int line, int column)
		{
			FileService.JumpToFilePosition(fileName, line, column);
		}
		
		public bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}
	}
}
