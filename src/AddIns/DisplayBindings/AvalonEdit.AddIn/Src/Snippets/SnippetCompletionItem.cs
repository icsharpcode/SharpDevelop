// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// Code completion item for snippets.
	/// </summary>
	public class SnippetCompletionItem : IFancyCompletionItem, ISnippetCompletionItem
	{
		readonly CodeSnippet codeSnippet;
		readonly ITextEditor textEditor;
		readonly TextArea textArea;
		
		public SnippetCompletionItem(ITextEditor textEditor, CodeSnippet codeSnippet)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			if (codeSnippet == null)
				throw new ArgumentNullException("codeSnippet");
			this.textEditor = textEditor;
			this.textArea = textEditor.GetService(typeof(TextArea)) as TextArea;
			if (textArea == null)
				throw new ArgumentException("textEditor must be an AvalonEdit text editor");
			this.codeSnippet = codeSnippet;
			
			this.Priority = CodeCompletionDataUsageCache.GetPriority("snippet" + codeSnippet.Name, true);
		}
		
		public bool AlwaysInsertSnippet { get; set; }
		
		public string Text {
			get { return codeSnippet.Name; }
		}
		
		public string Keyword {
			get { return codeSnippet.Keyword; }
		}
		
		public string Description {
			get {
				return codeSnippet.Description + Environment.NewLine +
					ResourceService.GetString("Dialog.Options.CodeTemplate.PressTabToInsertTemplate");
			}
		}
		
		public ICSharpCode.SharpDevelop.IImage Image {
			get {
				return ClassBrowserIconService.CodeTemplate;
			}
		}
		
		public void Complete(CompletionContext context)
		{
			if (context.Editor != this.textEditor)
				throw new ArgumentException("wrong editor");
			
			CodeCompletionDataUsageCache.IncrementUsage("snippet" + codeSnippet.Name);
			
			using (context.Editor.Document.OpenUndoGroup()) {
				if (context.CompletionChar == '\t' || AlwaysInsertSnippet) {
					codeSnippet.TrackUsage("SnippetCompletionItem");
					
					context.Editor.Document.Remove(context.StartOffset, context.Length);
					CreateSnippet().Insert(textArea);
				} else {
					context.Editor.Document.Replace(context.StartOffset, context.Length, this.Text);
				}
			}
		}
		
		Snippet CreateSnippet()
		{
			return codeSnippet.CreateAvalonEditSnippet(textEditor);
		}
		
		object IFancyCompletionItem.Content {
			get {
				return Text;
			}
		}
		
		TextBlock fancyDescription;
		
		object IFancyCompletionItem.Description {
			get {
				if (fancyDescription == null) {
					fancyDescription = new TextBlock();
					fancyDescription.Inlines.Add(new Run(this.Description + Environment.NewLine + Environment.NewLine));
					Inline r = CreateSnippet().ToTextRun();
					r.FontFamily = textArea.FontFamily;
					fancyDescription.Inlines.Add(r);
				}
				return fancyDescription;
			}
		}
		
		public double Priority { get; set; }
	}
}
