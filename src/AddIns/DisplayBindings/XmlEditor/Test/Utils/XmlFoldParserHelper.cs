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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class XmlFoldParserHelper
	{
		public XmlEditorOptions Options = new XmlEditorOptions(new Properties());
		public XmlFoldParser Parser;
		public IList<FoldingRegion> Folds;
		
		public XmlFoldParser CreateParser()
		{
			Parser = new XmlFoldParser(Options);
			return Parser;
		}
		
		public ITextSource CreateTextBuffer(string xml)
		{
			return new StringTextSource(xml);
		}
		
		public IList<FoldingRegion> GetFolds(string xml)
		{
			ITextSource textBuffer = new StringTextSource(xml);
			Folds = Parser.GetFolds(textBuffer);
			return Folds;
		}
		
		public FoldingRegion FirstFold {
			get { return Folds[0]; }
		}
		
		public string GetFirstFoldName(IList<FoldingRegion> folds)
		{
			return FirstFold.Name;
		}
		
		public string GetFirstFoldName()
		{
			return FirstFold.Name;
		}
		
		public DomRegion GetFirstFoldRegion()
		{
			return FirstFold.Region;
		}
		
		public FoldingRegion SecondFold {
			get { return Folds[1]; }
		}
		
		public DomRegion GetSecondFoldRegion()
		{
			return SecondFold.Region;
		}
	}
}
