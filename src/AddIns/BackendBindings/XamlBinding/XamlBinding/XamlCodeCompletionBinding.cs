// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3494 $</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ICSharpCode.SharpDevelop.ITextEditor editor, char ch)
		{
			XamlCompletionItemProvider provider;
			switch (ch) {
				case '<':
					provider = new XamlCompletionItemProvider(XamlExpressionContext.Empty);
					provider.ShowCompletion(editor);
					return CodeCompletionKeyPressResult.Completed;
				default:
					if (char.IsLetter(ch)) {
						int offset = editor.Caret.Offset;
						if (offset > 0) {
							char c = editor.Document.GetCharAt(offset - 1);
							if (c == ' ' || c == '\t') {
								XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(editor.Document.Text, offset);
								if (path != null && path.Elements.Count > 0) {
									provider = new XamlCompletionItemProvider(new XamlExpressionContext(path, "", false));
									provider.ShowCompletion(editor);
									return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
								}
							}
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ICSharpCode.SharpDevelop.ITextEditor editor)
		{
			XamlCompletionItemProvider provider = new XamlCompletionItemProvider(XamlExpressionContext.Empty);
			provider.ShowCompletion(editor);
			return true;
		}
	}
}
