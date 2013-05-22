// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.FormattingStrategy
{
	public class CSharpFormatterHelper
	{
		/// <summary>
		/// Formats the specified part of the document.
		/// </summary>
		public static void Format(ITextEditor editor, int offset, int length, CSharpFormattingOptions options)
		{
			var formatter = new CSharpFormatter(options, editor.ToEditorOptions());
			formatter.AddFormattingRegion(new DomRegion(editor.Document.GetLocation(offset), editor.Document.GetLocation(offset + length)));
			var changes = formatter.AnalyzeFormatting(editor.Document, SyntaxTree.Parse(editor.Document));
			changes.ApplyChanges(offset, length);
		}
	}
}
