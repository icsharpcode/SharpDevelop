// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting
{
	public abstract class ScriptingFormattingStrategy : DefaultFormattingStrategy
	{
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			LineIndenter indenter = CreateLineIndenter(editor, line);
			if (!indenter.Indent()) {
				base.IndentLine(editor, line);
			}
		}
		
		protected abstract LineIndenter CreateLineIndenter(ITextEditor editor, IDocumentLine line);
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithSingleLineComment(editor, LineComment);
		}
		
		public virtual string LineComment {
			get { return String.Empty; }
		}
	}
}
