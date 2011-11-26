// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.GeneralScope
{
	[TestFixture]
	public class PreprocessorDirectiveTests
	{
		[Test]
		public void InactiveIf()
		{
			string program = @"namespace NS {
	#if SOMETHING
	class A {}
	#endif
}";
			NamespaceDeclaration ns = ParseUtilCSharp.ParseGlobal<NamespaceDeclaration>(program);
			Assert.AreEqual(0, ns.Members.Count);
			Assert.AreEqual(7, ns.Children.Count());
			
			Assert.AreEqual(AstNode.Roles.Keyword, ns.Children.ElementAt(0).Role);
			Assert.AreEqual(AstNode.Roles.Identifier, ns.Children.ElementAt(1).Role);
			Assert.AreEqual(AstNode.Roles.LBrace, ns.Children.ElementAt(2).Role);
			Assert.AreEqual(AstNode.Roles.PreProcessorDirective, ns.Children.ElementAt(3).Role);
			Assert.AreEqual(AstNode.Roles.Comment, ns.Children.ElementAt(4).Role);
			Assert.AreEqual(AstNode.Roles.PreProcessorDirective, ns.Children.ElementAt(5).Role);
			Assert.AreEqual(AstNode.Roles.RBrace, ns.Children.ElementAt(6).Role);
			
			var pp = ns.GetChildrenByRole(AstNode.Roles.PreProcessorDirective);
			
			Assert.AreEqual(PreProcessorDirectiveType.If, pp.First().Type);
			Assert.IsFalse(pp.First().Take);
			Assert.AreEqual("SOMETHING", pp.First().Argument);
			Assert.AreEqual(new TextLocation(2, 2), pp.First().StartLocation);
			Assert.AreEqual(new TextLocation(2, 15), pp.First().EndLocation);
			
			var c = ns.GetChildByRole(AstNode.Roles.Comment);
			Assert.AreEqual(CommentType.InactiveCode, c.CommentType);
			Assert.AreEqual(new TextLocation(3, 1), c.StartLocation);
			Assert.AreEqual(new TextLocation(4, 2), c.EndLocation);
			Assert.AreEqual("\tclass A {}\n\t", c.Content.Replace("\r", ""));
			
			Assert.AreEqual(PreProcessorDirectiveType.Endif, pp.Last().Type);
			Assert.AreEqual(string.Empty, pp.Last().Argument);
			Assert.AreEqual(new TextLocation(4, 2), pp.First().StartLocation);
			Assert.AreEqual(new TextLocation(4, 8), pp.First().EndLocation);
		}
	}
}
