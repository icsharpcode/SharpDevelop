// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestFileService : IFileSystem
	{
		void OpenFile(string fileName);
		void JumpToFilePosition(string fileName, int line, int column);
	}
}
