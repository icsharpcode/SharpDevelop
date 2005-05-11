/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 11.05.2005
 * Time: 16:37
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.IO;

using NUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class SkipMethodBodiesTest
	{
		[Test]
		public void EmptyMethods()
		{
			string txt = @"internal sealed class Lexer : AbstractLexer
			{
				public Lexer(TextReader reader) : base(reader)
				{
				}
				
				void Method()
				{
				}
			}";
			Check((TypeDeclaration)ParseUtilCSharp.ParseGlobal(txt, typeof(TypeDeclaration), false, true));
		}
		
		[Test]
		public void NonEmptyMethods()
		{
			string txt = @"internal sealed class Lexer : AbstractLexer
			{
				public Lexer(TextReader reader) : base(reader)
				{
					if (reader == null) {
						throw new ArgumentNullException(""reader"");
					}
				}
				
				void Method()
				{
					while(something) {
						if (anything)
							break;
					}
				}
			}";
			Check((TypeDeclaration)ParseUtilCSharp.ParseGlobal(txt, typeof(TypeDeclaration), false, true));
		}
		
		void Check(TypeDeclaration td)
		{
			Assert.AreEqual("Lexer", td.Name);
			Assert.AreEqual(2, td.Children.Count);
			Assert.AreEqual(0, ((ConstructorDeclaration)td.Children[0]).Body.Children.Count);
			Assert.AreEqual(0, ((MethodDeclaration)td.Children[1]).Body.Children.Count);
		}
	}
}
