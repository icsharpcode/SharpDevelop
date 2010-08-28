// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.VBNetBinding
{
	/// <summary>
	/// Description of VBIndentationStrategy.
	/// </summary>
	public partial class VBIndentationStrategy
	{
		Stack<Block> indentationStack = new Stack<Block>();
		ITextEditor editor;
		IDocumentLine startLine, endLine;
		
		public string CurrentIndent {
			get { return (indentationStack.PeekOrDefault() ?? Block.Empty).Indentation; }
		}
		
		class Block {
			public static readonly Block Empty = new Block {
				Indentation = "",
				StartLine = 1
			};
			
			public string Indentation;
			public int StartLine;
		}
		
		public VBIndentationStrategy(ITextEditor editor, int start, int end)
			: this()
		{
			this.editor = editor;
			
			startLine = editor.Document.GetLine(start);
			endLine = editor.Document.GetLine(end);
		}
		
		void Indent(Token la)
		{
			ApplyIndent(la);
			Block parent = indentationStack.PeekOrDefault() ?? Block.Empty;
			indentationStack.Push(new Block() { Indentation = parent.Indentation + editor.Options.IndentationString, StartLine = t.Location.Line + 1 } );
		}
		
		void Unindent(Token la)
		{
			ApplyIndent(la);
			indentationStack.PopOrDefault();
		}
		
		void ApplyIndent(Token la)
		{
			Block current = indentationStack.PeekOrDefault() ?? Block.Empty;
			
			if (t.Location.Line < startLine.LineNumber)
				return;
			
			IDocumentLine firstLine = startLine.LineNumber > current.StartLine ? startLine : editor.Document.GetLine(current.StartLine);
			IDocumentLine currentLine = firstLine;
			
			while (currentLine.LineNumber < la.Location.Line) {
				editor.Document.SmartReplaceLine(currentLine, current.Indentation + currentLine.Text.Trim());
				
				if (currentLine.LineNumber == endLine.LineNumber)
					break;
				
				currentLine = editor.Document.GetLine(currentLine.LineNumber + 1);
			}
		}
	}
}
