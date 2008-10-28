// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public abstract class AbstractCompletionDataProvider : ICompletionDataProvider
	{
		public virtual ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		int defaultIndex = -1;
		
		/// <summary>
		/// Gets the index of the element in the list that is chosen by default.
		/// </summary>
		public int DefaultIndex {
			get {
				return defaultIndex;
			}
			set {
				defaultIndex = value;
			}
		}
		
		protected string preSelection = null;
		
		public string PreSelection {
			get {
				return preSelection;
			}
		}
		
		bool insertSpace;
		
		/// <summary>
		/// Gets/Sets if a space should be inserted in front of the completed expression.
		/// </summary>
		public bool InsertSpace {
			get {
				return insertSpace;
			}
			set {
				insertSpace = value;
			}
		}
		
		/// <summary>
		/// Gets if pressing 'key' should trigger the insertion of the currently selected element.
		/// </summary>
		public virtual CompletionDataProviderKeyResult ProcessKey(char key)
		{
			CompletionDataProviderKeyResult res;
			if (key == ' ' && insertSpace) {
				insertSpace = false; // insert space only once
				res = CompletionDataProviderKeyResult.BeforeStartKey;
			} else if (char.IsLetterOrDigit(key) || key == '_') {
				insertSpace = false; // don't insert space if user types normally
				res = CompletionDataProviderKeyResult.NormalKey;
			} else {
				// do not reset insertSpace when doing an insertion!
				res = CompletionDataProviderKeyResult.InsertionKey;
			}
			return res;
		}
		
		public virtual bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			if (InsertSpace) {
				textArea.Document.Insert(insertionOffset++, " ");
			}
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			
			return data.InsertAction(textArea, key);
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// This method may return null.
		/// </summary>
		public abstract ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);
	}

	public abstract class AbstractCodeCompletionDataProvider : AbstractCompletionDataProvider
	{
		Hashtable insertedElements           = new Hashtable();
		Hashtable insertedPropertiesElements = new Hashtable();
		Hashtable insertedEventElements      = new Hashtable();
		
		protected int caretLineNumber;
		protected int caretColumn;
		protected string fileName;
		
		protected List<ICompletionData> completionData = null;
		protected ExpressionContext overrideContext;
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// </summary>
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			completionData = new List<ICompletionData>();
			this.fileName = fileName;
			IDocument document = textArea.Document;
			
			// the parser works with 1 based coordinates
			caretLineNumber      = document.GetLineNumberForOffset(textArea.Caret.Offset) + 1;
			caretColumn          = textArea.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
			
			GenerateCompletionData(textArea, charTyped);
			
			return completionData.ToArray();
		}
		
		protected virtual ExpressionResult GetExpression(TextArea textArea)
		{
			IDocument document = textArea.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null) {
				return new ExpressionResult(TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset));
			} else {
				ExpressionResult res = expressionFinder.FindExpression(document.GetText(0, textArea.Caret.Offset), textArea.Caret.Offset);
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
			CodeCompletionData suggestedData = null;
			foreach (object o in list) {
				if (context != null && !context.ShowEntry(o))
					continue;
				CodeCompletionData ccd = CreateItem(o, context);
				if (object.Equals(o, context.SuggestedItem))
					suggestedData = ccd;
				if (ccd != null)
					completionData.Add(ccd);
			}
			if (context.SuggestedItem != null) {
				if (suggestedData == null) {
					suggestedData = CreateItem(context.SuggestedItem, context);
					if (suggestedData != null) {
						completionData.Add(suggestedData);
					}
				}
				if (suggestedData != null) {
					completionData.Sort(DefaultCompletionData.Compare);
					this.DefaultIndex = completionData.IndexOf(suggestedData);
				}
			}
		}
		
		CodeCompletionData CreateItem(object o, ExpressionContext context)
		{
			if (o is string) {
				return new CodeCompletionData(o.ToString(), ClassBrowserIconService.NamespaceIndex);
			} else if (o is IClass) {
				return new CodeCompletionData((IClass)o);
			} else if (o is IProperty) {
				IProperty property = (IProperty)o;
				if (property.Name != null && insertedPropertiesElements[property.Name] == null) {
					insertedPropertiesElements[property.Name] = property;
					return new CodeCompletionData(property);
				}
			} else if (o is IMethod) {
				IMethod method = (IMethod)o;
				if (method.Name != null) {
					CodeCompletionData ccd = new CodeCompletionData(method);
					if (insertedElements[method.Name] == null) {
						insertedElements[method.Name] = ccd;
						return ccd;
					} else {
						CodeCompletionData oldMethod = (CodeCompletionData)insertedElements[method.Name];
						++oldMethod.Overloads;
					}
				}
			} else if (o is IField) {
				return new CodeCompletionData((IField)o);
			} else if (o is IEvent) {
				IEvent e = (IEvent)o;
				if (e.Name != null && insertedEventElements[e.Name] == null) {
					insertedEventElements[e.Name] = e;
					return new CodeCompletionData(e);
				}
			} else {
				throw new ApplicationException("Unknown object: " + o);
			}
			return null;
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
