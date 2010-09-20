// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;

namespace ICSharpCode.VBNetBinding
{
	public class VBStatement
	{
		string startRegex = "";
		
		public string StartRegex {
			get { return startRegex; }
		}
		
		string endRegex     = "";
		
		public string EndRegex {
			get { return endRegex; }
		}
		
		string endStatement = "";
		
		public string EndStatement {
			get { return endStatement; }
		}
		
		int statementToken = 0;
		
		public int StatementToken {
			get { return statementToken; }
		}
		
		int indentPlus = 0;
		
		public int IndentPlus {
			get { return indentPlus; }
		}
		
		public VBStatement()
		{
		}
		
		public VBStatement(string startRegex, string endRegex, string endStatement, int indentPlus, int statementToken)
		{
			this.startRegex = startRegex;
			this.endRegex   = endRegex;
			this.endStatement = endStatement;
			this.indentPlus   = indentPlus;
			this.statementToken = statementToken;
		}
	}
}
