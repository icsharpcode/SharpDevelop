// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public IList<FoldingRegion> GetFolds(ITextBuffer textBuffer)
		{
			try {
				GetFolds(textBuffer.CreateReader());
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
