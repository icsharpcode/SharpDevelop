// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
