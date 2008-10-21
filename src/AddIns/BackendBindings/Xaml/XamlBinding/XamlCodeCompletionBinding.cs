// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3494 $</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public class XamlCodeCompletionBinding : ICodeCompletionBinding
	{
		public bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			if (ch == '<') {
				editor.ShowCompletionWindow(new XamlCompletionDataProvider(XamlExpressionContext.Empty), ch);
				return true;
			}
			else if (char.IsLetter(ch)) {
				int offset = editor.ActiveTextAreaControl.TextArea.Caret.Offset;
				if (offset > 0) {
					char c = editor.Document.GetCharAt(offset - 1);
					if (c == ' ' || c == '\t') {
						XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(editor.Text, offset);
						if (path != null && path.Elements.Count > 0) {
							editor.ShowCompletionWindow(
								new XamlCompletionDataProvider(
									new XamlExpressionContext(path, "", false)
								) { IsAttributeCompletion = true }
								, '\0');
							return false;
						}
					}
				}
			}
			return false;
		}

		public bool CtrlSpace(SharpDevelopTextAreaControl editor)
		{
			XamlCompletionDataProvider provider = new XamlCompletionDataProvider();
			provider.AllowCompleteExistingExpression = true;
			editor.ShowCompletionWindow(provider, '\0');
			return true;
		}
	}

	sealed class XamlCompletionDataProvider : CtrlSpaceCompletionDataProvider
	{
		public XamlCompletionDataProvider()
		{
		}

		public XamlCompletionDataProvider(ExpressionContext overrideContext)
			: base(overrideContext)
		{
		}

		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (key == ':' || key == '.') {
				return CompletionDataProviderKeyResult.NormalKey;
			}
			else {
				return base.ProcessKey(key);
			}
		}

		public bool IsAttributeCompletion;

		public override bool InsertAction(ICompletionData data, ICSharpCode.TextEditor.TextArea textArea, int insertionOffset, char key)
		{
			CodeCompletionData ccData = data as CodeCompletionData;
			if (IsAttributeCompletion && ccData != null) {
				textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
				textArea.InsertString(ccData.Text + "=\"\"");
				textArea.Caret.Column -= 1;

				SharpDevelopTextAreaControl editor = textArea.MotherTextEditorControl as SharpDevelopTextAreaControl;
				if (editor != null) {
					WorkbenchSingleton.SafeThreadAsyncCall(
						delegate {
							XamlCompletionDataProvider provider = new XamlCompletionDataProvider();
							provider.AllowCompleteExistingExpression = true;
							editor.ShowCompletionWindow(provider, '\0');
						}
					);
				}
				return false;
			}
			else {
				return base.InsertAction(data, textArea, insertionOffset, key);
			}
		}
	}
}
