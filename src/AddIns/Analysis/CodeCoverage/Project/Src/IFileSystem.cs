// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.CodeCoverage
{
	public interface IFileSystem : ICSharpCode.UnitTesting.IFileSystem
	{
		void DeleteFile(string path);
		
		bool DirectoryExists(string path);
		void CreateDirectory(string path);
		
		TextReader CreateTextReader(string path);
	}
}
