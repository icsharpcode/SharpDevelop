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
	public class OutputFormatter : AbstractOutputFormatter
	{
//		Lexer         lexer; 
		
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
		
//		Token token;
		public OutputFormatter(PrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
//			lexer = new Lexer(new StringReader(originalSourceFile));
//			token = lexer.NextToken();
//			PrintSpecials(token.kind);
		}
		
//		bool gotBlankLine  = false;
//		int currentSpecial = 0;
		
//		void PrintSpecials(int tokenKind)
//		{
//			if (currentSpecial >= lexer.SpecialTracker.CurrentSpecials.Count) {
//				return;
//			}
//			object o = lexer.SpecialTracker.CurrentSpecials[currentSpecial++];
//			if (o is Comment) {
////				Console.WriteLine("COMMENT " + o);	
//				Comment comment = (Comment)o;
//				switch (comment.CommentType) {
//					case CommentType.SingleLine:
//						text.Append("//");	
//						text.Append(comment.CommentText);	
//						text.Append("\n");
//						Indent();
//						break;
//					case CommentType.Documentation:
//						text.Append("///");	
//						text.Append(comment.CommentText);	
//						text.Append("\n");	
//						Indent();
//						break;
//					case CommentType.Block:
//						text.Append("/*");	
//						text.Append(comment.CommentText);	
//						text.Append("*/\n");	
//						Indent();
//						break;
//				}
//				PrintSpecials(tokenKind);
//			} else if (o is BlankLine) {
//				if (!gotBlankLine) {
////						text.Append("\n");
////						Indent();
//				} 
//				gotBlankLine = false;
//				PrintSpecials(tokenKind);
//			} else if (o is PreProcessingDirective) { 
////				Console.WriteLine("PPD:" + o);	
//				text.Append("\n");
//				PreProcessingDirective ppd = (PreProcessingDirective)o;
//				text.Append(ppd.Cmd);
//				if (ppd.Arg != null && ppd.Arg.Length > 0) {
//					text.Append(" ");
//					text.Append(ppd.Arg);
//				}
//				text.Append("\n");
//				Indent();
//				PrintSpecials(tokenKind);
//			} else {
//				int kind = (int)o;
////				Console.WriteLine(kind + " -- " + Tokens.GetTokenString(kind));
//				if (kind != tokenKind) {
//					PrintSpecials(tokenKind);
//				}
//			}
//		}
		
		public override void PrintToken(int token)
		{
//			Console.WriteLine("PRINT TOKEN:" + token);
			if (token == Tokens.Semicolon && !EmitSemicolon) {
				return;
			}
//			while (this.token == null || this.token.kind > 0) {
//				this.token = lexer.NextToken();
////				PrintSpecials(this.token.kind);
//				if (this.token.kind == token) {
//					break;
//				}
//			}
			text.Append(Tokens.GetTokenString(token));
//			gotBlankLine = false;
		}
		
		Stack braceStack = new Stack();
		
		public void BeginBrace(BraceStyle style)
		{
			switch (style) {
				case BraceStyle.EndOfLine:
					text.Append(" ");
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
		
		public void BeginNode(INode node)
		{
		}
		
		public void EndNode(INode node)
		{
		}
	}
}
