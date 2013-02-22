// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.FormattingStrategy
{
	public class CSharpFormatter
	{
		/// <summary>
		/// Formats the specified part of the document.
		/// </summary>
		public static void Format(ITextEditor editor, int offset, int length, CSharpFormattingOptions options)
		{
			var syntaxTree = new CSharpParser().Parse(editor.Document);
			var fv = new AstFormattingVisitor(options, editor.Document, editor.ToEditorOptions());
			fv.AddFormattingRegion(new DomRegion(editor.Document.GetLocation(offset), editor.Document.GetLocation(offset + length)));
			syntaxTree.AcceptVisitor(fv);
			fv.ApplyChanges(offset, length);
		}
	}
}
