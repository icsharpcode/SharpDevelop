// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public abstract class AbstractCompletionDataProvider : ICompletionDataProvider
	{
		Hashtable insertedElements           = new Hashtable();
		Hashtable insertedPropertiesElements = new Hashtable();
		Hashtable insertedEventElements      = new Hashtable();
		
		public ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		public int DefaultIndex {
			get {
				return -1;
			}
		}
		
		protected int caretLineNumber;
		protected int caretColumn;
		protected string fileName;
		protected string preSelection = null;
		
		public string PreSelection {
			get {
				return preSelection;
			}
		}
		protected ArrayList completionData = null;
		protected ExpressionContext overrideContext;
		
		public virtual ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			completionData = new ArrayList();
			this.fileName = fileName;
			IDocument document = textArea.Document;
			
			// the parser works with 1 based coordinates
			caretLineNumber      = document.GetLineNumberForOffset(textArea.Caret.Offset) + 1;
			caretColumn          = textArea.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			
			GenerateCompletionData(textArea, charTyped);
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		protected ExpressionResult GetExpression(TextArea textArea)
		{
			IDocument document = textArea.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null) {
				return new ExpressionResult(TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset));
			} else {
				ExpressionResult res = expressionFinder.FindExpression(document.GetText(0, textArea.Caret.Offset), textArea.Caret.Offset - 1);
				if (overrideContext != null)
					res.Context = overrideContext;
				return res;
			}
		}
		
		protected abstract void GenerateCompletionData(TextArea textArea, char charTyped);
		
		protected void AddResolveResults(ICollection list, ExpressionContext context)
		{
			if (list == null) {
				return;
			}
			completionData.Capacity += list.Count;
			foreach (object o in list) {
				if (context != null && !context.ShowEntry(o))
					continue;
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
		
		protected void AddResolveResults(ResolveResult results, ExpressionContext context)
		{
			insertedElements.Clear();
			insertedPropertiesElements.Clear();
			insertedEventElements.Clear();
			
			if (results != null) {
				AddResolveResults(results.GetCompletionData(ParserService.CurrentProjectContent), context);
			}
		}
	}
}
