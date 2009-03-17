// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion data for the ${res tag.
	/// </summary>
	public class ICSharpCodeCoreTagCompletionDataProvider : AbstractCompletionDataProvider
	{
		int startOffset;
		TextArea textArea;
		bool nonMatchingCharTyped;
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "ICSharpCode.TextEditor.Gui.CompletionWindow.DefaultCompletionData.#ctor(System.String,System.String,System.Int32)")]
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			this.textArea = textArea;
			this.startOffset = textArea.Caret.Offset;
			this.nonMatchingCharTyped = false;
			this.DefaultIndex = 0;
			return new ICompletionData[] { new DefaultCompletionData("{res", null, ClassBrowserIconService.GotoArrowIndex) };
		}
		
		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			// Return NormalKey as long as:
			// - the typed string matches the ${res tag
			// - ':' is not pressed
			// Return InsertionKey when:
			// - ':' is pressed and the typed string matches the ${res tag
			// - the typed string does not match the ${res tag and more than one
			//   character has already been typed (to close the completion window)
			
			string typedTag = this.GetTypedText();
			if (key != ':') {
				typedTag += key;
			}
			
			bool match = "${res:".StartsWith(typedTag, StringComparison.OrdinalIgnoreCase);
			
			if (key == ':') {
				if (match || this.nonMatchingCharTyped) {
					return CompletionDataProviderKeyResult.InsertionKey;
				} else {
					this.nonMatchingCharTyped = true;
					return CompletionDataProviderKeyResult.NormalKey;
				}
			} else {
				if (match) {
					this.nonMatchingCharTyped = false;
					return CompletionDataProviderKeyResult.NormalKey;
				} else {
					if (this.nonMatchingCharTyped) {
						return CompletionDataProviderKeyResult.InsertionKey;
					} else {
						this.nonMatchingCharTyped = true;
						return CompletionDataProviderKeyResult.NormalKey;
					}
				}
			}
		}
		
		string GetTypedText()
		{
			if (this.textArea == null) {
				return String.Empty;
			}
			
			int offset = Math.Max(this.startOffset, 0);
			return this.textArea.Document.GetText(offset, Math.Min(Math.Max(this.textArea.Caret.Offset - offset, 0), this.textArea.Document.TextLength - offset));
		}
	}
}
