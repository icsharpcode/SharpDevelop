// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;

namespace VBNetBinding
{
	public class VBStatement
	{
		public string StartRegex   = "";
		public string EndRegex     = "";
		public string EndStatement = "";
		
		public int StatementToken = 0;
		public int IndentPlus = 0;
		
		public VBStatement()
		{
		}
		
		public VBStatement(string startRegex, string endRegex, string endStatement, int indentPlus, int statementToken)
		{
			StartRegex = startRegex;
			EndRegex   = endRegex;
			EndStatement = endStatement;
			IndentPlus   = indentPlus;
			StatementToken = statementToken;
		}
	}
}
