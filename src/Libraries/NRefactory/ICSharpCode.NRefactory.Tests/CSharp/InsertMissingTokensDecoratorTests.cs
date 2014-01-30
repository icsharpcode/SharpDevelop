// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Editor;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Parser;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Description of InsertMissingTokensDecoratorTests.
	/// </summary>
	[TestFixture]
	public class InsertMissingTokensDecoratorTests
	{
		string[] fileNames;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			string path = Path.GetFullPath (Path.Combine ("..", ".."));
			if (!File.Exists(Path.Combine(path, "NRefactory.sln")))
				throw new InvalidOperationException("Test cannot find the NRefactory source code in " + path);
			fileNames = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
		}
		
		static void RemoveTokens(AstNode node)
		{
			foreach (var child in node.Descendants) {
				if (child is CSharpTokenNode && !(child is CSharpModifierToken))
					child.Remove();
				else if (child.NodeType == NodeType.Whitespace)
					child.Remove();
			}
		}
		
		void AssertOutput(AstNode node)
		{
			RemoveTokens(node);
			StringWriter w = new StringWriter();
			w.NewLine = "\n";
			node.AcceptVisitor(new CSharpOutputVisitor(TokenWriter.CreateWriterThatSetsLocationsInAST(w), FormattingOptionsFactory.CreateSharpDevelop()));
			var doc = new ReadOnlyDocument(w.ToString());
			ConsistencyChecker.CheckMissingTokens(node, "test.cs", doc);
			ConsistencyChecker.CheckPositionConsistency(node, "test.cs", doc);
		}
		
		[Test]
		public void SimpleClass()
		{
			var code = @"class Test
{
}
";
			var unit = SyntaxTree.Parse(code);
			AssertOutput(unit);
		}
		
		[Test]
		public void SimpleMethod()
		{
			var code = @"class Test
{
	void A ()
	{
	}
}
";
			var unit = SyntaxTree.Parse(code);
			AssertOutput(unit);
		}
		
		[Test, Ignore]
		public void SelfTest()
		{
			foreach (var file in fileNames) {
				Console.WriteLine("processing {0}...", file);
				var node = SyntaxTree.Parse(File.ReadAllText(file), file);
				AssertOutput(node);
			}
		}
	}
}
