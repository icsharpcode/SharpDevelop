// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using VB = ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.VBNet.Experimental;

namespace VBParserExperiment
{
	class Program
	{
		static string data = @"Class Test
	Public Sub New()
		Dim x = <a>
	End Sub
End Class
		";
		
		public static void Main(string[] args)
		{
			ExpressionFinder p = new ExpressionFinder();
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(data));
			Token t;
			do {
				t = lexer.NextToken();
				p.InformToken(t);
			} while (t.Kind != VB.Tokens.EOF);
			
			Console.ReadKey(true);
		}
	}
}