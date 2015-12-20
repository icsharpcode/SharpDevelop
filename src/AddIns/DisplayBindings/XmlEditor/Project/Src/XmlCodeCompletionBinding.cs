// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	public class XmlCodeCompletionBinding : ICodeCompletionBinding
	{
		XmlSchemaFileAssociations schemaFileAssociations;
		XmlSchemaCompletionCollection schemas;
		
		public XmlCodeCompletionBinding()
			: this(XmlEditorService.XmlSchemaFileAssociations)
		{
		}
		
		public XmlCodeCompletionBinding(XmlSchemaFileAssociations schemaFileAssociations)
		{
			this.schemaFileAssociations = schemaFileAssociations;
			this.schemas = schemaFileAssociations.Schemas;
		}
		
		char[] ignoredChars = new[] { '\\', '/', '"', '\'', '=', '>', '!', '?' };
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			return CodeCompletionKeyPressResult.None;
		}
		
		XmlCompletionItemCollection GetCompletionItems(ITextEditor editor, XmlSchemaCompletion defaultSchema)
		{
			int offset = editor.Caret.Offset;
			string textUpToCursor = editor.Document.GetText(0, offset);
			
			XmlCompletionItemCollection items = new XmlCompletionItemCollection();
			if (XmlParser.IsInsideAttributeValue(textUpToCursor, offset)) {
				items = schemas.GetNamespaceCompletion(textUpToCursor);
				if (items.Count == 0)
					items = schemas.GetAttributeValueCompletion(textUpToCursor, editor.Caret.Offset, defaultSchema);
			} else {
				items = schemas.GetAttributeCompletion(textUpToCursor, defaultSchema);
				if (items.Count == 0)
					items = schemas.GetElementCompletion(textUpToCursor, defaultSchema);
			}
			return items;
		}
		
		void SetCompletionWindowWidth(ICompletionListWindow completionWindow, XmlCompletionItemCollection completionItems)
		{
			XmlCompletionItem firstListItem = completionItems[0];
			if (firstListItem.DataType == XmlCompletionItemType.NamespaceUri) {
				completionWindow.Width = double.NaN;
			}
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			int elementStartIndex = XmlParser.GetActiveElementStartIndex(editor.Document.Text, editor.Caret.Offset);
			if (elementStartIndex <= -1)
				return false;
			if (ElementStartsWith("<!", elementStartIndex, editor.Document))
				return false;
			if (ElementStartsWith("<?", elementStartIndex, editor.Document))
				return false;
			
			XmlSchemaCompletion defaultSchema = schemaFileAssociations.GetSchemaCompletion(editor.FileName);
			
			XmlCompletionItemCollection completionItems = GetCompletionItems(editor, defaultSchema);
			if (completionItems.HasItems) {
				completionItems.Sort();
				string identifier = XmlParser.GetXmlIdentifierBeforeIndex(editor.Document, editor.Caret.Offset);
				completionItems.PreselectionLength = identifier.Length;
				ICompletionListWindow completionWindow = editor.ShowCompletionWindow(completionItems);
				if (completionWindow != null) {
					SetCompletionWindowWidth(completionWindow, completionItems);
				}
				return true;
			}
			return false;
		}
		
		bool ElementStartsWith(string text, int elementStartIndex, ITextSource document)
		{
			int textLength = Math.Min(text.Length, document.TextLength - elementStartIndex);
			return document.GetText(elementStartIndex, textLength).Equals(text, StringComparison.OrdinalIgnoreCase);
		}
		
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			if (char.IsWhiteSpace(ch) || editor.SelectionLength > 0)
				return false;
			if (ignoredChars.Contains(ch))
				return false;
			if (XmlParser.GetXmlIdentifierBeforeIndex(editor.Document, editor.Caret.Offset).Length > 1)
				return false;
			return CtrlSpace(editor);
		}
	}
}
