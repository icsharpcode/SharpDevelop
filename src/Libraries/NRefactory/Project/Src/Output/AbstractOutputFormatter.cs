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
	/// Base class of output formatters.
	/// </summary>
	public abstract class AbstractOutputFormatter
	{
		int           indentationLevel = 0;
		StringBuilder text = new StringBuilder();
		
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
		
		bool isIndented = false;
		
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
				isIndented = true;
			}
		}
		
		public void Space()
		{
			text.Append(' ');
		}
		
		bool gotBlankLine = true;
		ArrayList specialsText = new ArrayList();
		
		public virtual void NewLine()
		{
			if (DoNewLine) {
				text.Append(Environment.NewLine);
				gotBlankLine = true;
				isIndented = false;
				WriteSpecials();
			}
		}
		
		public virtual void EndFile()
		{
			WriteSpecials();
		}
		
		protected void WriteInNextNewLine(string text)
		{
			specialsText.Add(text);
			if (gotBlankLine) {
				WriteSpecials();
			}
		}
		
		void WriteSpecials()
		{
			if (isIndented) {
				foreach (string txt in specialsText) {
					text.Append(txt);
					text.Append(Environment.NewLine);
					Indent();
				}
			} else {
				foreach (string txt in specialsText) {
					Indent();
					text.Append(txt);
					text.Append(Environment.NewLine);
				}
				isIndented = false;
			}
			specialsText.Clear();
		}
		
		public void PrintTokenList(ArrayList tokenList)
		{
			gotBlankLine = false;
			foreach (int token in tokenList) {
				PrintToken(token);
				Space();
			}
		}
		
		public abstract void PrintComment(Comment comment);
		
		public virtual void PrintPreProcessingDirective(PreProcessingDirective directive)
		{
			WriteInNextNewLine(directive.Cmd + directive.Arg);
		}
		
		public abstract void PrintToken(int token);
		
		protected void PrintToken(string text)
		{
			gotBlankLine = false;
			this.text.Append(text);
		}
		
		public void PrintIdentifier(string identifier)
		{
			gotBlankLine = false;
			text.Append(identifier);
		}
	}
}
