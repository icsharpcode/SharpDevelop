// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
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
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			XmlSchemaCompletion defaultSchema = schemaFileAssociations.GetSchemaCompletion(editor.FileName);
			XmlCompletionItemCollection completionItems = GetCompletionItems(editor, ch, defaultSchema);
			if (completionItems.HasItems) {
				completionItems.Sort();
				ICompletionListWindow completionWindow = editor.ShowCompletionWindow(completionItems);
				if (completionWindow != null) {
					SetCompletionWindowWidth(completionWindow, completionItems);
				}
			}
				
			if ((ch == '<') || (ch == ' ') || (ch == '=')) {
				return CodeCompletionKeyPressResult.Completed;
			}
			return CodeCompletionKeyPressResult.None;
		}
		
		XmlCompletionItemCollection GetCompletionItems(ITextEditor editor, char characterTyped, XmlSchemaCompletion defaultSchema)
		{
			string textUpToCursor = GetTextUpToCursor(editor, characterTyped);
			
			switch (characterTyped) {
				case '=':
					return schemas.GetNamespaceCompletion(textUpToCursor);
				case '<':
					return schemas.GetElementCompletion(textUpToCursor, defaultSchema);
				case ' ':
					return schemas.GetAttributeCompletion(textUpToCursor, defaultSchema);
			}			
			return schemas.GetAttributeValueCompletion(characterTyped, textUpToCursor, defaultSchema);
		}
		
		string GetTextUpToCursor(ITextEditor editor, char characterTyped)
		{
			return editor.Document.GetText(0, editor.Caret.Offset) + characterTyped;
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
			string text = editor.Document.Text;
			int offset = editor.Caret.Offset;
	
			XmlSchemaCompletion defaultSchema = schemaFileAssociations.GetSchemaCompletion(editor.FileName);
			XmlCompletionItemCollection completionItems = schemas.GetAttributeValueCompletion(text, offset, defaultSchema);
			if (completionItems.HasItems) {
				editor.ShowCompletionWindow(completionItems);
				return true;
			}
			return false;
		}
	}
}
