// 
// TestTypeLevelIndentation.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.FormattingTests
{
	[TestFixture()]
	public class TestTypeLevelIndentation : TestBase
	{
		[Test]
		public void TestUsingDeclarations()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"		using Foo;", @"using Foo;");
		}

		[Test]
		public void TestUsingDeclarationsWithHeader()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, @"// THE SOFTWARE.

using    Foo   ;", @"// THE SOFTWARE.

using Foo;");
		}

		[Test]
		public void TestUsingAliasDeclarations()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
	
			Test(policy, @"		using Foo = Bar;", @"using Foo = Bar;");
		}

		[Test]
		public void TestPreProcessorIndenting()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.DoNotChange;

			Test(policy,
			      @"
class Test {
    #region FooBar

    #endregion
}",
			      @"
class Test {
	#region FooBar

	#endregion
}");
		}

		[Test]
		public void TestTypeWithAttributeIndenging()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.DoNotChange;

			Test(policy,
			     @"
	[Attr]
	class Test {
}",
			     @"
[Attr]
class Test {
}");
		}

		[Test]
		public void TestClassIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.DoNotChange;

			Test(policy,
			     @"			class Test {
}",
			     @"class Test {
}");
		}

		[Test]
		public void TestClassIndentationWithDocComment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.DoNotChange;

			Test(policy,
			     @"/// <summary>
		/// olwcowcolwc
		/// </summary>
			class Test {
}",
			     @"/// <summary>
/// olwcowcolwc
/// </summary>
class Test {
}");
		}

		[Test]
		public void TestAttributeIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.DoNotChange;
			
			Test(policy,
			      @"					[Attribute1]
		[Attribute2()]
          class Test {
}",
			      @"[Attribute1]
[Attribute2 ()]
class Test {
}");
		}

		[Test]
		public void TestClassIndentationInNamespaces()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.NamespaceBraceStyle = BraceStyle.EndOfLine;
			policy.ClassBraceStyle = BraceStyle.DoNotChange;
			
			Test(policy,
			      @"namespace A { class Test {
} }",
			      @"namespace A {
	class Test {
}
}");
		}

		[Test]
		public void TestNoIndentationInNamespaces()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.NamespaceBraceStyle = BraceStyle.EndOfLine;
			policy.ClassBraceStyle = BraceStyle.DoNotChange;
			policy.IndentNamespaceBody = false;
			
			Test(policy,
			      @"namespace A { class Test {
} }",
			      @"namespace A {
