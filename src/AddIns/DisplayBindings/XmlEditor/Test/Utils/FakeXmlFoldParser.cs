// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class FakeXmlFoldParser : IXmlFoldParser
	{
		public List<FoldingRegion> Folds = new List<FoldingRegion>();
		public ITextBuffer TextBufferPassedToGetFolds;
		
		public IList<FoldingRegion> GetFolds(ITextBuffer textBuffer)
		{
			TextBufferPassedToGetFolds = textBuffer;
			return Folds;
		}
	}
}
