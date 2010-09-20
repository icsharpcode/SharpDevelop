// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.UnitTesting;

namespace ICSharpCode.Scripting
{
	public interface IScriptingFileService : IFileSystem
	{
		string GetTempFileName();
		TextWriter CreateTextWriter(CreateTextWriterInfo createTextWriterInfo);
		void DeleteFile(string fileName);
	}
}
