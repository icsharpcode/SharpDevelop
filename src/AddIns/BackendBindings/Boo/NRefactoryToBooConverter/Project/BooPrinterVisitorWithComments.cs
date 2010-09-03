// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;
using ICSharpCode.NRefactory;

namespace NRefactoryToBooConverter
{
	public class BooPrinterVisitorWithComments : BooPrinterVisitor, ISpecialVisitor
	{
		IEnumerator<ISpecial> enumerator;
		bool available; // true when more specials are available
		
		public BooPrinterVisitorWithComments(IEnumerable<ISpecial> specials, TextWriter writer)
			: base(writer)
		{
			if (specials == null) throw new ArgumentNullException("specials");
			enumerator = specials.GetEnumerator();
			available = enumerator.MoveNext();
		}
		
		void AcceptPoint(LexicalInfo lex)
		{
			if (lex != null && lex.IsValid) {
				AcceptPoint(lex.Line, lex.Column);
			}
		}
		
		void AcceptPoint(SourceLocation loc)
		{
			if (loc != null && loc.IsValid) {
				AcceptPoint(loc.Line, loc.Column);
			}
		}
		
		/// <summary>
		/// Writes all specials up to the specified location.
		/// </summary>
		void AcceptPoint(int line, int column)
		{
			while (available) {
				Location b = enumerator.Current.StartPosition;
				if (b.Y < line || (b.Y == line && b.X <= column)) {
					WriteCurrent();
				} else {
					break;
				}
			}
		}
		
		void WriteCurrent()
		{
			enumerator.Current.AcceptVisitor(this, null);
			available = enumerator.MoveNext();
		}
		
		/// <summary>
		/// Outputs all missing specials to the writer.
		/// </summary>
		public void Finish()
		{
			while (available) {
				WriteCurrent();
			}
		}
		
		Node currentDocuNode;
		bool isInEndMode = false;
		
		public override bool Visit(Node node)
		{
			if (node == null) return base.Visit(node);
			currentDocuNode = node;
			AcceptPoint(node.LexicalInfo);
			currentDocuNode = null;
			bool result = base.Visit(node);
			isInEndMode = true;
			AcceptPoint(node.EndSourceLocation);
			isInEndMode = false;
			oldIndentation = _indent;
			return result;
		}
		
		#region ICSharpCode.NRefactory.Parser.ISpecialVisitor interface implementation
		int oldIndentation;
		
		struct DelayedSpecial
		{
			public readonly int Indentation;
			public readonly string Format;
			public readonly object[] Args;
			public DelayedSpecial(int indentation, string Format, object[] Args) {
				this.Indentation = indentation;
				this.Format = Format;
				this.Args = Args;
			}
		}
		
		List<DelayedSpecial> delayedSpecials = new List<DelayedSpecial>();
		
		bool writingDelayedSpecials;
		
		public override void WriteLine()
		{
			if (_disableNewLine == 0) {
				base.WriteLine();
				if (!writingDelayedSpecials) {
					writingDelayedSpecials = true;
					int tmp = _indent;
					foreach (DelayedSpecial special in delayedSpecials) {
						_indent = special.Indentation;
						WriteLine(special.Format, special.Args);
					}
					delayedSpecials.Clear();
					writingDelayedSpecials = false;
					_indent = tmp;
				}
				oldIndentation = _indent;
			}
		}
		
		void WriteSpecialText(bool writeInline, string format, params object[] args)
		{
			// write comment in it's own line
			if (_needsIndenting || writeInline) {
				int tmp = _indent;
				if (isInEndMode) {
					_indent = oldIndentation;
				}
				if (writeInline && !_needsIndenting) {
					WriteIndented(format, args);
				} else {
					WriteLine(format, args);
				}
				if (isInEndMode) {
					_indent = tmp;
				}
			} else {
				// comment is in the middle of line
				// put it after the next line
				delayedSpecials.Add(new DelayedSpecial(isInEndMode ? oldIndentation : _indent, format, args));
			}
		}
		
		object ISpecialVisitor.Visit(ISpecial special, object data)
		{
			throw new NotImplementedException();
		}
		
		object ISpecialVisitor.Visit(BlankLine special, object data)
		{
			WriteSpecialText(false, "");
			return null;
		}
		
		object ISpecialVisitor.Visit(Comment special, object data)
		{
			switch (special.CommentType) {
				case CommentType.Documentation:
					if (currentDocuNode == null)
						goto default;
					currentDocuNode.Documentation += special.CommentText;
					break;
				case CommentType.Block:
					WriteSpecialText(true, "/*{0}*/", special.CommentText);
					break;
				default:
					WriteSpecialText(false, "//{0}", special.CommentText);
					break;
			}
			return null;
		}
		
		object ISpecialVisitor.Visit(PreprocessingDirective special, object data)
		{
			if (string.IsNullOrEmpty(special.Arg))
				WriteSpecialText(false, "{0}", special.Cmd);
			else
				WriteSpecialText(false, "{0} {1}", special.Cmd, special.Arg);
			return null;
		}
		#endregion
	}
}
