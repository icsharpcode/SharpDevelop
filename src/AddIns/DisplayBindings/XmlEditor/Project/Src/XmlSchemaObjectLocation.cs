// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
