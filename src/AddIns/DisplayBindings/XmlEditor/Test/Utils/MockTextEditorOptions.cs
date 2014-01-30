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
using ICSharpCode.SharpDevelop.Editor;

namespace XmlEditor.Tests.Utils
{
	public class MockTextEditorOptions : ITextEditorOptions
	{
		public MockTextEditorOptions()
		{
			IndentationString = "\t";
		}
		
		public string IndentationString { get; set; }
		
		public bool AutoInsertBlockEnd {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool ConvertTabsToSpaces {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int IndentationSize {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int VerticalRulerColumn {
			get {
				throw new NotImplementedException();
			}
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add { } remove { } }
		
		public bool UnderlineErrors {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string FontFamily {
			get {
				throw new NotImplementedException();
			}
		}
		
		public double FontSize {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
