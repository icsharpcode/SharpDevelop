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
using System.Xml.Schema;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class DerivedXmlSchemaObjectLocation : XmlSchemaObjectLocation
	{
		bool jumpToFilePositionCalled;
		string jumpToFilePositionMethodFileNameParameter;
		int jumpToFilePositionMethodLineParameter = -1;
		int jumpToFilePositionMethodColumnParameter = -1;
		
		public DerivedXmlSchemaObjectLocation(XmlSchemaObject schemaObject)
			: base(schemaObject)
		{
		}
		
		public bool IsDerivedJumpToFilePositionMethodCalled {
			get { return jumpToFilePositionCalled; }
		}
		
		public string JumpToFilePositionMethodFileNameParameter {
			get { return jumpToFilePositionMethodFileNameParameter; }
		}
		
		public int JumpToFilePositionMethodLineParameter {
			get { return jumpToFilePositionMethodLineParameter; }
		}
		
		public int JumpToFilePositionMethodColumnParameter {
			get { return jumpToFilePositionMethodColumnParameter; }
		}
		
		public void CallJumpToFilePosition(string fileName, int line, int column)
		{
			JumpToFilePosition(fileName, line, column);
		}
		
		protected override void JumpToFilePosition(string fileName, int line, int column)
		{
			jumpToFilePositionCalled = true;
			jumpToFilePositionMethodFileNameParameter = fileName;
			jumpToFilePositionMethodLineParameter = line;
			jumpToFilePositionMethodColumnParameter = column;
		}
	}
}
