/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
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
	public class UsingDeclarationTests
	{
		void CheckTwoSimpleUsings(CompilationUnit u)
		{
			Assert.AreEqual(2, u.Children.Count);
			Assert.IsTrue(u.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)u.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(!((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("System", ((Using)ud.Usings[0]).Name);
			
			
			Assert.IsTrue(u.Children[1] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[1];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(!((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("My.Name.Space", ((Using)ud.Usings[0]).Name);
		}
		
		void CheckTwoSimpleAliases(CompilationUnit u)
		{
			Assert.AreEqual(2, u.Children.Count);
			
			Assert.IsTrue(u.Children[0] is UsingDeclaration);
			UsingDeclaration ud = (UsingDeclaration)u.Children[0];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("TESTME", ((Using)ud.Usings[0]).Alias);
			Assert.AreEqual("System", ((Using)ud.Usings[0]).Name);
			
			Assert.IsTrue(u.Children[1] is UsingDeclaration);
			ud = (UsingDeclaration)u.Children[1];
			Assert.AreEqual(1, ud.Usings.Count);
			Assert.IsTrue(((Using)ud.Usings[0]).IsAlias);
			Assert.AreEqual("myAlias", ((Using)ud.Usings[0]).Alias);
			Assert.AreEqual("My.Name.Space", ((Using)ud.Usings[0]).Name);
		}
		
		#region C#
		[Test]
		public void CSharpWrongUsingTest()
		{
			string program = "using\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
			parser.Parse();
			Assert.IsTrue(parser.Errors.count > 0);
		}
		
		[Test]
		public void CSharpDeclarationTest()
		{
			string program = "using System;\n" +
			                 "using My.Name.Space;\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CheckTwoSimpleUsings(parser.CompilationUnit);
		}
		
		[Test]
		public void CSharpUsingAliasDeclarationTest()
		{
			string program = "using TESTME=System;\n" +
			                 "using myAlias=My.Name.Space;\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.CSharp, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetWrongUsingTest()
		{
			string program = "Imports\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.VBNet, new StringReader(program));
			parser.Parse();
			Assert.IsTrue(parser.Errors.count > 0);
		}
		[Test]
		public void VBNetDeclarationTest()
		{
			string program = "Imports System\n" +
			                 "Imports My.Name.Space\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			CheckTwoSimpleUsings(parser.CompilationUnit);
		}
		
		[Test]
		public void VBNetUsingAliasDeclarationTest()
		{
			string program = "Imports TESTME=System\n" +
			                 "Imports myAlias=My.Name.Space\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			
		}
		
		[Test]
		public void VBNetComplexUsingAliasDeclarationTest()
		{
			string program = "Imports NS1, AL=NS2, NS3, AL2=NS4, NS5\n";
			IParser parser = ParserFactory.CreateParser(SupportedLanguages.VBNet, new StringReader(program));
			parser.Parse();
			
			Assert.AreEqual("", parser.Errors.ErrorOutput);
			// TODO : Extend test ... 
		}
		#endregion
	}
}
