// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlEditorOptions options;
		
		public XmlCodeCompletionBinding()
			: this(XmlEditorService.XmlEditorOptions)
		{
		}
		
		public XmlCodeCompletionBinding(XmlEditorOptions options)
		{
			this.options = options;
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			string text = editor.Document.GetText(0, editor.Caret.Offset);
			XmlCompletionDataProvider provider = options.GetProvider(editor.FileName);
			XmlCompletionItemList completionItems = provider.GenerateCompletionData(text, ch);
			if (completionItems.Items.Count > 0) {
				ICompletionListWindow completionWindow = editor.ShowCompletionWindow(completionItems);
				if (completionWindow != null) {
					XmlCompletionItem firstListItem = (XmlCompletionItem)completionItems.Items[0];
					if (firstListItem.DataType == XmlCompletionDataType.NamespaceUri) {
						completionWindow.Width = double.NaN;
					}
				}
			}
				
			if ((ch == '<') || (ch == ' ') || (ch == '=')) {
				return CodeCompletionKeyPressResult.Completed;
			}
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{	
			// Attribute value completion.
			string text = editor.Document.Text;
			int offset = editor.Caret.Offset;
			if (XmlParser.IsInsideAttributeValue(text, offset)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(text, offset);
				XmlCompletionDataProvider provider = options.GetProvider(editor.FileName);
				string attributeName = XmlParser.GetAttributeNameAtIndex(text, offset);
				XmlCompletionItemList completionItems = provider.GetAttributeValueCompletionData(path, attributeName);
				if (completionItems.Items.Count > 0) {
					editor.ShowCompletionWindow(completionItems);
					return true;
				}
			}
			return false;
		}
	}
}
