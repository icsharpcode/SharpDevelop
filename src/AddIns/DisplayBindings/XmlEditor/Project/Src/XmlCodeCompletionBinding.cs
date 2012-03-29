// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
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
			if (char.IsWhiteSpace(ch) || editor.SelectionLength > 0)
				return CodeCompletionKeyPressResult.None;
			if (ignoredChars.Contains(ch))
				return CodeCompletionKeyPressResult.None;
			if (XmlParser.GetXmlIdentifierBeforeIndex(editor.Document, editor.Caret.Offset).Length > 0)
				return CodeCompletionKeyPressResult.None;
			editor.Document.Insert(editor.Caret.Offset, ch.ToString());
			CtrlSpace(editor);
			return CodeCompletionKeyPressResult.EatKey;
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
		
		bool ElementStartsWith(string text, int elementStartIndex, ITextBuffer document)
		{
			int textLength = Math.Min(text.Length, document.TextLength - elementStartIndex);
			return document.GetText(elementStartIndex, textLength).Equals(text, StringComparison.OrdinalIgnoreCase);
		}
	}
}