class Test {
}
}");
		}

		[Test]
		public void TestClassIndentationInNamespacesCase2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.NamespaceBraceStyle = BraceStyle.NextLine;
			policy.ClassBraceStyle = BraceStyle.NextLine;
			policy.ConstructorBraceStyle = BraceStyle.NextLine;
			
			Test(policy,
			      @"using System;

namespace MonoDevelop.CSharp.Formatting {
	public class FormattingProfileService {
		public FormattingProfileService () {
		}
	}
}",
			      @"using System;

namespace MonoDevelop.CSharp.Formatting
{
	public class FormattingProfileService
	{
		public FormattingProfileService ()
		{
		}
	}
}");
		}

		[Test]
		public void TestIndentClassBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentClassBody = true;
			Test(policy,
			      @"class Test
{
				Test a;
}", @"class Test
{
	Test a;
}");
			
			policy.IndentClassBody = false;
			Test(policy,
			      @"class Test
{
	Test a;
}",
			      @"class Test
{
Test a;
}");
		}

		[Test]
		public void TestDocCommentIndenting()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			Test(policy,
			     @"class Test
{
		/// <summary>
   /// Test
              /// </summary>
	Test a;
}", @"class Test
{
	/// <summary>
	/// Test
	/// </summary>
	Test a;
}");
		}

		[Test]
		public void TestIndentInterfaceBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentInterfaceBody = true;
			
			Test(policy,
			      @"interface Test
{
				Test Foo ();
}", @"interface Test
{
	Test Foo ();
}");
			policy.IndentInterfaceBody = false;
			Test(policy,
			      @"interface Test
{
	Test Foo ();
}", @"interface Test
{
Test Foo ();
}");
		}

		[Test]
		public void TestIndentStructBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentStructBody = true;
			
			Test(policy,
			      @"struct Test
{
				Test Foo ();
}", @"struct Test
{
	Test Foo ();
}");
			policy.IndentStructBody = false;
			Test(policy,
			      @"struct Test
{
	Test Foo ();
}", @"struct Test
{
Test Foo ();
}");
		}

		[Test]
		public void TestIndentEnumBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentEnumBody = true;
			
			Test(policy,
			      @"enum Test
{
				A
}", @"enum Test
{
	A
}");
			policy.IndentEnumBody = false;
			Test(policy,
			      @"enum Test
{
	A
}", @"enum Test
{
A
}");
		}

		[Test]
		public void TestIndentEnumBodyCase2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentEnumBody = true;

			Test(policy,
			     @"enum Test
{
				A , 
	B, 
C
}", @"enum Test
{
	A,
	B,
	C
}");
		}

		[Test]
		public void TestIndentEnumBodyCase3()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentEnumBody = true;

			Test(policy,
			     @"enum Test
{
				A = 3  + 5, 
	B=5  , 
C=5 <<       12
}", @"enum Test
{
	A = 3 + 5,
	B = 5,
	C = 5 << 12
}");
		}

		[Test]
		public void TestIndentMethodBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentMethodBody = true;
			
			Test(policy,
			      @"class Test
{
	Test Foo ()
	{
;
								;
	}
}",
			      @"class Test
{
	Test Foo ()
	{
		;
		;
	}
}");
			policy.IndentMethodBody = false;
			Test(policy,
			      @"class Test
{
	Test Foo ()
	{
		;
		;
	}
}",
			      @"class Test
{
	Test Foo ()
	{
	;
	;
	}
}");
		}

		[Test]
		public void TestIndentMethodBodyOperatorCase()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentMethodBody = true;

			var adapter = Test(policy,
			                    @"class Test
{
	static Test operator+(Test left, Test right)
	{
;
								;
	}
}",
			                    @"class Test
{
	static Test operator+ (Test left, Test right)
	{
		;
		;
	}
}");
			policy.IndentMethodBody = false;
			Continue(policy, adapter, @"class Test
{
	static Test operator+ (Test left, Test right)
	{
	;
	;
	}
}");
		}

		[Test]
		public void TestIndentPropertyBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentPropertyBody = true;
			
			var adapter = Test(policy,
			                    @"class Test
{
	Test TestMe {
			get;
set;
	}
}",
			                    @"class Test
{
	Test TestMe {
		get;
		set;
	}
}");
			policy.IndentPropertyBody = false;
			
			Continue(policy, adapter,
			          @"class Test
{
	Test TestMe {
	get;
	set;
	}
}");
		}

		[Test]
		public void TestIndentPropertyOneLine()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.AllowOneLine;

			Test(policy,
			      @"class Test
{
	Test TestMe {      get;set;                  }
}",
			      @"class Test
{
	Test TestMe { get; set; }
}");
		}

		[Test]
		public void TestIndentPropertyOneLineCase2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.AllowOneLine;
			policy.SimpleGetBlockFormatting = PropertyFormatting.AllowOneLine;
			policy.SimpleSetBlockFormatting = PropertyFormatting.AllowOneLine;

			Test(policy,
			      @"class Test
{
	Test TestMe {      get { ; }set{;}                  }
}",
			      @"class Test
{
	Test TestMe { get { ; } set { ; } }
}");
		}

		[Test]
		public void TestIndentPropertyBodyIndexerCase()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentPropertyBody = true;
			
			var adapter = Test(policy,
			                    @"class Test
{
	Test this[int a] {
			get {
	return null;
}
set {
	;
}
	}
}",
			                    @"class Test
{
	Test this [int a] {
		get {
			return null;
		}
		set {
			;
		}
	}
}");
			policy.IndentPropertyBody = false;
			Continue(policy, adapter,
			          @"class Test
{
	Test this [int a] {
	get {
		return null;
	}
	set {
		;
	}
	}
}");
		}

		[Test]
		public void TestAutoPropertyAlignment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.AutoPropertyFormatting = PropertyFormatting.AllowOneLine;
			var adapter = Test(policy,
			                    @"class Test
{
	Test TestMe { get; set; }
}",
			                    @"class Test
{
	Test TestMe { get; set; }
}");
			policy.AutoPropertyFormatting = PropertyFormatting.ForceNewLine;
			Continue(policy, adapter,
			          @"class Test
{
	Test TestMe {
		get;
		set;
	}
}");
			policy.AutoPropertyFormatting = PropertyFormatting.ForceOneLine;
			
			Continue(policy, adapter,
			          @"class Test
{
	Test TestMe { get; set; }
}");
		}

		[Test]
		public void TestSimplePropertyAlignment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.AllowOneLine;
			var adapter = Test(policy,
			                   @"class Test
{
	Test TestMe { get { ; } set { ; } }
}",
			                   @"class Test
{
	Test TestMe { get { ; } set { ; } }
}");
			policy.SimplePropertyFormatting = PropertyFormatting.ForceNewLine;
			Continue(policy, adapter,
			         @"class Test
{
	Test TestMe {
		get { ; }
		set { ; }
	}
}");
			policy.SimplePropertyFormatting = PropertyFormatting.ForceOneLine;

			Continue(policy, adapter,
			         @"class Test
{
	Test TestMe { get { ; } set { ; } }
}");
		}


		[Test]
		public void TestClashingPropertyAlignment()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.ForceOneLine;
			policy.SimpleGetBlockFormatting = PropertyFormatting.ForceNewLine;
			Test(policy, @"class Test
{
	Test TestMe {
		get { FooBar (); }
	}
}", @"class Test
{
	Test TestMe { get { FooBar (); } }
}");
		}



		[Test]
		public void TestIndentNamespaceBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.ClassBraceStyle = BraceStyle.DoNotChange;
			policy.NamespaceBraceStyle = BraceStyle.EndOfLine;
			policy.IndentNamespaceBody = true;
			var adapter = Test(policy,
			                    @"			namespace Test {
class FooBar {
}
		}",
			                    @"namespace Test {
	class FooBar {
}
}");
			
			policy.IndentNamespaceBody = false;
			Continue(policy, adapter,
			          @"namespace Test {
class FooBar {
}
}");
		}

		[Test]
		public void TestMethodIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.MethodBraceStyle = BraceStyle.DoNotChange;
			
			Test(policy,
			      @"class Test
{
MyType TestMethod () {}
}",
			      @"class Test
{
	MyType TestMethod () {}
}");
		}

		[Test]
		public void TestPropertyIndentation()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.PropertyBraceStyle = BraceStyle.DoNotChange;
			
			Test(policy, 
			      @"class Test
{
				public int Prop { get; set; }
}", @"class Test
{
	public int Prop { get; set; }
}");
		}

		[Test]
		public void TestPropertyIndentationCase2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			
			Test(policy, 
			      @"class Test
{
				public int Prop {
 get;
set;
}
}",
			      @"class Test
{
	public int Prop {
		get;
		set;
	}
}");
		}

		[Test]
		public void TestPropertyIndentationClosingBracketCorrection()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, 
			      @"class Test
{
				public int Prop { get;
				}
}", @"class Test
{
	public int Prop { get; }
}");
		}

		[Test]
		public void TestPropertyIndentationClosingBracketCorrection2()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, 
			      @"class Test
{
				public int Prop {
					get;}
}", @"class Test
{
	public int Prop {
		get;
	}
}");
		}

		[Test]
		public void TestAutoPropertyCorrection()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.AutoPropertyFormatting = PropertyFormatting.ForceNewLine;
			Test(policy, 
			     @"class Test
{
				public int Prop { get;          private set; }
}", @"class Test
{
	public int Prop {
		get;
		private set;
	}
}");
		}

		[Test]
		public void TestSimplePropertyCorrection()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.ForceNewLine;
			Test(policy, 
			     @"class Test
{
				public int Prop { get { ; }         private set {; } }
}", @"class Test
{
	public int Prop {
		get { ; }
		private set { ; }
	}
}");
		}

		[Test]
		public void TestEventField()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, 
@"class Test
{
	public   event 

 EventHandler    TestMe           ;
}",
@"class Test
{
	public event EventHandler TestMe;
}");

		}


		[Test]
		public void TestIndentEventBody()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentEventBody = true;
			
			var adapter = Test(policy, 
			                    @"class Test
{
	public event EventHandler TestMe {
								add {
							;
						}
remove {
	;
}
	}
}",
			                    @"class Test
{
	public event EventHandler TestMe {
		add {
			;
		}
		remove {
			;
		}
	}
}");
			policy.IndentEventBody = false;
			Continue(policy, adapter,
			          @"class Test
{
	public event EventHandler TestMe {
	add {
		;
	}
	remove {
		;
	}
	}
}");
		}

		/// <summary>
		/// Bug 9990 - Formatting a document on save splits event into 'e vent'
		/// </summary>
		[Test]
		public void TestBug9990()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.SimplePropertyFormatting = PropertyFormatting.ForceNewLine;
			Test(policy, 
			     @"class Test
{
		public event EventHandler UpdateStarted = delegate { }; public event EventHandler<UpdateFinishedEventArgs> UpdateFinished = delegate { };
}", @"class Test
{
	public event EventHandler UpdateStarted = delegate { };
	public event EventHandler<UpdateFinishedEventArgs> UpdateFinished = delegate { };
}");
		}

		[Test]
		public void TestPropertyOneLineCorrection()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy,
			      @"class Test
{
	int test { get { return test;}
set { test = value; } }
}",
			      @"class Test
{
	int test { get { return test; } set { test = value; } }
}");
		}

		[Test]
		public void TestConstructorInitializer()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"
class Foo
{
	public Foo ():         base         (0)
	{
	}
}
", @"
class Foo
{
	public Foo () : base (0)
	{
	}
}
");
		}
		[Test]
		public void TestConstructorInitializerCase2()
		{
			var policy = FormattingOptionsFactory.CreateMono();
			Test(policy, @"
class Foo
{
public Foo ()         :
base         (0)
{
}
}
", @"
class Foo
{
	public Foo () :
		base (0)
	{
	}
}
");
		}

		[Test]
		public void TestConstructorInitializerColonDontCare()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.NewLineAfterConstructorInitializerColon = NewLinePlacement.DoNotCare;
			policy.NewLineBeforeConstructorInitializerColon = NewLinePlacement.DoNotCare;
			Test(policy, @"class A
{
	public A ()		:			base ()
	{

	}

	public A ()			: 			
						base ()
	{

	}

	public A ()
						: 					base ()
	{

	}

	public A ()				
		     : 					
						base ()
	{

	}
}",
				@"class A
{
	public A () : base ()
	{

	}

	public A () :
		base ()
	{

	}

	public A ()
		: base ()
	{

	}

	public A ()
		:
		base ()
	{

	}
}");
		}

		[Test]
		public void TestConstructorInitializerColonNewLineBeforeSameLineAfter()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.NewLineBeforeConstructorInitializerColon = NewLinePlacement.NewLine;
			policy.NewLineAfterConstructorInitializerColon = NewLinePlacement.SameLine;

			Test(policy, @"class A
{
	public A ()		:			base ()
	{

	}

	public A ()			: 			
						base ()
	{

	}

	public A ()
						: 					base ()
	{

	}

	public A ()				
		     : 					
						base ()
	{

	}
}",
				@"class A
{
	public A ()
		: base ()
	{

	}

	public A ()
		: base ()
	{

	}

	public A ()
		: base ()
	{

	}

	public A ()
		: base ()
	{

	}
}");
		}

		[Test]
		public void TestIndentPreprocessorStatementsAdd()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentPreprocessorDirectives = true;
			
			Test(policy,
			     @"class Test
{
#region DEBUG
#endregion
}", @"class Test
{
	#region DEBUG

	#endregion
}");
		}
	
		[Test]
		public void TestIndentPreprocessorStatementsRemove()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();
			policy.IndentPreprocessorDirectives = false;
			
			Test(policy,
			     @"class Test
{
	#region DEBUG

	#endregion
}", @"class Test
{
#region DEBUG

#endregion
}");
		}


		[Test]
		public void TestCollectionFieldInitializer ()
		{
			CSharpFormattingOptions policy = FormattingOptionsFactory.CreateMono();

			Test(policy, 
@"using System.Collections.Generic;

class Foo
{
new Dictionary<int,int> o = new Dictionary<int,int> () { 
		{1, 2 },
	{1, 2 },
				{1, 2 },
		{1, 2 }
				}; 
}
", 
@"using System.Collections.Generic;

class Foo
{
	new Dictionary<int,int> o = new Dictionary<int,int> () { 
		{ 1, 2 },
		{ 1, 2 },
		{ 1, 2 },
		{ 1, 2 }
	};
}
");
		}

	}
}
