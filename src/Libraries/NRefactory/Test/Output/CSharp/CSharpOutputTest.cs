// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.PrettyPrinter
{
	[TestFixture]
	public class CSharpOutputTest
	{
		[Test]
		public void CSharpOutputTest1()
		{
//	 TODO:		
//			string program = "public class Test" + Environment.NewLine + 
//			                 "{" + Environment.NewLine + 
//			                 "\tvoid A()" + Environment.NewLine + 
//			                 "\t{" + Environment.NewLine + 
//			                 "\t\tstring test = 546.ToString();" + Environment.NewLine + 
//			                 "\t}" + Environment.NewLine + 
//			                 "}" + Environment.NewLine;
//			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
//			parser.Parse();
//			CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor();
//			outputVisitor.Visit(parser.CompilationUnit, null);
//			
//			Assert.AreEqual(program, outputVisitor.Text);
		}
		
	}
}
