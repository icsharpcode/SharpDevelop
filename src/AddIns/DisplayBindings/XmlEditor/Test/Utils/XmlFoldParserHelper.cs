// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public MockTextBuffer CreateTextBuffer(string xml)
		{
			return new MockTextBuffer(xml);
		}
		
		public IList<FoldingRegion> GetFolds(string xml)
		{
			MockTextBuffer textBuffer = new MockTextBuffer(xml);
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
