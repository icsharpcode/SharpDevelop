// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	public class MockPythonFileService : IPythonFileService
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
	}
}
