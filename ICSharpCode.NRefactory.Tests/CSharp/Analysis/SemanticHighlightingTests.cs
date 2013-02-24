//
// SemanticHighlightingTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.CSharp.Analysis
{
	[TestFixture]
	public class SemanticHighlightingTests
	{
		class TestSemanticHighlightingVisitor : SemanticHighlightingVisitor<FieldInfo>
		{
			public TestSemanticHighlightingVisitor(CSharpAstResolver resolver)
			{
				base.resolver = resolver;
				this.regionStart = new TextLocation (1, 1);
				this.regionEnd = new TextLocation (int.MaxValue, int.MaxValue);
				var fields = typeof (TestSemanticHighlightingVisitor).GetFields (BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (var field in fields) {
					if (field.FieldType == typeof (FieldInfo))
						field.SetValue (this, field);
				}
			}

			List<Tuple<DomRegion, string>> colors = new List<Tuple<DomRegion, string>> ();

			protected override void Colorize(TextLocation start, TextLocation end, FieldInfo color)
			{
				colors.Add (Tuple.Create (new DomRegion (start, end), color != null ? color.Name : null));
			}

			public string GetColor(TextLocation loc)
			{
				foreach (var color in colors) {
					if (color.Item1.IsInside (loc))
						return color.Item2;
				}
				return null;
			}
		}

		static TestSemanticHighlightingVisitor CreateHighighting (string text)
		{
			var syntaxTree = SyntaxTree.Parse (text, "a.cs");
			if (syntaxTree.Errors.Count > 0) {
				Console.WriteLine (text);
				Assert.Fail ("parse error.");
			}
			var project = new CSharpProjectContent().AddAssemblyReferences(new [] { CecilLoaderTests.Mscorlib, CecilLoaderTests.SystemCore });
			var file = syntaxTree.ToTypeSystem();
			project = project.AddOrUpdateFiles(file);

			var resolver = new CSharpAstResolver(project.CreateCompilation(), syntaxTree, file);
			var result = new TestSemanticHighlightingVisitor (resolver);
			syntaxTree.AcceptVisitor (result);
			return result;
		}

		void TestColor(string text, string keywordColor)
		{
			var sb = new StringBuilder ();
			var offsets = new List<int> ();
			foreach (var ch in text) {
				if (ch == '$') {
					offsets.Add (sb.Length);
					continue;
				}
				sb.Append (ch);
			}
			var visitor = CreateHighighting (sb.ToString ());
			var doc = new ReadOnlyDocument (sb.ToString ());

			foreach (var offset in offsets) {
				var loc = doc.GetLocation (offset);
				var color = visitor.GetColor (loc);
				Assert.AreEqual (keywordColor, color, "Color at " + loc + " is wrong:" + color);
			}
		}

		[Test]
		public void TestValueInPropertySetter()
		{
			TestColor (@"class Class { int Property { get {} set { test = $value; } } }", "valueKeywordColor");
		}

		[Test]
		public void TestValueInPropertyGetter()
		{
			TestColor (@"class Class { int value; int Property { get { return $value; } set { } } }", "fieldAccessColor");
		}

		[Test]
		public void TestValueInCustomEvent()
		{
			TestColor (@"using System;
class Class {
	public event EventHandler Property { 
		add { Console.WriteLine ($value); } 
		remove { Console.WriteLine ($value); }
	}
}", "valueKeywordColor");
		}
	
		[Test]
		public void TestExternAliasColor()
		{
			TestColor (@"extern $alias FooBar;", "externAliasKeywordColor");
		}
	}
}

