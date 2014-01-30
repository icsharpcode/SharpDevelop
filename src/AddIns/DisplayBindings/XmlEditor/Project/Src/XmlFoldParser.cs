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
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XmlEditor
{
	public class XmlFoldParser : IXmlFoldParser
	{
		XmlEditorOptions options;
		XmlTextReader reader;
		List<FoldingRegion> folds = new List<FoldingRegion>();
		Stack<XmlElementFold> elementFoldStack;
		
		public XmlFoldParser(XmlEditorOptions options)
		{
			this.options = options;
		}
		
		public XmlFoldParser()
			: this(XmlEditorService.XmlEditorOptions)
		{
		}
		
		public IList<FoldingRegion> GetFolds(ITextSource textSource)
		{
			try {
				GetFolds(textSource.CreateReader());
				return folds;
			} catch (XmlException) {
			}
			return null;
		}
		
		void GetFolds(TextReader textReader)
		{
			folds = new List<FoldingRegion>();
			elementFoldStack = new Stack<XmlElementFold>();
			
			CreateXmlTextReaderWithNoNamespaceSupport(textReader);
			
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						AddElementFoldToStackIfNotEmptyElement();
						break;
						
					case XmlNodeType.EndElement:
						CreateElementFoldingRegionIfNotSingleLine();
						break;
						
					case XmlNodeType.Comment:
						CreateCommentFoldingRegionIfNotSingleLine();
						break;
				}
			}
			folds.Sort(CompareFoldingRegion);
		}
		
		void CreateXmlTextReaderWithNoNamespaceSupport(TextReader textReader)
		{
			reader = new XmlTextReader(textReader);
			reader.XmlResolver = null; // prevent XmlTextReader from loading external DTDs
			reader.Namespaces = false;
		}
		
		void AddElementFoldToStackIfNotEmptyElement()
		{
			if (!reader.IsEmptyElement) {
				XmlElementFold fold = new XmlElementFold();
				fold.ReadStart(reader);
				elementFoldStack.Push(fold);
			}
		}
		
		void CreateElementFoldingRegionIfNotSingleLine()
		{
			XmlElementFold fold = elementFoldStack.Pop();
			fold.ReadEnd(reader);
			if (!fold.IsSingleLine) {
				FoldingRegion foldingRegion = CreateFoldingRegion(fold);
				folds.Add(foldingRegion);
			}
		}
		
		FoldingRegion CreateFoldingRegion(XmlElementFold fold)
		{
			if (options.ShowAttributesWhenFolded) {
				return fold.CreateFoldingRegionWithAttributes();
			}
			return fold.CreateFoldingRegion();
		}
		
		/// <summary>
		/// Creates a comment fold if the comment spans more than one line.
		/// </summary>
		/// <remarks>The text displayed when the comment is folded is the first
		/// line of the comment.</remarks>
		void CreateCommentFoldingRegionIfNotSingleLine()
		{
			XmlCommentFold fold = new XmlCommentFold(reader);
			if (!fold.IsSingleLine) {
				folds.Add(fold.CreateFoldingRegion());
			}
		}
		
		int CompareFoldingRegion(FoldingRegion lhs, FoldingRegion rhs)
		{
			int compareBeginLine = lhs.Region.BeginLine.CompareTo(rhs.Region.BeginLine);
			if (compareBeginLine == 0) {
				return lhs.Region.BeginColumn.CompareTo(rhs.Region.BeginColumn);
			}
			return compareBeginLine;
		}
	}
}
