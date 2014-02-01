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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps the CodeEditor class to provide the ITextEditor interface.
	/// </summary>
	sealed class CodeEditorAdapter : CodeCompletionEditorAdapter
	{
		readonly CodeEditor codeEditor;
		
		public CodeEditorAdapter(CodeEditor codeEditor, CodeEditorView textEditor) : base(textEditor)
		{
			if (codeEditor == null)
				throw new ArgumentNullException("codeEditor");
			this.codeEditor = codeEditor;
		}
		
		public override FileName FileName {
			get { return codeEditor.FileName; }
		}
		
		ILanguageBinding languageBinding;
		
		public override ILanguageBinding Language {
			get { return languageBinding; }
		}
		
		List<ITextEditorExtension> extensions;
		const string extensionsPath = "/SharpDevelop/ViewContent/TextEditor/Extensions";
		
		public override ITextEditor PrimaryView {
			get { return codeEditor.PrimaryTextEditorAdapter; }
		}
		
		internal void FileNameChanged()
		{
			DetachExtensions();
			extensions = AddInTree.BuildItems<ITextEditorExtension>(extensionsPath, this, false);
			AttachExtensions();
			
			languageBinding = SD.LanguageService.GetLanguageByFileName(PrimaryView.FileName);
			
			// update properties set by languageBinding
			this.TextEditor.TextArea.IndentationStrategy = new OptionControlledIndentationStrategy(this, languageBinding.FormattingStrategy);
		}
		
		internal void DetachExtensions()
		{
			if (extensions != null) {
				foreach (var extension in extensions)
					extension.Detach();
			}
		}
		
		
		internal void AttachExtensions()
		{
			if (extensions != null) {
				foreach (var extension in extensions)
					extension.Attach(this);
			}
		}
		
		sealed class OptionControlledIndentationStrategy : IndentationStrategyAdapter
		{
			public OptionControlledIndentationStrategy(ITextEditor editor, IFormattingStrategy formattingStrategy)
				: base(editor, formattingStrategy)
			{
			}
			
			public override void IndentLine(ICSharpCode.AvalonEdit.Document.TextDocument document, ICSharpCode.AvalonEdit.Document.DocumentLine line)
			{
				if (CodeEditorOptions.Instance.UseSmartIndentation)
					base.IndentLine(document, line);
				else
					new DefaultIndentationStrategy().IndentLine(document, line);
			}
			
			// Do not override IndentLines: it is called only when smart indentation is explicitly requested by the user (Ctrl+I),
			// so we keep it enabled even when the option is set to false.
		}
		
		public override IEnumerable<ISnippetCompletionItem> GetSnippets()
		{
			CodeSnippetGroup g = SnippetManager.Instance.FindGroup(Path.GetExtension(this.FileName));
			if (g != null) {
				return g.Snippets.Select(s => s.CreateCompletionItem(this));
			} else {
				return base.GetSnippets();
			}
		}
		
		public override IList<ICSharpCode.SharpDevelop.Refactoring.IContextActionProvider> ContextActionProviders {
			get { return ((CodeEditorView)TextEditor).ContextActionProviders; }
		}
	}
}
