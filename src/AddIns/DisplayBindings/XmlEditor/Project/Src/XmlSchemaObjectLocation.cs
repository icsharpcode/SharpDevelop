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
using ICSharpCode.SharpDevelop;
using System.Xml.Schema;

namespace ICSharpCode.XmlEditor
{
	public class XmlSchemaObjectLocation
	{
		string fileName = String.Empty;
		int lineNumber = -1;
		int linePosition = -1;
		
		public XmlSchemaObjectLocation(XmlSchemaObject schemaObject)
		{
			if (schemaObject != null) {
				ReadLocation(schemaObject);
			}
		}
		
		void ReadLocation(XmlSchemaObject schemaObject)
		{
			if (schemaObject.SourceUri != null) {
				fileName = schemaObject.SourceUri.Replace("file:///", String.Empty);
			}
			lineNumber = schemaObject.LineNumber;
			linePosition = schemaObject.LinePosition;
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public int LineNumber {
			get { return lineNumber; }
		}
		
		public int LinePosition {
			get { return linePosition; }
		}
		
		public void JumpToFilePosition()
		{
			if (!String.IsNullOrEmpty(fileName)) {
				JumpToFilePosition(fileName, lineNumber, linePosition);
			}
		}
		
		protected virtual void JumpToFilePosition(string fileName, int line, int column)
		{
			FileService.JumpToFilePosition(fileName, line, column);
		}
	}
}
