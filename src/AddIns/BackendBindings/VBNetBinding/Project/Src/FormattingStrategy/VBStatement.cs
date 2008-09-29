// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2994 $</version>
// </file>
using System;

namespace VBNetBinding.FormattingStrategy
{
	public class VBStatement
	{
		public string StartRegex   = "";
		public string EndRegex     = "";
		public string EndStatement = "";
		
		public int IndentPlus = 0;
		
		public VBStatement()
		{
		}
		
		public VBStatement(string startRegex, string endRegex, string endStatement, int indentPlus)
		{
			StartRegex = startRegex;
			EndRegex   = endRegex;
			EndStatement = endStatement;
			IndentPlus   = indentPlus;
		}
	}
}
