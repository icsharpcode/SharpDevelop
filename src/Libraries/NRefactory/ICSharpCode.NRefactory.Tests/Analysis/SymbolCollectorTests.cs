//
// SymbolCollectorTests.cs
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.CodeCompletion;
using System.Text;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.Analysis
{
	[TestFixture]
	public class SymbolCollectorTests
	{

		void CollectMembers(string code, string memberName, bool includeOverloads = true)
		{
			StringBuilder sb = new StringBuilder();
			List<int> offsets = new List<int>();
			foreach (var ch in code) {
				if (ch == '$') {
					offsets.Add(sb.Length);
					continue;
				}
				sb.Append(ch);
			}
			var syntaxTree = SyntaxTree.Parse(sb.ToString (), "test.cs");
			var unresolvedFile = syntaxTree.ToTypeSystem();
			var compilation = TypeSystemHelper.CreateCompilation(unresolvedFile);

			var symbol = FindReferencesTest.GetSymbol(compilation, memberName);
			var col = new SymbolCollector();
			col.IncludeOverloads = includeOverloads;
			col.GroupForRenaming = true;

			var result = col.GetRelatedSymbols (new TypeGraph (compilation.Assemblies),
			                                   symbol);
			if (offsets.Count != result.Count()) {
				foreach (var a in result)
					Console.WriteLine(a);
			}
			Assert.AreEqual(offsets.Count, result.Count());
			var doc = new ReadOnlyDocument(sb.ToString ());
			result
				.Select(r => doc.GetOffset ((r as IEntity).Region.Begin))
				.SequenceEqual(offsets);
		}

		[Test]
		public void TestSingleInterfaceImpl ()
		{
			var code = @"
interface IA
{
	void $Method();
}

class A : IA
{
	public virtual void $Method() { };
}

class B : A
{
	public override void Method() { };
}

class C : IA
{
	public void $Method() { };
}";
			CollectMembers(code, "IA.Method");
		}


		[Test]
		public void TestMultiInterfacesImpl1 ()
		{
			var code = @"
interface IA
{
	void $Method();
}
interface IB
{
	void $Method();
}
class A : IA, IB
{
	public void $Method() { }
}
class B : IA
{
	public void $Method() { }
}
class C : IB
{
	public void $Method() { }
}";
			CollectMembers(code, "A.Method");
		}


		[Test]
		public void TestOverloads ()
		{
			var code = @"
class A
{
	public void $Method () { }
	public void $Method (int i) { }
	public void $Method (string i) { }
}
";
			CollectMembers(code, "A.Method");
		}

		[Test]
		public void TestConstructor ()
		{
			var code = @"
class $A
{
	public $A() { }
	public $A(int i) { }
}
";
			CollectMembers(code, "A");
		}


		[Test]
		public void TestDestructor ()
		{
			var code = @"
class $A
{
	$~A() { }
}
";
			CollectMembers(code, "A");
		}

		[Test]
		public void TestStaticConstructor ()
		{
			var code = @"
class $A
{
	static $A() { }
	public $A(int i) { }
}
";
			CollectMembers(code, "A");
		}
		
		[Test]
		public void TestShadowedMember ()
		{
			var code = @"
class A
{
	public int $Prop
	{ get; set; }
}
class B : A
{
	public int Prop
	{ get; set; }
}
";
			CollectMembers(code, "A.Prop");
		}



		
		[Test]
		public void TestShadowedMemberCase2 ()
		{
			var code = @"interface IA 
{
	int $Prop { get; set; } 
}

class A : IA
{
	public int $Prop
	{ get; set; }
}

class B : A, IA
{
	public int $Prop
	{ get; set; }
}
";
			CollectMembers(code, "A.Prop");
		}



	}
}

