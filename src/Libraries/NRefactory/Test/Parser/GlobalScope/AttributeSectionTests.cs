/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class AttributeSectionTests
	{
		[Test]
		public void AttributeSection1Test()
		{
//			string program = @"
//< StructLayout( LayoutKind.Explicit )> _
//Public Structure MyUnion
//
//	< FieldOffset( 0 )> Public i As Integer
//	< FieldOffset( 0 )> Public d As Double
//	
//End Structure 'MyUnion
//";
//			IParser parser = ParserFactory.CreateParser(SupportedLanguages.VBNet, new StringReader(program));
//			parser.Parse();
//			Assert.IsTrue(parser.Errors.ErrorOutput.Length == 0);
		}
		
	}
}
