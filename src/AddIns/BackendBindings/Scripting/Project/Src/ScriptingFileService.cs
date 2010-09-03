// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.Scripting
{
	public class ScriptingFileService : IScriptingFileService
	{
		public string GetTempFileName()
		{
			return Path.GetTempFileName();
		}
		
		public TextWriter CreateTextWriter(CreateTextWriterInfo createTextWriterInfo)
		{
			return createTextWriterInfo.CreateTextWriter();
		}
		
		public void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}
		
		public bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}
	}
}
