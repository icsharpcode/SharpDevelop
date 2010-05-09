// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.VBNet.Experimental;

namespace VBParserExperiment
{
	class Program
	{
		static string data = @"Option Explicit
Option Strict On

Imports System
Imports System.Linq

Module Program
	
End Module

Namespace Test
	Module Functions
		Public Sub New()
			
		End Sub
		
		Public Sub Test(ByVal x As Integer)
			
		End Sub
	End Module
End Namespace
		";
		
		public static void Main(string[] args)
		{
			Parser p = new Parser(ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(data)));
			
			p.Parse();
			
			Console.ReadKey(true);
		}
	}
}