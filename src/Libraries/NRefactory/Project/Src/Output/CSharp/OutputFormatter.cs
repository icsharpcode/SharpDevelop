// OutputFormatter.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	public sealed class CSharpOutputFormatter : AbstractOutputFormatter
	{
		PrettyPrintOptions prettyPrintOptions;
		
		bool          emitSemicolon  = true;
		
		public bool EmitSemicolon {
			get {
				return emitSemicolon;
			}
			set {
				emitSemicolon = value;
			}
		}
		
		public CSharpOutputFormatter(PrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}
		
		public override void PrintToken(int token)
		{
			if (token == Tokens.Semicolon && !EmitSemicolon) {
				return;
			}
			PrintText(Tokens.GetTokenString(token));
		}
		
		Stack braceStack = new Stack();
		
		public void BeginBrace(BraceStyle style)
		{
			switch (style) {
				case BraceStyle.EndOfLine:
					Space();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					++IndentationLevel;
					break;
				case BraceStyle.NextLine:
					NewLine();
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					++IndentationLevel;
					break;
				case BraceStyle.NextLineShifted:
					NewLine();
					++IndentationLevel;
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					break;
				case BraceStyle.NextLineShifted2:
					NewLine();
					++IndentationLevel;
					Indent();
					PrintToken(Tokens.OpenCurlyBrace);
					NewLine();
					++IndentationLevel;
					break;
			}
			braceStack.Push(style);
		}
		
		public void EndBrace()
		{
			BraceStyle style = (BraceStyle)braceStack.Pop();
			switch (style) {
				case BraceStyle.EndOfLine:
				case BraceStyle.NextLine:
					--IndentationLevel;
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					NewLine();
					break;
				case BraceStyle.NextLineShifted:
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					NewLine();
					--IndentationLevel;
					break;
				case BraceStyle.NextLineShifted2:
					--IndentationLevel;
					Indent();
					PrintToken(Tokens.CloseCurlyBrace);
					NewLine();
					--IndentationLevel;
					break;
			}
		}
		
		public override void PrintIdentifier(string identifier)
		{
			if (Keywords.GetToken(identifier) >= 0)
				PrintText("@");
			PrintText(identifier);
		}
		
		public override void PrintComment(Comment comment)
		{
			switch (comment.CommentType) {
				case CommentType.Block:	
					PrintText("/*");
					PrintText(comment.CommentText);
					PrintText("*/");
					break;
				case CommentType.Documentation:
					WriteInPreviousLine("///" + comment.CommentText);
					break;
				default:
					WriteInPreviousLine("//" + comment.CommentText);
					break;
			}
		}
	}
}
