// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CodeCompletionDataProvider : ICompletionDataProvider
	{
		Hashtable insertedElements           = new Hashtable();
		Hashtable insertedPropertiesElements = new Hashtable();
		Hashtable insertedEventElements      = new Hashtable();
		
		public ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		int caretLineNumber;
		int caretColumn;
		string fileName;
		string preSelection = null;
		
		public string PreSelection {
			get {
				return preSelection;
			}
		}
		ArrayList completionData = null;
		bool ctrlSpace;
		ExpressionContext context;
		
		public CodeCompletionDataProvider(bool ctrlSpace)
		{
			this.ctrlSpace = ctrlSpace;
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			IDocument document =  textArea.Document;
			completionData = new ArrayList();
			this.fileName = fileName;
			
			// the parser works with 1 based coordinates
			caretLineNumber      = document.GetLineNumberForOffset(textArea.Caret.Offset) + 1;
			caretColumn          = textArea.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			string expression;
			if (expressionFinder == null) {
				expression = TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset);
				context = ExpressionContext.Default;
			} else {
				ExpressionResult er = expressionFinder.FindExpression(textArea.Document.GetText(0, textArea.Caret.Offset), textArea.Caret.Offset - 1);
				expression = er.Expression;
				context = er.Context;
			}
			ResolveResult results;
			preSelection  = null;
			
			if (ctrlSpace) {
				if (expression == null || expression.Length == 0) {
					preSelection = "";
					if (charTyped != '\0') {
						preSelection = null;
					}
					AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, document.TextContent));
					return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
				}
				
				int idx = expression.LastIndexOf('.');
				if (idx > 0) {
					preSelection = expression.Substring(idx + 1);
					expression = expression.Substring(0, idx);
					if (charTyped != '\0') {
						preSelection = null;
					}
					
				} else {
					preSelection = expression;
					if (charTyped != '\0') {
						preSelection = null;
					}
					AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, document.TextContent));
					return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
				}
			}
			
			//Console.WriteLine("Expression: >{0}<", expression);
			
			if (expression == null || expression.Length == 0) {
				return null;
			}
			
			results = ParserService.Resolve(expression,
			                                caretLineNumber,
			                                caretColumn,
			                                fileName,
			                                document.TextContent);
			AddResolveResults(results);
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		void AddResolveResults(ICollection list)
		{
			if (list == null) {
				return;
			}
			completionData.Capacity += list.Count;
			foreach (object o in list) {
				if (context == ExpressionContext.Namespace && !(o is string)) continue;
				if (o is string) {
					completionData.Add(new CodeCompletionData(o.ToString(), ClassBrowserIconService.NamespaceIndex));
				} else if (o is IClass) {
					completionData.Add(new CodeCompletionData((IClass)o));
				} else if (o is IProperty) {
					IProperty property = (IProperty)o;
					if (property.Name != null && insertedPropertiesElements[property.Name] == null) {
						completionData.Add(new CodeCompletionData(property));
						insertedPropertiesElements[property.Name] = property;
					}
				} else if (o is IMethod) {
					IMethod method = (IMethod)o;
					if (method.Name != null &&!method.IsConstructor) {
						CodeCompletionData ccd = new CodeCompletionData(method);
						if (insertedElements[method.Name] == null) {
							completionData.Add(ccd);
							insertedElements[method.Name] = ccd;
						} else {
							CodeCompletionData oldMethod = (CodeCompletionData)insertedElements[method.Name];
							++oldMethod.Overloads;
						}
					}
				} else if (o is IField) {
					completionData.Add(new CodeCompletionData((IField)o));
				} else if (o is IEvent) {
					IEvent e = (IEvent)o;
					if (e.Name != null && insertedEventElements[e.Name] == null) {
						completionData.Add(new CodeCompletionData(e));
						insertedEventElements[e.Name] = e;
					}
				}
			}
		}
		
		void AddResolveResults(ResolveResult results)
		{
			insertedElements.Clear();
			insertedPropertiesElements.Clear();
			insertedEventElements.Clear();
			
			if (results != null) {
				AddResolveResults(results.GetCompletionData(ParserService.CurrentProjectContent));
			}
		}
	}
}
