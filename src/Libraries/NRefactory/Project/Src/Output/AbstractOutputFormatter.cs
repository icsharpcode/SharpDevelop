/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 15.09.2004
 * Time: 10:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	/// <summary>
	/// Description of VBNetOutputFormatter.
	/// </summary>
	public abstract class AbstractOutputFormatter
	{
		int           indentationLevel = 0;
		protected StringBuilder text = new StringBuilder();
		
		bool          indent         = true;
		bool          doNewLine      = true;
		AbstractPrettyPrintOptions prettyPrintOptions;
		
		public int IndentationLevel {
			get {
				return indentationLevel;
			}
			set {
				indentationLevel = value;
			}
		}
		
		public string Text {
			get {
				return text.ToString();
			}
		}
		
		
		public bool DoIndent {
			get {
				return indent;
			}
			set {
				indent = value;
			}
		}
		
		public bool DoNewLine {
			get {
				return doNewLine;
			}
			set {
				doNewLine = value;
			}
		}
		
		public AbstractOutputFormatter(AbstractPrettyPrintOptions prettyPrintOptions)
		{
			this.prettyPrintOptions = prettyPrintOptions;
		}
		
		public void Indent()
		{
			if (DoIndent) {
				int indent = 0;
				while (indent < prettyPrintOptions.IndentSize * indentationLevel) {
					char ch = prettyPrintOptions.IndentationChar;
					if (ch == '\t' && indent + prettyPrintOptions.TabSize > prettyPrintOptions.IndentSize * indentationLevel) {
						ch = ' ';
					}
					text.Append(ch);
					if (ch == '\t') {
						indent += prettyPrintOptions.TabSize;
					} else {
						++indent;
					}
				}
			}
		}
		
		public void Space()
		{
			text.Append(' ');
		}
		
		public void NewLine()
		{
			if (DoNewLine) {
				text.Append(Environment.NewLine);
//				gotBlankLine = true;
			}
		}
		
		public void EndFile()
		{
//			while (this.token == null || this.token.kind > 0) {
//				this.token = lexer.NextToken();
//				PrintSpecials(token.kind);
//			}
//			PrintSpecials(-1);
//			foreach (object o in lexer.SpecialTracker.CurrentSpecials) {
//				Console.WriteLine(o);
//			}
		}
		
		
		public void PrintTokenList(ArrayList tokenList)
		{
//			ArrayList trackList = (ArrayList)tokenList.Clone();
//			while (this.token == null || trackList.Count > 0) {
//				this.token = lexer.NextToken();
////				PrintSpecials(this.token.kind);
//				for (int i = 0; i < trackList.Count; ++i) {
//					trackList.RemoveAt(i);
//					break;
//				}
//			}
			foreach (int token in tokenList) {
				PrintToken(token);
				Space();
			}
		}
		
		public abstract void PrintToken(int token);
		
		public void PrintIdentifier(string identifier)
		{
//			this.token = lexer.NextToken();
//			PrintSpecials(token.kind);
			text.Append(identifier);
		}
		
	}
}
