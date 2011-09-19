// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using CSharpBinding.FormattingStrategy;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpLanguageBinding.
	/// </summary>
	public class CSharpLanguageBinding : DefaultLanguageBinding
	{
		public override IFormattingStrategy FormattingStrategy {
			get { return new CSharpFormattingStrategy(); }
		}
		
//		public override LanguageProperties Properties {
//			get { return LanguageProperties.CSharp; }
//		}
//
		public override IBracketSearcher BracketSearcher {
			get { return new CSharpBracketSearcher(); }
		}
		
		ITextEditor editor;
		CSharpSemanticHighlighter semanticHighlighter;
		
		public override void Attach(ITextEditor editor)
		{
			base.Attach(editor);
			this.editor = editor;
			ISyntaxHighlighter highlighter = editor.GetService(typeof(ISyntaxHighlighter)) as ISyntaxHighlighter;
			if (highlighter != null) {
				semanticHighlighter = new CSharpSemanticHighlighter(editor, highlighter.HighlightingDefinition);
				highlighter.AddAdditionalHighlighter(semanticHighlighter);
			}
		}
		
		public override void Detach()
		{
			ISyntaxHighlighter highlighter = editor.GetService(typeof(ISyntaxHighlighter)) as ISyntaxHighlighter;
			if (highlighter != null) {
				highlighter.RemoveAdditionalHighlighter(semanticHighlighter);
				semanticHighlighter = null;
			}
			this.editor = null;
			base.Detach();
		}
	}
}
