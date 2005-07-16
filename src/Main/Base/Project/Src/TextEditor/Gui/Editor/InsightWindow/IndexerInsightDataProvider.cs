// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃƒÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
	public class IndexerInsightDataProvider : IInsightDataProvider
	{
		string              fileName = null;
		IDocument document = null;
		TextArea textArea;
		List<IIndexer>   methods  = new List<IIndexer>();
		
		public int InsightDataCount {
			get {
				return methods.Count;
			}
		}
		
		public string GetInsightData(int number)
		{
			IIndexer method = methods[number];
			IAmbience conv = AmbienceService.CurrentAmbience;
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string documentation = method.Documentation;
			return conv.Convert(method) +
				"\n" +
				CodeCompletionData.GetDocumentation(documentation); // new (by G.B.)
		}
		
		int initialOffset;
		public void SetupDataProvider(string fileName, TextArea textArea)
		{
			this.fileName = fileName;
			this.document = textArea.Document;
			this.textArea = textArea;
			initialOffset = textArea.Caret.Offset;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			string word = expressionFinder == null ? TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset) : expressionFinder.FindExpression(textArea.Document.TextContent, textArea.Caret.Offset - 1);
			if (word == null) // word can be null when cursor is in string/comment
				return;
			word = word.Trim();
			
			// the parser works with 1 based coordinates
			int caretLineNumber = document.GetLineNumberForOffset(textArea.Caret.Offset) + 1;
			int caretColumn     = textArea.Caret.Offset - document.GetLineSegment(caretLineNumber).Offset + 1;
			
			ResolveResult result = ParserService.Resolve(word, caretLineNumber, caretColumn, fileName, document.TextContent);
			if (result == null)
				return;
			IReturnType type = result.ResolvedType;
			if (type == null)
				return;
			/*
			IProjectContent projectContent = ParserService.CurrentProjectContent;
			if (projectContent == null)
				return;
			IClass c = projectContent.GetClass(type.FullyQualifiedName);
			if (c == null)
				return;
			bool canViewProtected = c.IsTypeInInheritanceTree(result.CallingClass);*/
			foreach (IIndexer i in type.GetIndexers()) {
				//if (i.IsAccessible(result.CallingClass, canViewProtected)) {
					methods.Add(i);
				//}
			}
		}
		
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
	}
}
