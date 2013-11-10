// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
