// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockScriptingFileService : IScriptingFileService
	{
		CreateTextWriterInfo createTextWriterInfoPassedToCreateTextWriter;
		string tempFileName;
		TextWriter textWriter;
		string fileNameDeleted;
		
		public void SetTempFileName(string fileName)
		{
			this.tempFileName = fileName;
		}
		
		public string GetTempFileName()
		{
			return tempFileName;
		}
		
		public void SetTextWriter(TextWriter writer)
		{
			this.textWriter = writer;
		}
		
		public TextWriter CreateTextWriter(CreateTextWriterInfo textWriterInfo)
		{
			createTextWriterInfoPassedToCreateTextWriter = textWriterInfo;
			return textWriter;
		}
		
		public CreateTextWriterInfo CreateTextWriterInfoPassedToCreateTextWriter {
			get { return createTextWriterInfoPassedToCreateTextWriter; }
			set { createTextWriterInfoPassedToCreateTextWriter = value; }
		}
		
		public void DeleteFile(string fileName)
		{
			fileNameDeleted = fileName;
		}
		
		public string FileNameDeleted {
			get { return fileNameDeleted; }
			set { fileNameDeleted = value; }
		}
		
		public bool FileExists(string fileName)
		{
			return true;
		}
	}
}
