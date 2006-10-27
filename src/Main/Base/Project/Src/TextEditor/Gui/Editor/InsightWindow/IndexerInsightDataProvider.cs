// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class IndexerInsightDataProvider : MethodInsightDataProvider
	{
		/// <summary>
		/// Creates a IndexerInsightDataProvider looking at the caret position.
		/// </summary>
		public IndexerInsightDataProvider() {}
		
		/// <summary>
		/// Creates a IndexerInsightDataProvider looking at the specified position.
		/// </summary>
		public IndexerInsightDataProvider(int lookupOffset, bool setupOnlyOnce) : base(lookupOffset, setupOnlyOnce) {}
		
		protected override void SetupDataProvider(string fileName, IDocument document, ExpressionResult expressionResult, int caretLineNumber, int caretColumn)
		{
			ResolveResult result = ParserService.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, document.TextContent);
			if (result == null)
				return;
			IReturnType type = result.ResolvedType;
			if (type == null)
				return;
			foreach (IProperty i in type.GetProperties()) {
				if (i.IsIndexer) {
					methods.Add(i);
				}
			}
		}
	}
}
