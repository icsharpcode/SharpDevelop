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
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.InsightWindow;


namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class MethodInsightDataProvider : IInsightDataProvider
	{
		string              fileName = null;
		IDocument document = null;
		TextArea textArea  = null;
		List<IMethod>    methods  = new List<IMethod>();
		
		int caretLineNumber;
		int caretColumn;
		
		public int InsightDataCount {
			get {
				return methods.Count;
			}
		}
		
		public string GetInsightData(int number)
		{
			IMethod method = methods[number];
			IAmbience conv = AmbienceService.CurrentAmbience;
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string documentation = ParserService.CurrentProjectContent.GetXmlDocumentation(method.DocumentationTag);
			return conv.Convert(method) +
				"\n" +
				CodeCompletionData.GetDocumentation(documentation); // new (by G.B.)
		}
		
		int initialOffset;
		public void SetupDataProvider(string fileName, TextArea textArea)
		{
			IDocument document = textArea.Document;
			this.fileName = fileName;
			this.document = document;
			this.textArea = textArea;
			initialOffset = textArea.Caret.Offset;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			string word = expressionFinder == null ? TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset) : expressionFinder.FindExpression(textArea.Document.TextContent, textArea.Caret.Offset - 1);
			word = word.Trim();
			
			// the parser works with 1 based coordinates
			caretLineNumber      = document.GetLineNumberForOffset(textArea.Caret.Offset) + 1;
			caretColumn          = textArea.Caret.Offset - document.GetLineSegment(caretLineNumber).Offset + 1;
			
			bool constructorInsight = false;
			if (word.ToLower().StartsWith("new ")) {
				constructorInsight = true;
				word = word.Substring(4);
			}
			ResolveResult results = ParserService.Resolve(word, caretLineNumber, caretColumn, fileName, document.TextContent);
			if (constructorInsight) {
				TypeResolveResult result = results as TypeResolveResult;
				if (result == null)
					return;
				IClass c = result.ResolvedClass;
				bool canViewProtected = c.IsTypeInInheritanceTree(result.CallingClass);
				foreach (IMethod method in c.Methods) {
					if (method.IsConstructor) {
						if (method.IsAccessible(result.CallingClass, canViewProtected)) {
							methods.Add(method);
						}
					}
				}
			} else {
				MethodResolveResult result = results as MethodResolveResult;
				if (result == null)
					return;
				IClass c = result.ContainingClass;
				bool canViewProtected = c.IsTypeInInheritanceTree(result.CallingClass);
				foreach (IClass curType in c.ClassInheritanceTree) {
					foreach (IMethod method in curType.Methods) {
						if (method.Name == result.Name) {
							if (method.IsAccessible(result.CallingClass, canViewProtected)) {
								// TODO: exclude methods that were overridden
								methods.Add(method);
							}
						}
					}
				}
			}
		}
		
		public bool CaretOffsetChanged()
		{
			bool closeDataProvider = textArea.Caret.Offset <= initialOffset;
			int brackets = 0;
			int curlyBrackets = 0;
			if (!closeDataProvider) {
				bool insideChar   = false;
				bool insideString = false;
				for (int offset = initialOffset; offset < Math.Min(textArea.Caret.Offset, document.TextLength); ++offset) {
					char ch = document.GetCharAt(offset);
					switch (ch) {
						case '\'':
							insideChar = !insideChar;
							break;
						case '(':
							if (!(insideChar || insideString)) {
								++brackets;
							}
							break;
						case ')':
							if (!(insideChar || insideString)) {
								--brackets;
							}
							if (brackets <= 0) {
								return true;
							}
							break;
						case '"':
							insideString = !insideString;
							break;
						case '}':
							if (!(insideChar || insideString)) {
								--curlyBrackets;
							}
							if (curlyBrackets < 0) {
								return true;
							}
							break;
						case '{':
							if (!(insideChar || insideString)) {
								++curlyBrackets;
							}
							break;
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
//			int offset = document.Caret.Offset - 1;
//			if (offset >= 0) {
//				return document.GetCharAt(offset) == ')';
//			}
			return false;
		}
	}
}
