// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.InsightWindow;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;


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
		
		/*
		public bool CaretOffsetChanged()
		{
			bool closeDataProvider = textArea.Caret.Offset <= initialOffset;
			
			if (!closeDataProvider) {
				bool insideChar   = false;
				bool insideString = false;
				for (int offset = initialOffset; offset < Math.Min(textArea.Caret.Offset, document.TextLength); ++offset) {
					char ch = document.GetCharAt(offset);
					switch (ch) {
						case '\'':
							insideChar = !insideChar;
							break;
						case '"':
							insideString = !insideString;
							break;
						case ']':
						case '}':
						case '{':
						case ';':
							if (!(insideChar || insideString)) {
								return true;
							}
							break;
					}
				}
			}
			
			return closeDataProvider;
		}
		
		public bool CharTyped()
		{
			int offset = textArea.Caret.Offset - 1;
			if (offset >= 0) {
				return document.GetCharAt(offset) == ']';
			}
			return false;
		}
		 */
	}
}
