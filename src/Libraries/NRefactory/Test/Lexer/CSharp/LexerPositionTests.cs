// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision: 230 $</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.NRefactory.Tests.Lexer.CSharp
{
	[TestFixture]
	public class LexerPositionTests
	{
		ILexer GenerateLexer(string s)
		{
			return ParserFactory.CreateLexer(SupportedLanguages.CSharp, new StringReader(s));
		}
		
		[Test]
		public void Test1()
		{
			ILexer l = GenerateLexer("public");
			Token t = l.NextToken();
			Assert.AreEqual(t.Location, new Point(1, 1));
		}
	}
}
